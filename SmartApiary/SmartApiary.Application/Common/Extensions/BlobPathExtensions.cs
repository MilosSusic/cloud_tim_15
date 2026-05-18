using SmartApiary.Application.Common;

namespace SmartApiary.Application.Common.Extensions
{
    public static class BlobPathExtensions
    {
        public static string GetBlobPath(BlobMetadata metadata)
        {
            var fileName = Path.GetFileName(metadata.FileName);

            return metadata.Category switch
            {
                BlobCategory.Apiary => $"apiary_{Guid.NewGuid()}_{fileName}",
                BlobCategory.Field => $"field_{Guid.NewGuid()}_{fileName}",
                _ => $"tools_{Guid.NewGuid()}_{fileName}"
            };
        }
    }
}
