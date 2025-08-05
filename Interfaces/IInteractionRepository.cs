using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface IInteractionRepository
    {
        Task<IEnumerable<Interaction>> GetAllAsync();
        Task<Interaction> GetByIdAsync(int id);
        Task<Interaction> CreateAsync(Interaction interaction);
        Task<Interaction> UpdateAsync(Interaction interaction);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Interaction>> GetByUserAsync(string userEmail);
        Task<IEnumerable<Interaction>> GetByCustomerAsync(int customerId);
    }
}
