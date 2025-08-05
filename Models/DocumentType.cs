using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class DocumentType //contract, proposal, invoice
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
    }
}