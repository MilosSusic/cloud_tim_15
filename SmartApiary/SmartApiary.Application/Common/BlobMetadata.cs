namespace SmartApiary.Application.Common
{
    public enum BlobCategory
    {
        Apiary,
        Field,
        Tools
    }

    public sealed record BlobMetadata(BlobCategory Category, string FileName);
}
