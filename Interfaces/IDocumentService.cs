using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Documents>> GetUserDocumentsAsync(string userEmail);
        Task<string> GetDocumentUrlAsync(int id);
        Task<Documents> CreateDocumentAsync(Documents document, IFormFile file, string currentUserEmail);
        Task<Documents> UpdateDocumentAsync(int id, Documents document);
        Task<bool> DeleteDocumentAsync(int id);
        Task<bool> CanCreateDocumentAsync(string userEmail);
    }
}
