using System.Net;
using System.Threading.Tasks;
using System.Web;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Outcomes.v2;

namespace ConsumerCertification.Controllers
{
    [OAuthHeaderAuthentication]
    public class LineItemsController : LineItemsControllerBase
    {
        // Simple "database" of lineitems for demonstration purposes
        public const string ContextId = "course-1";
        public const string LineItemId = "lineitem-1";
        public const string ResultId = "result-1";
        public static LineItem LineItem;
        public static LisResult Result;

        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), ContextId, context.Id);

                if (LineItem == null || !LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    LineItem = null;
                    Result = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.Id);

                if (LineItem == null || !LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem = LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItemWithResults = context =>
            {
                OnGetLineItem(context);
                if (context.LineItem != null && Result != null)
                {
                    context.LineItem.Result = Result == null ? new LisResult[] {} : new[] {Result};
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItems = context =>
            {
                context.LineItemContainerPage = new LineItemContainerPage
                {
                    ExternalContextId = LtiConstants.LineItemContainerContextId,
                    Id = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId),
                    LineItemContainer = new LineItemContainer
                    {
                        MembershipSubject = new LineItemMembershipSubject
                        {
                            ContextId = context.ContextId,
                            LineItems = new LineItem[] { }
                        }
                    }
                };

                if (LineItem != null &&
                    (string.IsNullOrEmpty(context.ActivityId) ||
                     context.ActivityId.Equals(LineItem.AssignedActivity.ActivityId)))
                {
                    context.LineItemContainerPage.LineItemContainer.MembershipSubject.LineItems = new[] {LineItem};
                }
                context.StatusCode = HttpStatusCode.OK;
                return Task.FromResult<object>(null);
            };

            // Create a LineItem
            OnPostLineItem = context =>
            {
                if (LineItem != null)
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                    return Task.FromResult<object>(null);
                }

                // Normally LineItem.Id would be calculated based on an id assigned by the database
                context.LineItem.Id = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, LineItemId); 
                context.LineItem.Results = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, LineItemId);
                LineItem = context.LineItem;
                context.StatusCode = HttpStatusCode.Created;
                return Task.FromResult<object>(null);
            };

            // Update LineItem (but not results)
            OnPutLineItem = context =>
            {
                if (context.LineItem == null || LineItem == null || !LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem.Result = LineItem.Result;
                    LineItem = context.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = context =>
            {
                if (context.LineItem == null || LineItem == null || !LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    LineItem = context.LineItem;
                    if (context.LineItem.Result != null && context.LineItem.Result.Length > 0)
                    {
                        Result = context.LineItem.Result[0];
                    }
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
