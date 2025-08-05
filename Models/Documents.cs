using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace AFuturaCRMV2.Models
{
    public class Documents
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string AuthorEmail { get; set; }
        public int Version { get; set; } = 1;
        public string Comment { get; set; }
    }
}
