using System.Web.Http;
using LtiLibrary.Owin.Security.Lti;

namespace Consumer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Use XmlSerializer instead of DataContractSerializer
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
#if DEBUG
            config.Formatters.XmlFormatter.Indent = true;
#endif
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Enable OWIN authentication for Outcomes API
            // See http://www.asp.net/web-api/overview/security/individual-accounts-in-web-api
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(LtiAuthenticationDefaults.AuthenticationType));
        }
    }
}
