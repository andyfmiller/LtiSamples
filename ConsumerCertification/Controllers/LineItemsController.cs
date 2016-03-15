using System;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Outcomes.v2;

namespace ConsumerCertification.Controllers
{
    public class LineItemsController : LineItemsControllerBase
    {
        // Simple "database" of lineitems for demonstration purposes
        public const string ContextId = "course-1";
        public const string LineItemId = "lineitem-1";
        private static LineItem _lineItem;

        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                var lineItemUri = GetLineItemsUri(context.ContextId, context.Id);

                if (lineItemUri == null || _lineItem == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                var lineItemUri = GetLineItemsUri(context.ContextId, context.Id);

                if (lineItemUri == null || _lineItem == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem = _lineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItems = context =>
            {
                if (_lineItem == null ||
                    (!string.IsNullOrEmpty(context.ActivityId) &&
                     !context.ActivityId.Equals(_lineItem.AssignedActivity.ActivityId)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItemContainerPage = new LineItemContainerPage
                    {
                        ExternalContextId = LtiConstants.LineItemContainerContextId,
                        Id = GetLineItemsUri(context.ContextId),
                        LineItemContainer = new LineItemContainer
                        {
                            LineItemMembershipSubject = new LineItemMembershipSubject
                            {
                                ContextId = context.ContextId,
                                LineItems = new[] { _lineItem }
                            }
                        }
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            // Create a LineItem
            OnPostLineItem = context =>
            {
                if (_lineItem != null)
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                    return Task.FromResult<object>(null);
                }

                // Normally LineItem.Id would be calculated based on an id assigned by the database
                context.LineItem.Id = GetLineItemsUri(context.ContextId, LineItemId); 
                context.LineItem.Results = GetLineItemResultsUri(context.ContextId, LineItemId);
                _lineItem = context.LineItem;
                context.StatusCode = HttpStatusCode.Created;
                return Task.FromResult<object>(null);
            };

            OnPutLineItem = context =>
            {
                if (context.LineItem == null || _lineItem == null || !_lineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _lineItem = context.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }

        public Uri GetLineItemsUri(string contextId, string id = null)
        {
            if (string.IsNullOrEmpty(contextId)) return null;

            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(httpContextWrapper);
            if (routeData == null) return null;
            var requestContext = new RequestContext(httpContextWrapper, routeData);
         
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
            Uri.TryCreate(httpContextWrapper.Request.Url, lineItemUrl, out uri);
            return uri;
        }

        private static Uri GetLineItemResultsUri(string contextId, string id)
        {
            using (var controller = new ResultsController())
            {
                return controller.GetResultsUri(contextId, id);
            }
        }
    }
}
