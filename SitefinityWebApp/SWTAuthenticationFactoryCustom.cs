using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Owin;
using System;
using System.Reflection.Emit;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Utilities.TypeConverters;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SitefinityWebApp
{
    public class SWTAuthenticationFactoryCustom
    {
        public virtual AppFunc BuildAuthPipeline(IAppBuilder app, OwinMiddleware next)
        {
            var claimsModule = ClaimsManager.CurrentAuthenticationModule;
            var newApp = app.New();

            newApp.Use(typeof(CookieSaverMiddlewareCustom));

            var sitefinityCookieAuthenticationOptionsType = TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.SitefinityAuthentication.SitefinityCookieAuthenticationOptions");
            var sitefinityAuthenticationExtensionsType = TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.SitefinityAuthenticationExtensions");
            var sitefinityCookieAuthenticationOptions = Activator.CreateInstance(sitefinityCookieAuthenticationOptionsType, new object[] { claimsModule.RPAuthenticationType });
            var useSitefinityCookieAuthenticationMethod = sitefinityAuthenticationExtensionsType.GetMethod("UseSitefinityCookieAuthentication");
            useSitefinityCookieAuthenticationMethod.Invoke(null, new object[] { newApp, sitefinityCookieAuthenticationOptions });

            var useSitefinityBasicAuthenticationMethod = sitefinityAuthenticationExtensionsType.GetMethod("UseSitefinityBasicAuthentication");
            useSitefinityBasicAuthenticationMethod.Invoke(null, new object[] { newApp });

            var swtAuthenticationOptionsType = TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.SWT.SWTAuthenticationOptions");
            var swtAuthenticationOptions = Activator.CreateInstance(swtAuthenticationOptionsType, new object[] { claimsModule.STSAuthenticationType });
            swtAuthenticationOptionsType.GetProperty("SignInAsAuthenticationType").SetValue(swtAuthenticationOptions, claimsModule.RPAuthenticationType);
            var useSWTAuthenticationMethod = TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.SWT.SWTAuthenticationExtensions").GetMethod("UseSWTAuthentication");
            useSWTAuthenticationMethod.Invoke(null, new object[] { newApp, swtAuthenticationOptions });

            var useUserValidation = sitefinityAuthenticationExtensionsType.GetMethod("UseUserValidation");
            useUserValidation.Invoke(null, new object[] { newApp });

            var useRefererValidation = sitefinityAuthenticationExtensionsType.GetMethod("UseRefererValidation");
            useRefererValidation.Invoke(null, new object[] { newApp });

            newApp.Run(ctx => next.Invoke(ctx));
            return newApp.Build();
        }
    }
}