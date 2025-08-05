using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Documents>> GetDocumentsByUserAsync(string userEmail)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Customer)
                .Where(d => d.AuthorEmail.Contains(userEmail))
                .ToListAsync();
        }

        public async Task<Documents> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Documents> CreateAsync(Documents document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Documents> UpdateAsync(Documents document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null) return false;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Documents.AnyAsync(d => d.Id == id);
        }
    }
}
