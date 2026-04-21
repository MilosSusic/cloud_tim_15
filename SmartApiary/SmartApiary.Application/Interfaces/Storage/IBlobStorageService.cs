namespace SmartApiary.Application.Interfaces.Storage
{
    public interface IBlobStorageService
    {
        Task<string> UploadStreamAsync(
            Stream content,
            string fileName,
            string contentType,
            CancellationToken ct = default);
    }
}
