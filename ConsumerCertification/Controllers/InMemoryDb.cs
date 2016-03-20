using LtiLibrary.Core.Outcomes.v2;

namespace ConsumerCertification.Controllers
{
    public static class InMemoryDb
    {
        // Simple "database" of lineitems for demonstration purposes
        public const string ContextId = "course-1";
        public const string LineItemId = "lineitem-1";
        public const string ResultId = "result-1";
        public static LineItem LineItem;
        public static LisResult Result;

        // ConsumerSecret used in authentication
        public static string ConsumerSecret;
    }
}
