using System.IO;
using System.Linq;
using System.Web.Http;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.Libraries;

namespace SitefinityWebApp.Api
{
    
    public class AuthDownloadController : ApiController
    {
        [HttpGet]
        [Authorize]
        [Route("api/with-auth/getdoc")]
        public IHttpActionResult GetDoc()
        {
            var librariesManager = LibrariesManager.GetManager();
            var document = librariesManager.GetDocuments().Where(d => !d.BlobStorageProvider.Contains("Azure") && d.Status == ContentLifecycleStatus.Live && d.Visible).FirstOrDefault();

            Stream stream = librariesManager.Download(document);

            return new DocumentResult(stream, document.MimeType, document.Extension, document.UrlName);
        }

        [HttpGet]
        [Authorize]
        [Route("api/with-auth/getdocazure")]
        public IHttpActionResult GetDocAzure()
        {
            var librariesManager = LibrariesManager.GetManager();
            var document = librariesManager.GetDocuments().Where(d => d.BlobStorageProvider.Contains("Azure") && d.Status == ContentLifecycleStatus.Live && d.Visible).FirstOrDefault();

            Stream stream = librariesManager.Download(document);

            return new DocumentResult(stream, document.MimeType, document.Extension, document.UrlName);
        }
    }
}
