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
        public void DownloadImageAsyncShouldThrowInvalidOperationExceptionForUnrecognizedContainer()
        {
            // Exercises the caught-NotSupportedException path: GetContainer itself doesn't
            // recognize "documents" as a container at all.
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadImageAsync("documents/file.txt"));
        }

        [Test]
        public void DownloadImageAsyncShouldThrowInvalidOperationExceptionForTemplateBlobName()
        {
            // Exercises the other branch: GetContainer recognizes "templates" just fine, it's
            // simply the wrong container for an image download.
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadImageAsync("templates/2026/Aug/14/SomeFamily.pdf"));
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
        public void DownloadTemplateAsyncShouldThrowInvalidOperationExceptionForUnrecognizedContainer()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadTemplateAsync("documents/file.txt"));
        }

        [Test]
        public void DownloadTemplateAsyncShouldThrowInvalidOperationExceptionForImageBlobName()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DownloadTemplateAsync($"images/{Guid.NewGuid()}.jpg"));
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
