using System.Net;
using System.Threading.Tasks;
using System.Web;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Outcomes.v2;

namespace SimpleLti.Controllers
{
    [OAuthHeaderAuthentication]
    public class ResultsController : ResultsControllerBase
    {
        public ResultsController()
        {
            OnDeleteResult = context =>
            {
                var resultUri = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId, context.Id);

                if (resultUri == null || LineItemsController.Result == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    LineItemsController.Result = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetResult = context =>
            {
                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a PUT or a GET request.

                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId);
                var resultUri = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId, context.Id);

                if (LineItemsController.LineItem == null || !LineItemsController.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else if (LineItemsController.Result == null || !LineItemsController.Result.Id.Equals(resultUri))
                {
                    context.Result = new LisResult
                    {
                        ExternalContextId = LtiConstants.ResultContextId,
                        Id = resultUri,
                        ResultOf = lineItemUri,
                        ResultStatus = ResultStatus.Initialized
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    context.Result = LineItemsController.Result;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetResults = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current),
                    context.ContextId, context.LineItemId);
                if (LineItemsController.LineItem == null || !LineItemsController.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.ResultContainerPage = new ResultContainerPage
                    {
                        ExternalContextId = LtiConstants.ResultContainerContextId,
                        Id = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId),
                        ResultContainer = new ResultContainer
                        {
                            MembershipSubject = new ResultMembershipSubject
                            {
                                Results = LineItemsController.Result == null ? new LisResult[] {} : new[] { LineItemsController.Result }
                            }
                        }
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnPostResult = context =>
            {
                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current),
                    context.ContextId, context.LineItemId);
                if (LineItemsController.LineItem == null || !LineItemsController.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    LineItemsController.Result = context.Result;
                    LineItemsController.Result.Id = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current),
                        context.ContextId, context.LineItemId, LineItemsController.ResultId);
                    context.Result = LineItemsController.Result;
                    context.StatusCode = HttpStatusCode.Created;
                }
                return Task.FromResult<object>(null);
            };

            OnPutResult = context =>
            {
                // https://www.imsglobal.org/specs/ltiomv2p0/specification-3
                // When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.  
                // Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a 
                // PUT or a GET request.

                var lineItemUri = RoutingHelper.GetLineItemsUri(new HttpContextWrapper(HttpContext.Current),
                    context.ContextId, context.LineItemId);
                if (LineItemsController.LineItem == null || !LineItemsController.LineItem.Id.Equals(lineItemUri))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    // If this is the first connection, the PUT is equivalent to a POST
                    LineItemsController.Result = context.Result;
                    if (context.Result.Id == null)
                    {
                        LineItemsController.Result.Id =
                            RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current),
                                context.ContextId, context.LineItemId, LineItemsController.ResultId);
                    }
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }
    }
}
