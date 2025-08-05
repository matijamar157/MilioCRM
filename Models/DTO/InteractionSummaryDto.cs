namespace AFuturaCRMV2.Models.DTO
{
    public class InteractionSummaryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string InteractionTypeName { get; set; }
        public string CustomerName { get; set; }
        public string Subject { get; set; }
        public string UserEmail { get; set; }
    }
}
