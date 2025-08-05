using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface IInteractionService
    {
        Task<IEnumerable<Interaction>> GetAllInteractionsAsync();
        Task<IEnumerable<Interaction>> GetUserInteractionsAsync(string userEmail);
        Task<IEnumerable<Interaction>> GetCustomerInteractionsAsync(int customerId);
        Task<Interaction> GetInteractionDetailsAsync(int id);
        Task<Interaction> CreateInteractionAsync(Interaction interaction, string currentUserEmail);
        Task<Interaction> UpdateInteractionAsync(int id, Interaction interaction);
        Task<bool> DeleteInteractionAsync(int id);
    }
}
