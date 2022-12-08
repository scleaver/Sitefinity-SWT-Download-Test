using ServiceStack;
using System;
using System.Configuration.Internal;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SitefinityWebApp
{
    public class DocumentResult : IHttpActionResult
    {
        private readonly string mimeType;
        private readonly string fileName;
        private readonly Stream docStream;

        public DocumentResult(Stream stream, string mimeType, string extension, string urlName)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrEmpty(mimeType))
                throw new ArgumentException(nameof(mimeType));

            fileName = string.Concat(urlName, extension);
            this.docStream = stream;
            this.mimeType = mimeType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                docStream.Position = 0;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(docStream)
                };

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                response.Content.Headers.ContentLength = docStream.Length;

                return response;
            }, cancellationToken);
        }
    }
}