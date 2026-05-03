using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Storage;

namespace SmartApiary.Functions.Processing
{
    public class UploadFieldImage
    {
        private readonly IFieldBlobStorageService _blobStorage;
        private readonly ILogger _logger;

        public UploadFieldImage(IFieldBlobStorageService blobStorage, ILoggerFactory loggerFactory)
        {
            _blobStorage = blobStorage;
            _logger = loggerFactory.CreateLogger<UploadFieldImage>();
        }

        [Function("UploadFieldImage")]
        public async Task<HttpResponseData> Run([
            HttpTrigger(AuthorizationLevel.Function, "post", Route = "upload-field-image")
        ] HttpRequestData req)
        {
            var parser = await MultipartFormDataParser.ParseAsync(req.Body);
            if (parser.Files == null || !parser.Files.Any())
            {
                var badResp = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResp.WriteStringAsync("No file uploaded.");
                return badResp;
            }

            var urls = new List<string>();
            foreach (var file in parser.Files)
            {
                var url = await _blobStorage.UploadStreamAsync(
                    file.Data,
                    file.FileName,
                    file.ContentType ?? "application/octet-stream");
                urls.Add(url);
            }

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(new { urls });
            return resp;
        }
    }
}
