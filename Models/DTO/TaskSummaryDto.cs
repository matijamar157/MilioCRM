namespace AFuturaCRMV2.Models.DTO
{
    public class TaskSummaryDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; }
        public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.Today && !IsCompleted;
        public string CustomerName { get; set; }
        public string UserEmail { get; set; }
    }
}
