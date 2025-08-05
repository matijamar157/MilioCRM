namespace AFuturaCRMV2.Models.DTO
{
    public class CreateDocumentDto
    {
        public string Title { get; set; }
        public int CustomerId { get; set; }
        public int DocumentTypeId { get; set; }
        public string Version { get; set; }
        public string Comment { get; set; }
    }
}
