using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Username { get; set; }
    }
}
