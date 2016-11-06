using ConsumerCertification.Models;
using HtmlAgilityPack;
using LtiLibrary.AspNet.Extensions;
using LtiLibrary.AspNet.Lti1;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Lti1;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using LtiLibrary.Core.OAuth;

namespace ConsumerCertification.Controllers
{
    public class Lti1Controller : Controller
    {
        private TestCourse Course1 = new TestCourse
        {
            CourseId = "Course1",
            Title = "Design of <b>Personal</b> Environments 1",
            Links = new[] {
                    new TestLink {
                        LinkId = "link1",
                        Title = "My <i>Weekly</i> Wiki",
                        Description = "This learning space is <b>private</b>"
                    },
                    new TestLink {
                        LinkId = "link2",
                        Title = "Prescribed text",
                        Description = "This is the textbook to accompany the course."
                    }
                }
        };
        private TestCourse Course2 = new TestCourse
        {
            CourseId = "Course2",
            Title = "Design of Personal Environments 2",
            Links = new[] {
                    new TestLink {
                        LinkId = "link3",
                        Title = "My Learning Diary",
                        Description = "Record your activities within this course."
                    }
            }
        };
        private TestLink GradebookTool = new TestLink
        {
            LinkId = "link4",
            Title = "External Gradebook",
            Description = "Track student progress"
        };
        private TestUser Instructor = new TestUser
        {
            Email = "sian@imscert.org",
            FamilyName = "Instructor",
            GivenName = "Siân",
            UserId = "1",
            Username = "sinstructor"
        };
        private TestUser Student = new TestUser
        {
            Email = "john@imscert.org",
            FamilyName = "Student",
            GivenName = "John",
            UserId = "2",
            Username = "jstudent"
        };
        private TestMentor Mentor = new TestMentor
        {
            Email = "bill@imscert.org",
            FamilyName = "Mentor",
            GivenName = "Bill",
            UserId = "3",
            MentoringUserId = "2",
            Username = "bmentor"
        };
        public static string CustomParameters = @"simple_key=custom_simple_value
Complex!@#$^*(){}[]KEY=Complex!@#$^*;(){}[]½Value
cert_userid=$User.id
cert_username=$User.username
tc_profile_url=$ToolConsumerProfile.url
lineitem_url=$LineItem.url
lineitems_url=$LineItems.url
result_url=$Result.url
results_url=$Results.url";

        // GET: Lti1
        public ActionResult Index()
        {
            var model = new Lti1TestLaunch
            {
                Url = "https://www.imsglobal.org/lti/cert/tc_tool.php?x=With%20Space&y=yes",
                CustomParameters = CustomParameters
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Lti1TestLaunch model, string link)
        {
            if (ModelState.IsValid)
            {
                // Store the secret to LineItemsController and ResultsController can authenticate requests
                InMemoryDb.ConsumerSecret = model.ConsumerSecret;
                InMemoryDb.LineItem = null;
                InMemoryDb.Result = null;

                return RedirectToAction("TestSuite", "Lti1", new { Url = model.Url, ConsumerKey = model.ConsumerKey, ConsumerSecret = model.ConsumerSecret });
            }
            return View(model);
        }

        public ActionResult LtiLaunch(Lti1TestLaunch model, int link)
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType);
            request.ConsumerKey = model.ConsumerKey;
            request.Url = new Uri(model.Url);
            request.AddCustomParameters(CustomParameters);
            request.LaunchPresentationLocale = Thread.CurrentThread.CurrentUICulture.Name;
            request.LaunchPresentationDocumentTarget = DocumentTarget.frame;
            request.LaunchPresentationWidth = 400;
            request.LaunchPresentationHeight = 300;
            request.LaunchPresentationCssUrl = GetLaunchPresentationCssUrl();
            request.LaunchPresentationReturnUrl = Request.Url.AbsoluteUri;
            request.LineItemServiceUrl = GetLineItemsServiceUrl(InMemoryDb.ContextId, InMemoryDb.LineItemId);
            request.LineItemsServiceUrl = GetLineItemsServiceUrl(InMemoryDb.ContextId);
            request.LisOutcomeServiceUrl = GetLisOutcomeServiceUrl();
            request.ResultServiceUrl = GetResultsServiceUrl(InMemoryDb.ContextId, InMemoryDb.LineItemId, InMemoryDb.ResultId);
            request.ResultsServiceUrl = GetResultsServiceUrl(InMemoryDb.ContextId, InMemoryDb.LineItemId);
            request.ToolConsumerInfoProductFamilyCode = "LtiLibrary";
            request.ToolConsumerInfoVersion = "1";
            request.ToolConsumerProfileUrl = GetToolConsumerProfileUrl();
            request.ToolConsumerInstanceContactEmail = "support@andyfmiller.com";
            request.ToolConsumerInstanceDescription = Assembly.GetExecutingAssembly().GetName().ToString();
            request.ToolConsumerInstanceGuid = Assembly.GetExecutingAssembly().GetName().Name;
            request.ToolConsumerInstanceName = "andyfmiller.com";

            switch (link)
            {
                case 1: // Instructor, Course 1, Resource 1
                    request.ContextId = Course1.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course1.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.LisCourseOfferingSourcedId = request.ContextId;
                    request.LisCourseSectionSourcedId = request.ContextId;
                    request.ResourceLinkId = Course1.Links[0].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course1.Links[0].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course1.Links[0].Description);
                    request.LisPersonEmailPrimary = Instructor.Email;
                    request.LisPersonNameFamily = Instructor.FamilyName;
                    request.LisPersonNameGiven = Instructor.GivenName;
                    request.LisPersonSourcedId = Instructor.UserId;
                    request.UserId = Instructor.UserId;
                    request.UserName = Instructor.Username;
                    request.Roles = Role.Instructor.ToString();
                    break;
                case 2: // Instructor, Course 1, Resource 2
                    request.ContextId = Course1.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course1.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course1.Links[1].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course1.Links[1].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course1.Links[1].Description);
                    request.LisPersonEmailPrimary = Instructor.Email;
                    request.LisPersonNameFamily = Instructor.FamilyName;
                    request.LisPersonNameGiven = Instructor.GivenName;
                    request.LisPersonSourcedId = Instructor.UserId;
                    request.UserId = Instructor.UserId;
                    request.UserName = Instructor.Username;
                    request.Roles = Role.Instructor.ToString();
                    break;
                case 3: // Instructor, Course 2, Resource 1
                    request.ContextId = Course2.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course2.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course2.Links[0].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course2.Links[0].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course2.Links[0].Description);
                    request.LisPersonEmailPrimary = Instructor.Email;
                    request.LisPersonNameFamily = Instructor.FamilyName;
                    request.LisPersonNameGiven = Instructor.GivenName;
                    request.LisPersonSourcedId = Instructor.UserId;
                    request.UserId = Instructor.UserId;
                    request.UserName = Instructor.Username;
                    request.Roles = Role.Instructor.ToString();
                    break;
                case 4: // Student, Course 1, Resource 1
                    request.ContextId = Course1.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course1.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course1.Links[0].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course1.Links[0].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course1.Links[0].Description);
                    request.LisPersonEmailPrimary = Student.Email;
                    request.LisPersonNameFamily = Student.FamilyName;
                    request.LisPersonNameGiven = Student.GivenName;
                    request.LisPersonSourcedId = Student.UserId;
                    request.LisResultSourcedId = $"{Student.UserId}-{Course1.Links[0].LinkId}";
                    request.UserId = Student.UserId;
                    request.UserName = Student.Username;
                    request.Roles = Role.Learner.ToString();
                    break;
                case 5: // Student, Course 1, Resource 2
                    request.ContextId = Course1.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course1.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course1.Links[1].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course1.Links[1].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course1.Links[1].Description);
                    //request.LisPersonEmailPrimary = Student.Email;
                    request.LisPersonNameFamily = Student.FamilyName;
                    request.LisPersonNameGiven = Student.GivenName;
                    request.LisPersonSourcedId = Student.UserId;
                    request.LisResultSourcedId = $"{Student.UserId}-{Course1.Links[1].LinkId}";
                    request.UserId = Student.UserId;
                    request.UserName = Student.Username;
                    request.Roles = Role.Learner.ToString();
                    break;
                case 6: // Student, Course 2, Resource 1
                    request.ContextId = Course2.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course2.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course2.Links[0].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course2.Links[0].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course2.Links[0].Description);
                    //request.LisPersonEmailPrimary = Student.Email;
                    //request.LisPersonNameFamily = Student.FamilyName;
                    //request.LisPersonNameGiven = Student.GivenName;
                    request.LisPersonSourcedId = Student.UserId;
                    request.LisResultSourcedId = $"{Student.UserId}-{Course2.Links[0].LinkId}";
                    request.UserId = Student.UserId;
                    request.UserName = Student.Username;
                    request.Roles = Role.Learner.ToString();
                    break;
                case 7: // Mentor, Course 1, Resource 1
                        // Tests passed with this launch
                        // 5.9
                    request.ContextId = Course1.CourseId.ToString(CultureInfo.InvariantCulture);
                    request.ContextLabel = request.ContextId;
                    request.ContextTitle = ConvertToPlainText(Course1.Title);
                    request.ContextType = LisContextType.CourseSection;
                    request.ResourceLinkId = Course1.Links[0].LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(Course1.Links[0].Title);
                    request.ResourceLinkDescription = ConvertToPlainText(Course1.Links[0].Description);
                    request.LisPersonEmailPrimary = Mentor.Email;
                    request.LisPersonNameFamily = Mentor.FamilyName;
                    request.LisPersonNameGiven = Mentor.GivenName;
                    request.UserId = Mentor.UserId;
                    request.UserName = Mentor.Username;
                    request.Roles = Role.Mentor.ToString();
                    request.RoleScopeMentor = Mentor.MentoringUserId;
                    break;
                case 8: // Instructor, No context (6.4)
                        // Tests passed with this launch
                        // 6.4
                    request.ResourceLinkId = GradebookTool.LinkId;
                    request.ResourceLinkTitle = ConvertToPlainText(GradebookTool.Title);
                    request.ResourceLinkDescription = ConvertToPlainText(GradebookTool.Description);
                    request.LisPersonEmailPrimary = Instructor.Email;
                    request.LisPersonNameFamily = Instructor.FamilyName;
                    request.LisPersonNameGiven = Instructor.GivenName;
                    request.LisPersonSourcedId = Instructor.UserId;
                    request.UserId = Instructor.UserId;
                    request.UserName = Instructor.Username;
                    request.Roles = Role.Instructor.ToString();
                    break;
            }
            return View(request.GetViewModel(model.ConsumerSecret));
        }

        public ActionResult TestSuite(Lti1TestLaunch model)
        {
            return View(model);
        }

        private string ConvertToPlainText(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.ToPlainText();
        }

        private string GetLaunchPresentationCssUrl()
        {
            Uri profileUri;
            if (Uri.TryCreate(Request.Url, "/content/site.css", out profileUri))
            {
                return profileUri.AbsoluteUri;
            }
            return null;
        }

        private string GetLineItemsServiceUrl(string contextId, string id = null)
        {
            var uri = RoutingHelper.GetLineItemsUri(HttpContext, contextId, id);
            return uri == null ? null : uri.AbsoluteUri;
        }

        private string GetResultsServiceUrl(string contextId, string lineItemId, string id = null)
        {
            var uri = RoutingHelper.GetResultsUri(HttpContext, contextId, lineItemId, id);
            return uri == null ? null : uri.AbsoluteUri;
        }

        private string GetLisOutcomeServiceUrl()
        {
            Uri profileUri;
            if (Uri.TryCreate(Request.Url, "/api/outcomes", out profileUri))
            {
                return profileUri.AbsoluteUri;
            }
            return null;
        }

        private string GetToolConsumerProfileUrl()
        {
            Uri profileUri;
            if (Uri.TryCreate(Request.Url, "/api/profile", out profileUri))
            {
                return profileUri.AbsoluteUri;
            }
            return null;
        }
    }
}