using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Services
{
    public class LookupService : ILookupService
    {
        private readonly ApplicationDbContext _context;

        public LookupService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CustomerStatus>> GetCustomerStatusesAsync()
        {
            return await _context.CustomerStatuses.ToListAsync();
        }

        public async Task<IEnumerable<CustomerType>> GetCustomerTypesAsync()
        {
            return await _context.CustomerTypes.ToListAsync();
        }

        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
        {
            return await _context.DocumentTypes.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCompanyCustomersAsync(int companyId)
        {
            return await _context.Customers
                .Where(c => c.CompanyId == companyId)
                .ToListAsync();
        }
    }
}
