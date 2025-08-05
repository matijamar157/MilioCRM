namespace AFuturaCRMV2.Models.DTO
{
    public class CreateInteractionDto
    {
        public DateTime Date { get; set; }
        public int InteractionTypeId { get; set; }
        public int? CustomerId { get; set; }
        public string Notes { get; set; }
        public string Subject { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    public class UpdateInteractionDto : CreateInteractionDto
    {
        public int Id { get; set; }
    }
}
