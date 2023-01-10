using SitefinityWebApp.Custom.Services;
using System.Web.Http;

namespace SitefinityWebApp.WebApiControllers
{
    public class WebApiController : ApiController
    {
        private IHelloSitefinityService service;

        public WebApiController(IHelloSitefinityService service)
        {
            this.service = service;
        }


        [HttpGet]
        public IHttpActionResult Hello()
        {
            string helloString = this.service.SayHello();
            if (string.IsNullOrEmpty(helloString))
            {
                return this.NotFound();
            }
            return this.Ok(helloString);
        }
    }
}