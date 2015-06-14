using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Consumer.Lti;
using LtiLibrary.AspNet.Profiles;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Lti2;
using LtiLibrary.Core.Profiles;

namespace Consumer.Controllers
{
    /// <summary>
    /// This is a sample ApiController that returns a minimal ToolConsumerProfile.
    /// </summary>
    public class ToolConsumerProfileController : ToolConsumerProfileControllerBase
    {
        public ToolConsumerProfileController()
        {
            OnGetToolConsumerProfile = context =>
            {
                // I use AssemblyInfo to store product and vendor values that may be used
                // in multiple places in this sample
                var guid = LtiUtility.GetProduct();
                var code = guid; // Product and ProductFamily are the same in this sample
                var vendorName = LtiUtility.GetCompany();
                var productName = LtiUtility.GetTitle();
                var productVersion = LtiUtility.GetVersion();

                // Build a minimal ToolConsumerProfile for LTI 1.2
                var profile = new ToolConsumerProfile
                {
                    CapabilityOffered = new[] {LtiConstants.BasicLaunchLtiMessageType},
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
