using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Extensions;
using Owin;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Owin;
using Telerik.Sitefinity.Utilities.TypeConverters;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SitefinityWebApp
{
    public class SitefinityMiddlewareCustom : OwinMiddleware
    {
        public SitefinityMiddlewareCustom(OwinMiddleware next, IAppBuilder app) : base(next)
        {
            this.sitefinityMiddlewarePipeline = this.BuildSitefinityPipeline(app);
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (Bootstrapper.IsReady)
            {
                await this.sitefinityMiddlewarePipeline.Value(context.Environment);
                return;
            }

            await this.Next.Invoke(context);
        }

        private static AppFunc BuildSitefinityCustomPipeline(IAppBuilder app, OwinMiddleware next)
        {
            var newApp = app.New();

            TypeResolutionService.ResolveType("Telerik.Sitefinity.Owin.SitefinityMiddleware").GetMethod("UseSitefinityCoreMiddlewares", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { newApp });
            var middlewareInitParamsStage = typeof(SitefinityMiddlewareFactory).GetProperty("MiddlewareInitParamsStage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(SitefinityMiddlewareFactory.Current) as IEnumerable<Tuple<Type, object[], PipelineStage>>;
            foreach (var middlewareStage in middlewareInitParamsStage)
            {
                var middlewareType = middlewareStage.Item1;
                var middlewareArgs = middlewareStage.Item2;
                var stage = middlewareStage.Item3;

                var newArgs = new List<object>();

                if (middlewareType.IsSubclassOf(TypeResolutionService.ResolveType("Telerik.Sitefinity.Owin.DynamicBranchMiddleware")) || middlewareType == typeof(SitefinityAuthenticationMiddlewareCustom))
                {
                    newArgs.Add(newApp);
                }

                if (middlewareArgs != null)
                    newArgs.AddRange(middlewareArgs);

                if (newArgs.Count() > 0)
                    newApp.Use(middlewareType, newArgs.ToArray());
                else
                    newApp.Use(middlewareType);

                newApp.UseStageMarker(stage);
            }

            newApp.Run(ctx => next.Invoke(ctx));
            return newApp.Build();
        }

        private Lazy<AppFunc> BuildSitefinityPipeline(IAppBuilder app)
        {
            return new Lazy<AppFunc>(() => BuildSitefinityCustomPipeline(app, this.Next), true);
        }

        private readonly Lazy<AppFunc> sitefinityMiddlewarePipeline;
    }
}