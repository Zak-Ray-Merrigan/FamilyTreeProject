using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Domain;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;

namespace VirtualFamilyMuseumLibraryTest.Drive.Domain
{
    // Integration tests against the real "family6f26m763wyjwkdrive" Azure Storage account,
    // exercising FamilyDriveService.DownloadImageAsync end to end against the images container.
    // This codebase has no mocking library and no fakes for IFamilyDriveRepository (see
    // FamilyDriveTest/TemplateReaderTest for the same convention), so this bootstraps via the
    // generic host the same way those two do. Requires an Azure identity (e.g. `az login`) with
    // Storage Blob Data Owner/Contributor access on the images container.
    public class FamilyDriveServiceTest
    {
        private IHost host = null!;
        private FamilyDriveService service = null!;
        private IFamilyDriveRepository drive = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", ExtensionsTest.FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", ExtensionsTest.FAMILY_VAULT_TEST_ENDPOINT);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConstantStorePipeline(ExecutionTypes.Test);
            builder.AddFamilyInsights(ExecutionTypes.Test);
            builder.AddFamilyDrive();
            host = builder.Build();
            service = host.Services.GetRequiredService<FamilyDriveService>();
            drive = host.Services.GetRequiredService<IFamilyDriveRepository>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            host.Dispose();
        }

        // =====================================================================
        // DownloadImageAsync — invalid blob name, fails before any repository/network call
        // =====================================================================

        [Test]
        public void DownloadImageAsyncShouldThrowArgumentNullExceptionForNullBlobName()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DownloadImageAsync(null!));
        }

        [Test]
        public void DownloadImageAsyncShouldThrowNotSupportedExceptionForUnrecognizedContainer()
        {
            // No domain-level container check anymore: this propagates unwrapped straight from
            // repository.GetAsync -> FamilyDrive.GetBlobClient -> DriveExtensions.GetContainer,
            // which doesn't recognize "documents" as a container at all.
            Assert.ThrowsAsync<NotSupportedException>(async () => await service.DownloadImageAsync("documents/file.txt"));
        }

        [Test]
        public async Task DownloadImageAsyncShouldThrowInvalidOperationExceptionForTemplateBlobName()
        {
            // GetContainer recognizes "templates" just fine, so this actually fetches the blob
            // (must exist for the content-type check further down to be what rejects it, rather
            // than the not-found check firing first) and gets ContentType == Application_PDF back
            // — a mismatch against the Image_JPEG DownloadImageAsync expects.
            string blobName = NewScratchTemplateBlobName();
            try
            {
                using MemoryStream upload = new(NewValidPdfBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadImageAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // DownloadImageAsync — not found
        // =====================================================================

        [Test]
        public void DownloadImageAsyncShouldThrowFileNotFoundExceptionForNonExistentImageBlob()
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () => await service.DownloadImageAsync(NewScratchImageBlobName()));
        }

        // =====================================================================
        // DownloadImageAsync — success, round trip against a scratch image blob
        // =====================================================================

        [Test]
        public async Task DownloadImageAsyncShouldReturnSuccessStatusAndDescriptiveMessageForExistingImageBlob()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new(NewValidJpegBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                FamilyDriveResult<FamilyBlobResource> result = await service.DownloadImageAsync(blobName);

                Assert.That(result.Status, Is.EqualTo(FamilyDriveResultStatuses.Success));
                Assert.That(result.Message, Is.EqualTo($"{blobName} (image/jpeg) has been downloaded."));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DownloadImageAsyncShouldReturnPayloadWithMatchingBlobNameAndContentType()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new(NewValidJpegBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                FamilyDriveResult<FamilyBlobResource> result = await service.DownloadImageAsync(blobName);

                Assert.That(result.Payload, Is.Not.Null);
                Assert.That(result.Payload!.BlobName, Is.EqualTo(blobName));
                Assert.That(result.Payload.ContentType, Is.EqualTo(FamilyContentTypes.Image_JPEG));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DownloadImageAsyncShouldReturnPayloadWithContentMatchingUploadedBytes()
        {
            string blobName = NewScratchImageBlobName();
            byte[] content = NewValidJpegBytes();
            try
            {
                using (MemoryStream upload = new(content))
                {
                    await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);
                }

                FamilyDriveResult<FamilyBlobResource> result = await service.DownloadImageAsync(blobName);

                using MemoryStream downloaded = new();
                await result.Payload!.Content.CopyToAsync(downloaded);
                Assert.That(downloaded.ToArray(), Is.EqualTo(content));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DownloadImageAsyncShouldThrowInvalidOperationExceptionForCorruptedImageBlob()
        {
            // A stored blob with the right container/content-type header but bytes that aren't a
            // real, decodable JPEG — DownloadImageAsync's ImageSharp-based structural check should
            // reject this even though the shallow magic-byte check UploadImageAsync would have
            // performed at write time (SOI marker only) is satisfied.
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new([0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02, 0x03]);
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadImageAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // DownloadTemplateAsync — invalid blob name, fails before any repository/network call
        // =====================================================================

        [Test]
        public void DownloadTemplateAsyncShouldThrowArgumentNullExceptionForNullBlobName()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.DownloadTemplateAsync(null!));
        }

        [Test]
        public void DownloadTemplateAsyncShouldThrowNotSupportedExceptionForUnrecognizedContainer()
        {
            Assert.ThrowsAsync<NotSupportedException>(async () => await service.DownloadTemplateAsync("documents/file.txt"));
        }

        [Test]
        public async Task DownloadTemplateAsyncShouldThrowInvalidOperationExceptionForImageBlobName()
        {
            // Must exist for the content-type mismatch (Image_JPEG vs. the Application_PDF
            // DownloadTemplateAsync expects) to be what rejects it, rather than not-found firing first.
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new(NewValidJpegBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadTemplateAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // DownloadTemplateAsync — not found
        // =====================================================================

        [Test]
        public void DownloadTemplateAsyncShouldThrowFileNotFoundExceptionForNonExistentTemplateBlob()
        {
            Assert.ThrowsAsync<FileNotFoundException>(async () => await service.DownloadTemplateAsync(NewScratchTemplateBlobName()));
        }

        // =====================================================================
        // DownloadTemplateAsync — success, round trip against a scratch template blob
        // =====================================================================

        [Test]
        public async Task DownloadTemplateAsyncShouldReturnSuccessStatusAndDescriptiveMessageForExistingTemplateBlob()
        {
            string blobName = NewScratchTemplateBlobName();
            try
            {
                using MemoryStream upload = new(NewValidPdfBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                FamilyDriveResult<FamilyBlobResource> result = await service.DownloadTemplateAsync(blobName);

                Assert.That(result.Status, Is.EqualTo(FamilyDriveResultStatuses.Success));
                Assert.That(result.Message, Is.EqualTo($"{blobName} (application/pdf) has been downloaded."));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DownloadTemplateAsyncShouldReturnPayloadWithContentMatchingUploadedBytes()
        {
            string blobName = NewScratchTemplateBlobName();
            byte[] content = NewValidPdfBytes();
            try
            {
                using (MemoryStream upload = new(content))
                {
                    await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);
                }

                FamilyDriveResult<FamilyBlobResource> result = await service.DownloadTemplateAsync(blobName);

                using MemoryStream downloaded = new();
                await result.Payload!.Content.CopyToAsync(downloaded);
                Assert.That(downloaded.ToArray(), Is.EqualTo(content));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // DownloadTemplateAsync — corrupted content, two distinct shapes of malformed PDF
        // =====================================================================

        [Test]
        public async Task DownloadTemplateAsyncShouldThrowInvalidOperationExceptionForHeaderOnlyGarbageBlob()
        {
            // Real "%PDF-1.7" header, garbage after it — exercises the PdfException catch.
            string blobName = NewScratchTemplateBlobName();
            try
            {
                byte[] content = System.Text.Encoding.ASCII.GetBytes("%PDF-1.7\ngarbagegarbagegarbagegarbage");
                using MemoryStream upload = new(content);
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadTemplateAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DownloadTemplateAsyncShouldThrowInvalidOperationExceptionForNoPdfHeaderBlob()
        {
            // No "%PDF" signature at all — exercises the separate iText.IO.Exceptions.IOException
            // catch ("PDF header not found"), which PdfException alone does not cover.
            string blobName = NewScratchTemplateBlobName();
            try
            {
                using MemoryStream upload = new([0x00, 0x01, 0x02, 0x03, 0x04, 0x05]);
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadTemplateAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // RemoveImageAsync — invalid blob name, fails before any repository/network call
        // =====================================================================

        [Test]
        public void RemoveImageAsyncShouldThrowArgumentNullExceptionForNullBlobName()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.RemoveImageAsync(null!));
        }

        [Test]
        public void RemoveImageAsyncShouldThrowNotSupportedExceptionForUnrecognizedContainer()
        {
            Assert.ThrowsAsync<NotSupportedException>(async () => await service.RemoveImageAsync("documents/file.txt"));
        }

        [Test]
        public async Task RemoveImageAsyncShouldThrowInvalidOperationExceptionForTemplateBlobNameAndLeaveItInPlace()
        {
            string blobName = NewScratchTemplateBlobName();
            try
            {
                using MemoryStream upload = new(NewValidPdfBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.RemoveImageAsync(blobName));

                // The content-type mismatch must be caught before deletion happens, not after.
                Assert.That(await drive.GetAsync(blobName), Is.Not.Null);
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // RemoveImageAsync — not found is a normal outcome, not an exception
        // =====================================================================

        [Test]
        public async Task RemoveImageAsyncShouldReturnFalseForNonExistentImageBlob()
        {
            bool result = await service.RemoveImageAsync(NewScratchImageBlobName());
            Assert.That(result, Is.False);
        }

        // =====================================================================
        // RemoveImageAsync — success, actually removes the blob
        // =====================================================================

        [Test]
        public async Task RemoveImageAsyncShouldReturnTrueAndDeleteExistingImageBlob()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new(NewValidJpegBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                bool result = await service.RemoveImageAsync(blobName);

                Assert.That(result, Is.True);
                Assert.That(await drive.GetAsync(blobName), Is.Null);
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // RemoveTemplateAsync — invalid blob name, fails before any repository/network call
        // =====================================================================

        [Test]
        public void RemoveTemplateAsyncShouldThrowArgumentNullExceptionForNullBlobName()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.RemoveTemplateAsync(null!));
        }

        [Test]
        public void RemoveTemplateAsyncShouldThrowNotSupportedExceptionForUnrecognizedContainer()
        {
            Assert.ThrowsAsync<NotSupportedException>(async () => await service.RemoveTemplateAsync("documents/file.txt"));
        }

        [Test]
        public async Task RemoveTemplateAsyncShouldThrowInvalidOperationExceptionForImageBlobNameAndLeaveItInPlace()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new(NewValidJpegBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await service.RemoveTemplateAsync(blobName));

                Assert.That(await drive.GetAsync(blobName), Is.Not.Null);
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // RemoveTemplateAsync — not found is a normal outcome, not an exception
        // =====================================================================

        [Test]
        public async Task RemoveTemplateAsyncShouldReturnFalseForNonExistentTemplateBlob()
        {
            bool result = await service.RemoveTemplateAsync(NewScratchTemplateBlobName());
            Assert.That(result, Is.False);
        }

        // =====================================================================
        // RemoveTemplateAsync — success, actually removes the blob
        // =====================================================================

        [Test]
        public async Task RemoveTemplateAsyncShouldReturnTrueAndDeleteExistingTemplateBlob()
        {
            string blobName = NewScratchTemplateBlobName();
            try
            {
                using MemoryStream upload = new(NewValidPdfBytes());
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Application_PDF);

                bool result = await service.RemoveTemplateAsync(blobName);

                Assert.That(result, Is.True);
                Assert.That(await drive.GetAsync(blobName), Is.Null);
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // UploadImageAsync — invalid content, fails before any repository/network call
        // =====================================================================

        [Test]
        public void UploadImageAsyncShouldThrowArgumentNullExceptionForNullContent()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UploadImageAsync(null!, FamilyContentTypes.Image_JPEG));
        }

        // =====================================================================
        // UploadImageAsync — predictable failures, no exception, no upload attempted
        // =====================================================================

        [Test]
        public async Task UploadImageAsyncShouldReturnUnsupportedContentTypeStatusForNonJpegContentType()
        {
            using MemoryStream upload = new(NewValidPdfBytes());

            FamilyDriveResult<FamilyBlobResource> result = await service.UploadImageAsync(upload, FamilyContentTypes.Application_PDF);

            Assert.That(result.Status, Is.EqualTo(FamilyDriveResultStatuses.UnsupportedContentType));
            Assert.That(result.Message, Is.EqualTo("Content-type must be \"image/jpeg\"."));
            Assert.That(result.Payload, Is.Null);
        }

        [Test]
        public async Task UploadImageAsyncShouldReturnIllegitimateContentStatusForCorruptedJpegBytes()
        {
            using MemoryStream upload = new([0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02, 0x03]);

            FamilyDriveResult<FamilyBlobResource> result = await service.UploadImageAsync(upload, FamilyContentTypes.Image_JPEG);

            Assert.That(result.Status, Is.EqualTo(FamilyDriveResultStatuses.IllegitimateContent));
            Assert.That(result.Message, Is.EqualTo("Content isn't a legitimate JPEG image."));
            Assert.That(result.Payload, Is.Null);
        }

        // =====================================================================
        // UploadImageAsync — success, round trip against a real upload
        // =====================================================================

        [Test]
        public async Task UploadImageAsyncShouldReturnSuccessStatusAndUploadedBlobForLegitimateJpeg()
        {
            byte[] content = NewValidJpegBytes();
            using MemoryStream upload = new(content);
            string? blobName = null;
            try
            {
                FamilyDriveResult<FamilyBlobResource> result = await service.UploadImageAsync(upload, FamilyContentTypes.Image_JPEG);
                blobName = result.Payload?.BlobName;

                Assert.That(result.Status, Is.EqualTo(FamilyDriveResultStatuses.Success));
                Assert.That(result.Payload, Is.Not.Null);
                Assert.That(result.Payload!.BlobName, Does.StartWith("images/"));
                Assert.That(result.Payload.BlobName, Does.EndWith(".jpg"));
                Assert.That(result.Payload.ContentType, Is.EqualTo(FamilyContentTypes.Image_JPEG));
                Assert.That(result.Message, Is.EqualTo($"{result.Payload.BlobName} (image/jpeg) has been uploaded."));

                FamilyBlobResource? uploaded = await drive.GetAsync(result.Payload.BlobName);
                Assert.That(uploaded, Is.Not.Null);
                using MemoryStream downloaded = new();
                await uploaded!.Content.CopyToAsync(downloaded);
                Assert.That(downloaded.ToArray(), Is.EqualTo(content));
            }
            finally
            {
                if (blobName is not null)
                {
                    await drive.DeleteAsync(blobName);
                }
            }
        }

        private static string NewScratchImageBlobName()
        {
            return $"images/integration-test-{Guid.NewGuid()}.jpg";
        }

        private static byte[] NewValidJpegBytes()
        {
            using SKBitmap bitmap = new(1, 1);
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData encoded = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            return encoded.ToArray();
        }

        private static string NewScratchTemplateBlobName()
        {
            return $"templates/integration-test-{Guid.NewGuid()}.pdf";
        }

        private static byte[] NewValidPdfBytes()
        {
            using MemoryStream buffer = new();
            using (iText.Kernel.Pdf.PdfWriter writer = new(buffer))
            {
                writer.SetCloseStream(false);
                using iText.Kernel.Pdf.PdfDocument document = new(writer);
                document.AddNewPage();
            }
            return buffer.ToArray();
        }
    }
}
