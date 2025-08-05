using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class CustomerType // lead, prospect, finished documentation, customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}