using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Infrastructure.Persistence.AzureBlob.Services
{
    public class FieldBlobUploadService : IFieldBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public FieldBlobUploadService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }

        public async Task<string> UploadFileAsync(string filePath)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = $"field-images/{Guid.NewGuid()}_{Path.GetFileName(filePath)}";
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await using var fileStream = File.OpenRead(filePath);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = GetContentType(filePath) });
            return blobClient.Uri.ToString();
        }

        public async Task<string> UploadStreamAsync(
            Stream content,
            string fileName,
            string contentType,
            CancellationToken ct = default)
        {
            var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

            var blobName = $"field-images/{Guid.NewGuid()}_{fileName}";
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(
                content,
                new BlobHttpHeaders { ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType },
                cancellationToken: ct);
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
