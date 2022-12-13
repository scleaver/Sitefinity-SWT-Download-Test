using Microsoft.Owin.Extensions;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Authentication;
using Telerik.Sitefinity.Owin;

namespace SitefinityWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthenticationModule.Initialized += this.AuthenticationModule_Initialized;

            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use<SitefinityMiddlewareCustom>(app);
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private void AuthenticationModule_Initialized(object sender, EventArgs e)
        {
            var middlewares = typeof(SitefinityMiddlewareFactory).GetField("middlewareInitParamsStage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(SitefinityMiddlewareFactory.Current) as List<Tuple<Type, object[], PipelineStage>>;
            var sitefinityAuthenticationMiddleware = middlewares.Find(m => m.Item1.FullName == "Telerik.Sitefinity.Authentication.Owin.SitefinityAuthenticationMiddleware");
            middlewares.Remove(sitefinityAuthenticationMiddleware);

            SitefinityMiddlewareFactory.Current.AddIfNotPresentMiddleware<SitefinityAuthenticationMiddlewareCustom>(stage: PipelineStage.Authenticate);


            // Solve 302 redirect when not authenticated
            var ignorePathsField = typeof(AuthenticationModule).GetField("ignorePaths", BindingFlags.NonPublic | BindingFlags.Static);
            var ignorePaths = ignorePathsField.GetValue(null) as string[];
            var newIgnorePaths = ignorePaths.ToList();
            newIgnorePaths.Add(VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(HostingEnvironment.ApplicationVirtualPath), "api/")); // TODO: Change path here if needed
            ignorePathsField.SetValue(null, newIgnorePaths.ToArray());
        }
    }
}