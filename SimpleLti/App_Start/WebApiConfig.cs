using System.Web.Http;

namespace SimpleLti
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "ToolConsumerProfileApi",
                routeTemplate: "profiles/{controller}");

            config.Routes.MapHttpRoute(
                name: "LineItemsApi",
                routeTemplate: "courses/{contextId}/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute(
                name: "ResultsApi",
                routeTemplate: "courses/{contextId}/lineitems/{itemId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
        }
    }
}
