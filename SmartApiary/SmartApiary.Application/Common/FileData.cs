namespace SmartApiary.Application.Common
{
    public record FileData<TMetadata>(
        TMetadata Metadata,
        byte[] Content,
        string? ContentType = null);
}
