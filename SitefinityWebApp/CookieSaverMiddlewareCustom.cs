using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Telerik.Sitefinity.Owin.Extensions;
using Telerik.Sitefinity.Utilities.TypeConverters;

namespace SitefinityWebApp
{
    public class CookieSaverMiddlewareCustom : OwinMiddleware
    {
        public CookieSaverMiddlewareCustom(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            var aspHttpContext = context.GetHttpContext();
            var owinContextCookieHeaders = context.Response.Headers.GetValues(SetCookieHeaderKey);
            if (aspHttpContext != null && owinContextCookieHeaders != null)
            {
                var owinContextCookies = ParseCookieMethod.Invoke(null, new object[] { owinContextCookieHeaders }) as IEnumerable<HttpCookie>;
                foreach (var owinContextCookie in owinContextCookies)
                {
                    if (!aspHttpContext.Response.Cookies.AllKeys.Contains(owinContextCookie.Name) && !aspHttpContext.Response.HeadersWritten)
                    {
                        MarkCookieAsSetFromNativeModule(owinContextCookie);
                        aspHttpContext.Response.Cookies.Add(owinContextCookie);
                    }
                }
            }
        }

        private static void MarkCookieAsSetFromNativeModule(HttpCookie owinCookie)
        {
            if (FromHeaderProperty != null)
            {
                FromHeaderProperty.SetValue(owinCookie, true);
            }
            else if (IsInResponseHeaderProperty != null)
            {
                IsInResponseHeaderProperty.SetValue(owinCookie, true);
            }
        }

        private const string SetCookieHeaderKey = "Set-Cookie";
        public static readonly MethodInfo ParseCookieMethod = TypeResolutionService.ResolveType("Telerik.Sitefinity.Authentication.Owin.Cookies.CookieParser").GetMethod("Parse", new Type[] { typeof(IList<string>) });
        public static readonly PropertyInfo FromHeaderProperty = typeof(HttpCookie).GetProperty("FromHeader", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly PropertyInfo IsInResponseHeaderProperty = typeof(HttpCookie).GetProperty("IsInResponseHeader", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}