using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartApiary.Functions.Processing
{
    public class UploadApiaryImage
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public UploadApiaryImage(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<UploadApiaryImage>();
        }

        [Function("UploadApiaryImage")]
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

            var connectionString = _configuration["AzureBlob:ConnectionString"];
            var containerName = _configuration["AzureBlob:Container"];
            var blobClient = new BlobContainerClient(connectionString, containerName);
            await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var urls = new List<string>();
            foreach (var file in parser.Files)
            {
                var fileName = $"apiary-images/{Guid.NewGuid()}_{file.FileName}";
                var blob = blobClient.GetBlobClient(fileName);
                await blob.UploadAsync(file.Data, new BlobHttpHeaders { ContentType = file.ContentType ?? "application/octet-stream" });
                urls.Add(blob.Uri.ToString());
            }

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteAsJsonAsync(new { urls });
            return resp;
        }
    }
}
