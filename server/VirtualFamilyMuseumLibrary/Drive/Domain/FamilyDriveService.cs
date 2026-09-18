using Microsoft.Extensions.Logging;
using SkiaSharp;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
namespace VirtualFamilyMuseumLibrary.Drive.Domain
{
    public class FamilyDriveService(ILogger<FamilyDriveService> loggerIn, IFamilyDriveRepository repositoryIn)
    {
        private readonly ILogger<FamilyDriveService> logger = loggerIn;
        private readonly IFamilyDriveRepository repository = repositoryIn;

        public Task<FamilyDriveResult<FamilyBlobResource>> DownloadImageAsync(string blobName)
        {
            return DownloadAsync(blobName, FamilyContentTypes.Image_JPEG, IsLegitimateJpeg, "a legitimate JPEG image");
        }

        public Task<FamilyDriveResult<FamilyBlobResource>> DownloadTemplateAsync(string blobName)
        {
            return DownloadAsync(blobName, FamilyContentTypes.Application_PDF, IsLegitimatePdf, "a legitimate PDF");
        }

        public Task<bool> RemoveImageAsync(string blobName)
        {
            return RemoveAsync(blobName, FamilyContentTypes.Image_JPEG);
        }

        public Task<bool> RemoveTemplateAsync(string blobName)
        {
            return RemoveAsync(blobName, FamilyContentTypes.Application_PDF);
        }

        // Shared skeleton behind both public Download*Async methods: fetch the blob, validate its
        // stored content-type, then buffer and structurally validate the actual bytes before
        // handing back a Success result. Parameterized rather than duplicated per entity type,
        // since the two callers previously diverged only in which content-type/legitimacy-check
        // applied — and a bug fixed in one copy had no guarantee of being caught in the other.
        //
        // No separate container check: repository.GetAsync eventually calls
        // DriveExtensions.GetContainer, which already throws NotSupportedException on its own for
        // a name that isn't images/templates prefixed at all — that propagates unwrapped as the
        // unexpected failure it is. A recognized-but-wrong container (e.g. a templates blobName
        // passed to DownloadImageAsync) doesn't need a separate check either: container and
        // content-type are 1:1 by construction in this system (TemplateWriter always saves
        // Application_PDF, UploadImageAsync always saves Image_JPEG), so fetching from the wrong
        // container always produces a content-type mismatch, which the check below already covers.
        private async Task<FamilyDriveResult<FamilyBlobResource>> DownloadAsync(string blobName, FamilyContentTypes expectedContentType,
            Func<Stream, bool> isLegitimate, string legitimacyDescription)
        {
            logger.LogInformation("Downloading {BlobName} from the family drive.", blobName);
            EnsureBlobNameNotNull(blobName);

            FamilyBlobResource? resource = await repository.GetAsync(blobName);
            if (resource is null)
            {
                FileNotFoundException ex = new($"{blobName} isn't found.", blobName);
                logger.LogError(ex, "{BlobName} isn't found.", blobName);
                throw ex;
            }
            EnsureContentType(blobName, resource.ContentType, expectedContentType);

            // The Azure download stream isn't guaranteed to be seekable, and the legitimacy checks
            // consume whatever they read from it — so the content is buffered into a seekable copy
            // first, both to validate it and to hand the controller back a stream still positioned
            // at 0, regardless of what the original repository stream supported. The original is
            // fully drained here and never returned to the caller, so it's disposed once copied.
            Stream buffered = new MemoryStream();
            await using (resource.Content)
            {
                await resource.Content.CopyToAsync(buffered);
            }
            buffered.Position = 0;
            if (!isLegitimate(buffered))
            {
                await buffered.DisposeAsync();
                InvalidOperationException ex = new($"{blobName} isn't {legitimacyDescription}.");
                logger.LogError(ex, "{BlobName} isn't {LegitimacyDescription}.", blobName, legitimacyDescription);
                throw ex;
            }

            FamilyBlobResource verifiedResource = new()
            {
                BlobName = resource.BlobName,
                BlobUrl = resource.BlobUrl,
                Content = buffered,
                ContentType = resource.ContentType
            };

            logger.LogInformation("{BlobName} has been found.", blobName);
            return new FamilyDriveResult<FamilyBlobResource>
            {
                Status = FamilyDriveResultStatuses.Success,
                Message = $"{blobName} ({resource.ContentType.GetContentType()}) has been downloaded.",
                Payload = verifiedResource
            };
        }

        // GetContainer (reached transitively via repository.GetAsync/DeleteAsync) assumes a
        // non-null string, so a null blobName would otherwise surface as a raw
        // NullReferenceException instead of an intentional, well-described exception.
        private void EnsureBlobNameNotNull(string blobName)
        {
            if (blobName is null)
            {
                ArgumentNullException ex = new(nameof(blobName));
                logger.LogError(ex, "Blob name can't be null.");
                throw ex;
            }
        }

        private void EnsureContentType(string blobName, FamilyContentTypes actualContentType, FamilyContentTypes expectedContentType)
        {
            if (actualContentType != expectedContentType)
            {
                string expectedContentTypeText = expectedContentType.GetContentType();
                InvalidOperationException ex = new($"{blobName} must have a content-type of \"{expectedContentTypeText}\".");
                logger.LogError(ex, "{BlobName} must have a content-type of \"{ContentTypeText}\".", blobName, expectedContentTypeText);
                throw ex;
            }
        }

        // Structural validation, not a full pixel decode: confirms the buffered content parses as
        // a real JPEG (catching corruption/format-spoofing beyond the magic-byte check
        // UploadImageAsync does at write time), without paying for a full decode on every download.
        // SKCodec.Create reports failure via the out result code rather than throwing, so this
        // doesn't need exception handling the way the iText-based IsLegitimatePdf below does.
        // SKCodec.Create(Stream, ...) takes ownership of and disposes whatever stream it's given
        // once the codec is disposed — confirmed by testing, not documentation — which would close
        // `content` out from under the caller (still needed afterward as Payload.Content on
        // success). Wrapping it in an SKManagedStream with disposeManagedStream: false keeps
        // ownership with the caller.
        private static bool IsLegitimateJpeg(Stream content)
        {
            content.Position = 0;
            using SKManagedStream skStream = new(content, false);
            using SKCodec? codec = SKCodec.Create(skStream, out SKCodecResult result);
            content.Position = 0;
            return result == SKCodecResult.Success && codec is not null && codec.EncodedFormat == SKEncodedImageFormat.Jpeg;
        }

        private static bool IsLegitimatePdf(Stream content)
        {
            content.Position = 0;
            try
            {
                using PdfReader reader = new(content);
                using PdfDocument document = new(reader);
                return document.GetNumberOfPages() > 0;
            }
            catch (PdfException)
            {
                return false;
            }
            catch (iText.IO.Exceptions.IOException)
            {
                // A stream with no recognizable PDF header at all (e.g. "PDF header not found")
                // throws this instead of PdfException — a different type, in a different iText
                // namespace, confirmed by testing against garbage input directly rather than
                // assumed from the catch clause alone.
                return false;
            }
            finally
            {
                content.Position = 0;
            }
        }

        // Shared skeleton behind both public Remove*Async methods. Fetches first rather than
        // deleting unconditionally, specifically so the content-type can be checked and the
        // deletion rejected before it happens — FamilyDrive.DeleteAsync itself deletes
        // unconditionally the moment it finds a blob, returning metadata only afterward, so there's
        // no way to reject a wrong-content-type blob through that call alone. Unlike
        // DownloadAsync's not-found case, "nothing to remove" is a normal outcome here, not an
        // unexpected one, so it returns false rather than throwing.
        private async Task<bool> RemoveAsync(string blobName, FamilyContentTypes expectedContentType)
        {
            logger.LogInformation("Removing {BlobName} from the family drive.", blobName);
            EnsureBlobNameNotNull(blobName);

            FamilyBlobResource? resource = await repository.GetAsync(blobName);
            if (resource is null)
            {
                logger.LogInformation("{BlobName} isn't found; nothing to remove.", blobName);
                return false;
            }
            await resource.Content.DisposeAsync();
            EnsureContentType(blobName, resource.ContentType, expectedContentType);

            await repository.DeleteAsync(blobName);
            logger.LogInformation("{BlobName} has been removed.", blobName);
            return true;
        }
    }
}