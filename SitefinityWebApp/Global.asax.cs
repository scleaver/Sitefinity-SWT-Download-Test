using System;
using System.Net.Http.Headers;
using System.Web.Http;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Initialized += new EventHandler<ExecutedEventArgs>(this.Bootstrapper_Initialized);
        }

        private void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                if (Bootstrapper.IsReady)
                {
                    GlobalConfiguration.Configure(Register);
                }
            }
        }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnsureInitialized();

            config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}/{action}",
                    defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }
                );
        }
    }
}