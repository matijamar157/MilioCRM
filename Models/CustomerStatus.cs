using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class CustomerStatus //active, inactive
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}