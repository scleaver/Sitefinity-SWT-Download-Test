using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SitefinityWebApp
{
    public class RelyingPartyMiddlewareCustom : OwinMiddleware
    {
        public RelyingPartyMiddlewareCustom(OwinMiddleware next, IAppBuilder app)
            : base(next)
        {
            this.authenticationPipeline = this.BuildAuthPipeline(app);
        }

        public override async Task Invoke(IOwinContext context)
        {
            await this.authenticationPipeline.Value(context.Environment);
            return;

            await this.Next.Invoke(context);
        }

        private Lazy<AppFunc> BuildAuthPipeline(IAppBuilder app)
        {
            return new Lazy<AppFunc>(
                () => new SWTAuthenticationFactoryCustom().BuildAuthPipeline(app, this.Next),
                true);
        }

        private readonly Lazy<AppFunc> authenticationPipeline;
    }
}