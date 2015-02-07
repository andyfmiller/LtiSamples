namespace SimpleLti.Models
{
    public class BasicOutcomeModel
    {
        public string LisOutcomeServiceUrl { get; set; }
        public string LisResultSourcedId { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public double? Score { get; set; }
    }
}