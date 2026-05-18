using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmartApiary.Application.Common;
using SmartApiary.Application.Common.Extensions;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Infrastructure.Persistence.AzureBlob.Services
{
    public class AzureBlobStorage : IBlobStorageService
    {
        private readonly BlobContainerClient _apiaryContainerClient;
        private readonly BlobContainerClient _fieldContainerClient;
        private readonly ILogger _logger;

        public AzureBlobStorage(
            string apiaryConnectionString,
            string apiaryContainerName,
            string fieldConnectionString,
            string fieldContainerName,
            ILogger<AzureBlobStorage>? logger = null)
        {
            _apiaryContainerClient = new BlobContainerClient(apiaryConnectionString, apiaryContainerName);
            _fieldContainerClient = new BlobContainerClient(fieldConnectionString, fieldContainerName);
            _logger = logger ?? NullLogger<AzureBlobStorage>.Instance;
        }

        public AzureBlobStorage(string connectionString, string containerName)
            : this(connectionString, containerName, connectionString, containerName)
        {
        }

        public async Task<string> UploadStreamAsync(
            Stream content,
            string fileName,
            BlobCategory category,
            string contentType,
            CancellationToken ct = default)
        {
            var metadata = new BlobMetadata(category, fileName);
            var blobPath = BlobPathExtensions.GetBlobPath(metadata);
            var storedMetadata = metadata with { FileName = blobPath };

            await SaveAsync(new FileData<BlobMetadata>(
                storedMetadata,
                await ReadToEndAsync(content, ct),
                contentType), ct);

            return GetUrl(storedMetadata);
        }

        public async Task SaveAsync(FileData<BlobMetadata> file, CancellationToken ct = default)
        {
            var containerClient = GetContainerClient(file.Metadata.Category);
            var blobPath = file.Metadata.FileName;

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

            var blobClient = containerClient.GetBlobClient(blobPath);
            await using var stream = new MemoryStream(file.Content);

            _logger.LogInformation("Uploading to container: {Container}, Path: {Path}", containerClient.Name, blobPath);

            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? GetContentType(blobPath)
                    : file.ContentType
            }, cancellationToken: ct);
        }

        public async Task<bool> ExistsAsync(BlobMetadata metadata, CancellationToken ct = default)
        {
            var response = await GetContainerClient(metadata.Category)
                .GetBlobClient(metadata.FileName)
                .ExistsAsync(ct);
            return response.Value;
        }

        public async Task DeleteAsync(BlobMetadata metadata, CancellationToken ct = default)
        {
            await GetContainerClient(metadata.Category)
                .GetBlobClient(metadata.FileName)
                .DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
        }

        public string GetUrl(BlobMetadata metadata)
        {
            return GetContainerClient(metadata.Category).GetBlobClient(metadata.FileName).Uri.ToString();
        }

        public async Task<string> UploadFileAsync(string filePath, CancellationToken ct = default)
        {
            await using var fileStream = File.OpenRead(filePath);
            return await UploadStreamAsync(fileStream, Path.GetFileName(filePath), BlobCategory.Tools, GetContentType(filePath), ct);
        }

        private BlobContainerClient GetContainerClient(BlobCategory category)
        {
            return category switch
            {
                BlobCategory.Field => _fieldContainerClient,
                _ => _apiaryContainerClient
            };
        }

        private static async Task<byte[]> ReadToEndAsync(Stream stream, CancellationToken ct)
        {
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory, ct);
            return memory.ToArray();
        }

        private static string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
