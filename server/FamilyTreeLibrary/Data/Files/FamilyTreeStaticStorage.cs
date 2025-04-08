using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
namespace FamilyTreeLibrary.Data.Files
{
    public class FamilyTreeStaticStorage(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
    {
        private readonly string connectionString = vault["StorageAccountConnectionString"].AsString;
        private readonly FamilyTreeConfiguration configuration = configuration;
        
        public void ArchiveImage(string blobUri)
        {
            BlobClient client = GetClient(blobUri);
            client.SetAccessTier(AccessTier.Archive);
        }
        public MemoryStream GetStream(string blobUri)
        {
            BlobClient client = GetClient(blobUri);
            MemoryStream stream = new();
            client.DownloadTo(stream);
            stream.Position = 0;
            return stream;
        }

        public void MigrateLogs()
        {
            string[] sourceContainerNames = ["insights-logs-storageread", "insights-logs-storagewrite", "insights-metrics-pt1m"];
            string destinationContainerName = configuration["Storage:Container:Logs"];
            BlobServiceClient serviceClient = new(connectionString);
            foreach (string sourceContainerName in sourceContainerNames)
            {
                BlobContainerClient sourceContainer = serviceClient.GetBlobContainerClient(sourceContainerName);
                IEnumerable<BlobItem> blobItems = sourceContainer.GetBlobs();
                foreach (BlobItem item in blobItems)
                {
                    string blobName = item.Name;
                    BlobClient sourceBlob = sourceContainer.GetBlobClient(blobName);
                    BlobClient destinationBlob = serviceClient.GetBlobContainerClient(destinationContainerName).GetBlobClient(blobName);
                    CopyFromUriOperation copyOperation = destinationBlob.StartCopyFromUri(sourceBlob.Uri);
                    copyOperation.WaitForCompletion();
                    destinationBlob.SetAccessTier(AccessTier.Cold);
                    sourceBlob.DeleteIfExists();
                }
            }
        }

        public string UploadImage(FileStream imageStream)
        {
            BlobServiceClient blobService = new(connectionString);
            BlobContainerClient imageContainer = blobService.GetBlobContainerClient(configuration["Storage:Containers:Images"]);
            BlobClient image = imageContainer.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(imageStream.Name));
            image.Upload(imageStream);
            image.SetAccessTier(AccessTier.Hot);
            return image.Uri.ToString();
        }

        public string UploadTemplate(FileStream templateStream)
        {
            BlobServiceClient blobService = new(connectionString);
            BlobContainerClient templateContainer = blobService.GetBlobContainerClient(configuration["Storage:Containers:Templates"]);
            BlobClient template = templateContainer.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(templateStream.Name));
            template.Upload(templateStream);
            template.SetAccessTier(AccessTier.Cool);
            return template.Uri.ToString();
        }

        private BlobClient GetClient(string blobUri)
        {
            Uri uri = new(blobUri);
            string[] segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
            string containerName = segments[0];
            string blobName = segments[1];
            return new(connectionString, containerName, blobName);
        }

    }
}