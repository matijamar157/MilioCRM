using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface IInteractionTypeService
    {
        Task<IEnumerable<InteractionType>> GetInteractionTypesAsync();
        Task<InteractionType> GetInteractionTypeByIdAsync(int id);
    }
}
