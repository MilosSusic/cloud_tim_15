using SmartApiary.Application.Common;

namespace SmartApiary.Application.Interfaces.Storage
{
    public interface IBlobStorageService : IFileStorage<BlobMetadata>
    {
        Task<string> UploadStreamAsync(
            Stream content,
            string fileName,
            BlobCategory category,
            string contentType,
            CancellationToken ct = default);

        string GetUrl(BlobMetadata metadata);
    }
}
