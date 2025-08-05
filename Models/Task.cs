using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class Task
    {
        [Key]
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Priority { get; set; }
        public string Category { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserEmail { get; set; }
    }
}
