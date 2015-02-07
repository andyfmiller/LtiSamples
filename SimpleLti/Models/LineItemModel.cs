using LtiLibrary.Core.Outcomes.v2;

namespace SimpleLti.Models
{
    public class LineItemModel
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string ContextId { get; set; }
        public LineItem LineItem { get; set; }
        public string LineItemServiceUrl { get; set; }
        public string LineItemsServiceUrl { get; set; }
        public string HttpRequest { get; set; }
        public string HttpResponse { get; set; }
    }
}