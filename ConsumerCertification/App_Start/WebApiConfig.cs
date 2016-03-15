using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ConsumerCertification
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
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
                defaults: new { id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                name: "ResultsApi",
                routeTemplate: "courses/{contextId}/lineitems/{lineItemId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

        }
    }
}
