using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using SmartApiary.Application.Common;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Functions.Processing
{
    public class UploadImage
    {
        private readonly IBlobStorageService _blobStorage;
        private readonly ILogger _logger;

        public UploadImage(IBlobStorageService blobStorage, ILoggerFactory loggerFactory)
        {
            _blobStorage = blobStorage;
            _logger = loggerFactory.CreateLogger<UploadImage>();
        }

        [Function("UploadImage")]
        public async Task<HttpResponseData> Run([
            HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload-image")
        ] HttpRequestData req)
        {
            var parser = await MultipartFormDataParser.ParseAsync(req.Body);
            if (parser.Files == null || !parser.Files.Any())
            {
                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteStringAsync("No file uploaded.");
                return badResp;
            }

            var category = ResolveCategory(req);

            var urls = new List<string>();
            foreach (var file in parser.Files)
            {
                var url = await _blobStorage.UploadStreamAsync(
                    file.Data,
                    file.FileName,
                    category,
                    file.ContentType ?? "application/octet-stream");

                urls.Add(url);
            }

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(new { urls });
            return resp;
        }

        private static BlobCategory ResolveCategory(HttpRequestData req)
        {
            var query = QueryHelpers.ParseQuery(req.Url.Query);
            if (query.TryGetValue("category", out var categoryValue)
                && Enum.TryParse<BlobCategory>(categoryValue.ToString(), true, out var parsed))
            {
                return parsed;
            }

            return BlobCategory.Apiary;
        }
    }
}

