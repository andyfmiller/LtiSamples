using System.ComponentModel.DataAnnotations;

namespace ConsumerCertification.Models
{
    public class Lti1TestLaunch
    {
        [Required]
        public string ConsumerKey { get; set; }

        [Required]
        public string ConsumerSecret { get; set; }

        [Required]
        public string CustomParameters { get; set; }

        [Required]
        public string Url { get; set; }
    }
}