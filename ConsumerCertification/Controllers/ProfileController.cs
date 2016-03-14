using LtiLibrary.AspNet.Profiles;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Lti2;
using LtiLibrary.Core.Profiles;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ConsumerCertification.Controllers
{
    /// <summary>
    /// This is a sample ApiController that returns a minimal ToolConsumerProfile.
    /// </summary>
    public class ProfileController : ToolConsumerProfileControllerBase
    {
        public ProfileController()
        {
            OnGetToolConsumerProfile = context =>
            {
                var guid = Assembly.GetExecutingAssembly().GetName().Name;
                var code = "LtiLibrary";
                var vendorName = "andyfmiller.com";
                var productName = "LtiLibrary";
                var productVersion = "1";

                // Build a minimal ToolConsumerProfile for LTI 1.2
                var profile = new ToolConsumerProfile
                {
                    Id = Request.RequestUri,
                    CapabilityOffered = new[] 
                    {
                        LtiConstants.BasicLaunchLtiMessageType,
                        "User.id",
                        "User.username",
                        "CourseSection.sourcedId",
                        "Person.sourcedId",
                        "Person.email.primary",
                        "Person.name.given",
                        "Person.name.family",
                        "Person.name.full",
                        "Membership.role",
                        "LineItem.url",
                        "LineItems.url",
                        "Result.url",
                        "Results.url"
                    },
                    Guid = guid,
                    LtiVersion = context.LtiVersion,
                    ProductInstance = new ProductInstance
                    {
                        Guid = guid,
                        ProductInfo = new ProductInfo
                        {
                            ProductFamily = new ProductFamily
                            {
                                Code = code,
                                Vendor = new Vendor
                                {
                                    Code = code,
                                    Timestamp = DateTime.UtcNow,
                                    VendorName = new VendorName(vendorName)
                                }
                            },
                            ProductName = new ProductName(productName),
                            ProductVersion = productVersion
                        },
                        ServiceOwner = new ServiceOwner
                        {
                            Name = new ServiceOwnerName(vendorName),
                            Description = new ServiceOwnerDescription(Assembly.GetExecutingAssembly().FullName),
                            Support = new Contact ("support@andyfmiller.com")
                        }
                    }
                };

                // Outcomes 1 Service
                var outcomesUrl = UrlHelper.GenerateUrl("DefaultApi", null, "Outcomes",
                    new RouteValueDictionary {{"httproute", string.Empty}}, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                Uri outcomesServiceUri;

                // Outcomes 2 LineItems service
                var lineitemsUrl = UrlHelper.GenerateUrl("DefaultApi", null, "LineItems",
                    new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                Uri lineitemsServiceUri;

                // Outcomes 2 Results service
                var resultsUrl = UrlHelper.GenerateUrl("DefaultApi", null, "Results",
                    new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                Uri resultsServiceUri;

                profile.ServiceOffered = new[]
                {
                    new RestService
                    {
                        Action = new[] {"POST"},
                        EndPoint = Uri.TryCreate(Request.RequestUri, outcomesUrl, out outcomesServiceUri) ? outcomesServiceUri : null,
                        Format = new[] {LtiConstants.OutcomeMediaType}
                    },
                    new RestService
                    {
                        Id = new Uri("tcp:LineItem.collection"),
                        Action = new[] {"GET", "POST"},
                        EndPoint = Uri.TryCreate(Request.RequestUri, lineitemsUrl, out lineitemsServiceUri) ? lineitemsServiceUri : null,
                        Format = new[] {LtiConstants.LisResultContainerMediaType}
                    },
                    new RestService
                    {
                        Id = new Uri("tcp:LineItem.item"),
                        Action = new[] {"GET", "PUT", "DELETE"},
                        EndPoint = Uri.TryCreate(Request.RequestUri, lineitemsUrl, out lineitemsServiceUri) ? lineitemsServiceUri : null,
                        Format = new[] {LtiConstants.LisResultMediaType}
                    }
                };

                context.ToolConsumerProfile = profile;
                return Task.FromResult<object>(null);
            };
        }
    }
}
