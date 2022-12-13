using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Security.Configuration;
using Telerik.Sitefinity.Utilities.TypeConverters;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SitefinityWebApp
{
    public class SitefinityAuthenticationMiddlewareCustom : OwinMiddleware
    {
        public SitefinityAuthenticationMiddlewareCustom(OwinMiddleware next, IAppBuilder app)
            : base(next)
        {
            this.authenticationPipeline = this.BuildClaimsAuthenticationPipeline(app, this.Next);
        }

        private AppFunc BuildClaimsAuthenticationPipeline(IAppBuilder app, OwinMiddleware next)
        {
            var newApp = app.New();

            TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.SitefinityAuthenticationExtensions").GetMethod("UseSecurityTokenService").Invoke(null, new object[] { newApp });
            newApp.Use(TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.AccessControlMiddleware"));
            newApp.Use(TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.AnonymousUserMiddleware"));

            newApp.Use(typeof(RelyingPartyMiddlewareCustom), app);
            newApp.Run(ctx => next.Invoke(ctx));

            return newApp.Build();
        }

        public override async Task Invoke(IOwinContext context)
        {
            var isSystemReady = Bootstrapper.IsReady;
            if (isSystemReady && Config.Get<SecurityConfig>().AuthenticationMode == AuthenticationMode.Claims)
            {
                await this.authenticationPipeline.Invoke(context.Environment);
                return;
            }

            // When the application is not ready yet or Forms authentication is used, skip the claims authentication pipeline.
            await this.Next.Invoke(context);
        }

        private readonly AppFunc authenticationPipeline;
    }
}