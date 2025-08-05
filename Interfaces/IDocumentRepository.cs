using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Documents>> GetDocumentsByUserAsync(string userEmail);
        Task<Documents> GetByIdAsync(int id);
        Task<Documents> CreateAsync(Documents document);
        Task<Documents> UpdateAsync(Documents document);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
