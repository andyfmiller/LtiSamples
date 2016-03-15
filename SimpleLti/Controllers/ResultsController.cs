using System.Net;
using System.Threading.Tasks;
using System.Web;
using LtiLibrary.AspNet.Outcomes.v2;
using LtiLibrary.Core.Outcomes.v2;

namespace SimpleLti.Controllers
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
                var resultUri = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId, context.Id);

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
                var resultUri = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId, context.Id);

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
                        Id = RoutingHelper.GetResultsUri(new HttpContextWrapper(HttpContext.Current), context.ContextId, context.LineItemId),
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
    }
}
