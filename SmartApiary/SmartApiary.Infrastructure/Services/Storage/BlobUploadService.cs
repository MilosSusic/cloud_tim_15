using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SmartApiary.Infrastructure.Services.Storage
{
    public class BlobUploadService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobUploadService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<string> UploadFileAsync(string filePath)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = $"tools-upload/{Guid.NewGuid()}_{Path.GetFileName(filePath)}";
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await using var fileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = GetContentType(filePath) });
            return blobClient.Uri.ToString();
        }

        private static string GetContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
