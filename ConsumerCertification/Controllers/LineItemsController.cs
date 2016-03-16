using System.Net;
using System.Threading.Tasks;
using System.Web;
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
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), ContextId, context.Id);

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
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.Id);

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
                        Id = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId),
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
                context.LineItem.Id = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, LineItemId); 
                context.LineItem.Results = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, LineItemId);
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
    }
}
