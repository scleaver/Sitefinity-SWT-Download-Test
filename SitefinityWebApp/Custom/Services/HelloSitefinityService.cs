namespace SitefinityWebApp.Custom.Services
{
    public class HelloSitefinityService : IHelloSitefinityService
    {
        public string SayHello()
        {
            return "Hello, Sitefinity!";
        }
    }
}