using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace InvoicePdfGenerationQueue.Processor.Function.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }
        public async Task UploadPdfAsync(string fileName, Stream content)
        {
            await _containerClient.CreateIfNotExistsAsync();

            var blobClient = _containerClient.GetBlobClient(fileName);

            content.Position = 0; 

            await blobClient.UploadAsync(
                content,
                overwrite: true
            );

            await blobClient.SetAccessTierAsync(AccessTier.Cool);
        }
    }
}
