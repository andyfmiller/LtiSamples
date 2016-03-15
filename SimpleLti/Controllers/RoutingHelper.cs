using System;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace SimpleLti.Controllers
{
    public static class RoutingHelper
    {
        public static Uri GetLineItemsUri(HttpContextBase httpContext, string contextId, string id = null)
        {
            if (httpContext == null) return null;
            if (string.IsNullOrEmpty(contextId)) return null;

            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            if (routeData == null) return null;
            var requestContext = new RequestContext(httpContext, routeData);

            // Calculate the full URI of the LineItem based on the routes in WebApiConfig
            var lineItemUrl = UrlHelper.GenerateUrl("LineItemsApi", null, "LineItems",
                new RouteValueDictionary
                {
                    { "httproute", string.Empty },
                    { "contextId", contextId },
                    { "id", id }
                },
                RouteTable.Routes, requestContext,
                false);
            Uri uri;
            Uri.TryCreate(httpContext.Request.Url, lineItemUrl, out uri);
            return uri;
        }

        public static Uri GetResultsUri(HttpContextBase httpContext, string contextId, string lineItemId, string id = null)
        {
            if (httpContext == null) return null;
            if (string.IsNullOrEmpty(contextId)) return null;
            if (string.IsNullOrEmpty(lineItemId)) return null;

            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            if (routeData == null) return null;
            var requestContext = new RequestContext(httpContext, routeData);

            // Calculate the full URI of the Result based on the routes in WebApiConfig
            var lineItemUrl = UrlHelper.GenerateUrl("ResultsApi", null, "Results",
                new RouteValueDictionary
                {
                    { "httproute", string.Empty },
                    { "contextId", contextId },
                    { "lineItemId", lineItemId },
                    { "id", id }
                },
                RouteTable.Routes, requestContext,
                false);
            Uri uri;
            Uri.TryCreate(httpContext.Request.Url, lineItemUrl, out uri);
            return uri;
        }
    }
}
