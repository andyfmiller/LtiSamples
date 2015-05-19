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
                    Context = new [] 
                    {
                        LtiConstants.ToolConsumerProfileContext
                    },
                    Id = Request.RequestUri.ToString(),
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
                        "Membership.role"
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
                        },
                        ServiceOwner = new ServiceOwner
                        {
                            Name = new LocalizedName
                            {
                                Key = "service_owner.name",
                                Value = vendorName
                            },
                            Description = new LocalizedText
                            {
                                Key = "service_owner.description",
                                Value = Assembly.GetExecutingAssembly().FullName
                            },
                            Support = new Contact ("support@andyfmiller.com")
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
