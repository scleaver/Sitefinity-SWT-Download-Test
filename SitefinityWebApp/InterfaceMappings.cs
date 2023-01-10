using Ninject.Modules;
using SitefinityWebApp.Custom.Services;

namespace SitefinityWebApp
{
    public class InterfaceMappings : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IHelloSitefinityService>().To<HelloSitefinityService>();
        }
    }
}