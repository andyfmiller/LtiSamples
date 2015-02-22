using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LtiLibrary.AspNet.Profiles;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Lti2;
using LtiLibrary.Core.Profiles;

namespace SimpleLti.Controllers
{
    /// <summary>
    /// The ToolConsumerProfileController is hosted by the Tool Consumer and
    /// provides the Tool Consumer Profile API specified by IMS LTI 1.2 (and 2.0).
    /// </summary>
    public class ToolConsumerProfileController : ToolConsumerProfileControllerBase
    {
        public ToolConsumerProfileController()
        {
            OnGetToolConsumerProfile = context =>
            {
                // I use AssemblyInfo to store product and vendor values that may be used
                // in multiple places in this sample
                const string guid = "LtiLibrarySample";
                const string code = "LtiLibrary";
                const string vendorName = "andyfmiller.com";
                const string productName = "LtiLibrary Sample";
                const string productVersion = "1.2";

                // Build a minimal ToolConsumerProfile for LTI 1.2
                var profile = new ToolConsumerProfile
                {
                    CapabilityOffered = new[] { LtiConstants.BasicLaunchLtiMessageType },
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
                                    VendorName = new LocalizedName
                                    {
                                        Key = "product.vendor.name",
                                        Value = vendorName
                                    }
                                }
                            },
                            ProductName = new LocalizedName
                            {
                                Key = "product.name",
                                Value = productName
                            },
                            ProductVersion = productVersion
                        }
                    }
                };
                // Add Outcomes Management
                var outcomesUrl = UrlHelper.GenerateUrl("DefaultApi", null, "Outcomes",
                    new RouteValueDictionary {{"httproute", string.Empty}}, RouteTable.Routes,
                    HttpContext.Current.Request.RequestContext, false);
                Uri serviceUri;
                profile.ServiceOffered = new[]
                {
                    new RestService
                    {
                        Action = new[] {"POST"},
                        EndPoint = Uri.TryCreate(Request.RequestUri, outcomesUrl, out serviceUri) ? serviceUri : null,
                        Format = new[] {LtiConstants.OutcomeMediaType}
                    }
                };
                context.ToolConsumerProfile = profile;
                return Task.FromResult<object>(null);
            };
        }
    }
}
