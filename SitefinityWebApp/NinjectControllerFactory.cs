using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace SitefinityWebApp
{
    public class NinjectControllerFactory : FrontendControllerFactory
    {
        public NinjectControllerFactory()
        {
            this.ninjectKernel = Global.NinjectKernel;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }

            var resolvedController = this.ninjectKernel.Get(controllerType);
            IController controller = resolvedController as IController;

            return controller;
        }

        private readonly IKernel ninjectKernel;
    }
}