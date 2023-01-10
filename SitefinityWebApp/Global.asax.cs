using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.WebApi;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;

namespace SitefinityWebApp
{
    public class Global : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            Telerik.Sitefinity.Abstractions.Bootstrapper.Bootstrapped += this.Bootstrapper_Bootstrapped;
        }

        protected override IKernel CreateKernel()
        {
            IKernel kernel = new StandardKernel();
            NinjectKernel = kernel;
            return kernel;
        }

        public static IKernel NinjectKernel { get; private set; }

        protected void Bootstrapper_Bootstrapped(object sender, EventArgs e)
        {
            // Account for constructor injection in Controller types
            // This controller factory initialization accounts for both MVC (Feather) widgets, and Classic MVC Mode Controllers
            ObjectFactory.Container.RegisterType<ISitefinityControllerFactory, NinjectControllerFactory>(new ContainerControlledLifetimeManager());
            ISitefinityControllerFactory factory = ObjectFactory.Resolve<ISitefinityControllerFactory>();
            ControllerBuilder.Current.SetControllerFactory(factory);

            // Account for constructor injection in ApiController types
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(NinjectKernel);

            this.RegisterWebApiRoute();
            this.RegisterClassicMvcModeRoute();
        }

        /// <summary>
        /// Enables WebApi calls
        /// </summary>
        /// <remarks>
        /// This registration does not depend on Ninject and does not account for constructor injection
        /// </remarks>
        private void RegisterWebApiRoute()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                "DefaultApi",
                "webapi/{controller}/{id}",
                new { id = RouteParameter.Optional });
        }

        /// <summary>
        /// Enables Classic MVC Mode
        /// </summary>
        /// <remarks>
        /// This registration does not depend on Ninject and does not account for constructor injection
        /// </remarks>
        private void RegisterClassicMvcModeRoute()
        {
            RouteTable.Routes.MapRoute(
                "Classic",
                "classic/{controller}/{action}/{id}",
                new { controller = "Feature", action = "Index", id = RouteParameter.Optional });
        }
    }
}