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
        public LineItemsController()
        {
            OnDeleteLineItem = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), InMemoryDb.ContextId, context.Id);

                if (InMemoryDb.LineItem == null || !InMemoryDb.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    InMemoryDb.LineItem = null;
                    InMemoryDb.Result = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItem = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.Id);

                if (InMemoryDb.LineItem == null || !InMemoryDb.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem = InMemoryDb.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetLineItemWithResults = context =>
            {
                OnGetLineItem(context);
                if (context.LineItem != null && InMemoryDb.Result != null)
                {
                    context.LineItem.Result = InMemoryDb.Result == null ? new LisResult[] {} : new[] {InMemoryDb.Result};
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

                if (InMemoryDb.LineItem != null &&
                    (string.IsNullOrEmpty(context.ActivityId) ||
                     context.ActivityId.Equals(InMemoryDb.LineItem.AssignedActivity.ActivityId)))
                {
                    context.LineItemContainerPage.LineItemContainer.MembershipSubject.LineItems = new[] {InMemoryDb.LineItem};
                }
                context.StatusCode = HttpStatusCode.OK;
                return Task.FromResult<object>(null);
            };

            // Create a LineItem
            OnPostLineItem = context =>
            {
                if (InMemoryDb.LineItem != null)
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                    return Task.FromResult<object>(null);
                }

                // Normally LineItem.Id would be calculated based on an id assigned by the database
                context.LineItem.Id = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, InMemoryDb.LineItemId); 
                context.LineItem.Results = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, InMemoryDb.LineItemId);
                InMemoryDb.LineItem = context.LineItem;
                context.StatusCode = HttpStatusCode.Created;
                return Task.FromResult<object>(null);
            };

            // Update LineItem (but not results)
            OnPutLineItem = context =>
            {
                if (context.LineItem == null || InMemoryDb.LineItem == null || !InMemoryDb.LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.LineItem.Result = InMemoryDb.LineItem.Result;
                    InMemoryDb.LineItem = context.LineItem;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            // Update LineItem and Result
            OnPutLineItemWithResults = context =>
            {
                if (context.LineItem == null || InMemoryDb.LineItem == null || !InMemoryDb.LineItem.Id.Equals(context.LineItem.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    InMemoryDb.LineItem = context.LineItem;
                    if (context.LineItem.Result != null && context.LineItem.Result.Length > 0)
                    {
                        InMemoryDb.Result = context.LineItem.Result[0];
                    }
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
