using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ST10028058_CLDV6212_POE.Models;
using System.IO;
using System.Threading.Tasks;

namespace ST10028058_CLDV6212_POE.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "abcblob";

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteBlobAsync(string blobUri)
        {
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1];
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

    }   

    
}

//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.