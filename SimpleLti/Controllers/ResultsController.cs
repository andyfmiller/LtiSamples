using System;
using System.Net;
using System.Threading.Tasks;
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
                if (string.IsNullOrEmpty(context.Id) || _result == null || !_result.Id.Equals(new Uri(context.Id)))
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
                if (string.IsNullOrEmpty(context.Id) || _result == null || !_result.Id.Equals(new Uri(context.Id)))
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
                if (_result == null ||
                    (!string.IsNullOrEmpty(context.LineItemId) &&
                     !context.LineItemId.Equals(LineItemsController.LineItemId)))
                {
                    context.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    var id = new UriBuilder(Request.RequestUri) { Query = "firstPage" };
                    context.ResultContainerPage = new ResultContainerPage
                    {
                        Id = id.Uri,
                        ResultContainer = new ResultContainer
                        {
                            MembershipSubject = new Context
                            {
                                ContextId = LineItemsController.LineItemId,
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
