using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Services
{
    public class InteractionTypeService : IInteractionTypeService
    {
        private readonly ApplicationDbContext _context;

        public InteractionTypeService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InteractionType>> GetInteractionTypesAsync()
        {
            return await _context.InteractionTypes.OrderBy(it => it.Name).ToListAsync();
        }

        public async Task<InteractionType> GetInteractionTypeByIdAsync(int id)
        {
            return await _context.InteractionTypes.FindAsync(id);
        }
    }
}
