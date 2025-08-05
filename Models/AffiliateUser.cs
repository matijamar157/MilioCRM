using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class AffiliateUser
    {
        [Key]
        public int Id { get; set; }
        public string AffiliateEmail { get; set; }
        public string AffiliateProviderEmail { get; set; }
        public string AffiliatePackage { get; set; }
    }
}
