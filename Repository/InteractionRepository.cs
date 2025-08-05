using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Repository
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly ApplicationDbContext _context;

        public InteractionRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Interaction>> GetAllAsync()
        {
            return await _context.Interaction
                .Include(i => i.InteractionType)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interaction>> GetByUserAsync(string userEmail)
        {
            return await _context.Interaction
                .Include(i => i.InteractionType)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interaction>> GetByCustomerAsync(int customerId)
        {
            return await _context.Interaction
                .Include(i => i.InteractionType)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task<Interaction> GetByIdAsync(int id)
        {
            return await _context.Interaction
                .Include(i => i.InteractionType)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Interaction> CreateAsync(Interaction interaction)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));

            _context.Interaction.Add(interaction);
            await _context.SaveChangesAsync();
            return interaction;
        }

        public async Task<Interaction> UpdateAsync(Interaction interaction)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));

            _context.Interaction.Update(interaction);
            await _context.SaveChangesAsync();
            return interaction;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var interaction = await _context.Interaction.FindAsync(id);
            if (interaction == null)
                return false;

            _context.Interaction.Remove(interaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Interaction.AnyAsync(i => i.Id == id);
        }
    }
}
