using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Outcomes.v2;

namespace ConsumerCertification.Controllers
{
    public class ResultsController : ResultsControllerBase
    {
        // Simple "database" of results for demonstration purposes
        public const string ResultId = "result-1";
        private static LisResult _result;

        public ResultsController()
        {
            OnDeleteResult = context =>
            {
                var resultUri = GetResultsUri(context.ContextId, context.LineItemId, context.Id);

                if (resultUri == null || _result == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _result = null;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetResult = context =>
            {
                var resultUri = GetResultsUri(context.ContextId, context.LineItemId, context.Id);

                if (resultUri == null || _result == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.Result = _result;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnGetResults = context =>
            {
                if (_result == null)
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.ResultContainerPage = new ResultContainerPage
                    {
                        Id = GetResultsUri(context.ContextId, context.LineItemId),
                        ResultContainer = new ResultContainer
                        {
                            MembershipSubject = new ResultMembershipSubject
                            {
                                Results = new[] { _result }
                            }
                        }
                    };
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };

            OnPutResult = context =>
            {
                if (context.Result == null || _result == null || !_result.Id.Equals(context.Result.Id))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    _result = context.Result;
                    context.StatusCode = HttpStatusCode.OK;
                }
                return Task.FromResult<object>(null);
            };
        }

        public Uri GetResultsUri(string contextId, string lineItemId, string id = null)
        {
            if (string.IsNullOrEmpty(contextId)) return null;
            if (string.IsNullOrEmpty(lineItemId)) return null;

            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            var routeData = RouteTable.Routes.GetRouteData(httpContextWrapper);
            if (routeData == null) return null;
            var requestContext = new RequestContext(httpContextWrapper, routeData);

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
            Uri.TryCreate(httpContextWrapper.Request.Url, lineItemUrl, out uri);
            return uri;
        }

    }
}
