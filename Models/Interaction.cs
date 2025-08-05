namespace AFuturaCRMV2.Models
{
    public class Interaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int InteractionTypeId { get; set; }
        public InteractionType InteractionType { get; set; }
        public int CustomerId { get; set; }
        public string Notes { get; set; }
        public string Subject { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
