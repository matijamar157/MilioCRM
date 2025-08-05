namespace AFuturaCRMV2.Models.DTO
{
    public class UpdateDocumentDto
    {
        public string Title { get; set; }
        public int DocumentTypeId { get; set; }
        public string Version { get; set; }
        public string Comment { get; set; }
    }
}
