using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Assignment1.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("event-banners");

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(image.OpenReadStream(), new BlobHttpHeaders
            {
                ContentType = image.ContentType
            });

            return blobClient.Uri.ToString();
        }
    }
}