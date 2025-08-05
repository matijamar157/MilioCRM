using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class CompanyEmployee
    {
        [Key]
        public int Id { get; set; }
        public bool Company { get; set; }
        public bool Employee { get; set; }
        public string? BossEmail { get; set; }
        public string Email { get; set; } = "";
    }
}
