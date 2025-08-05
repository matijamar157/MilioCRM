namespace AFuturaCRMV2.Models.DTO
{
    public class CreateTaskDto
    {
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Priority { get; set; } = "Medium";
        public string Category { get; set; }
        public int? CustomerId { get; set; }
    }

    public class UpdateTaskDto : CreateTaskDto
    {
        public string Id { get; set; }
    }
}
