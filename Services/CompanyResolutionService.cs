using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Services
{
    public class CompanyResolutionService : ICompanyResolutionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyResolutionService> _logger;

        public CompanyResolutionService(ApplicationDbContext context, ILogger<CompanyResolutionService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Company> GetUserCompanyAsync(string userEmail)
        {
            return await _context.Company
                .FirstOrDefaultAsync(c => c.Username == userEmail);
        }

        public async Task<Company> ResolveCompanyForUserAsync(string userEmail)
        {
            var company = await GetUserCompanyAsync(userEmail);

            if (company == null)
            {
                // Try to find company through boss relationship
                var employee = await _context.CompanyEmployees
                    .FirstOrDefaultAsync(ce => ce.Email == userEmail);

                if (employee != null)
                {
                    company = await _context.Company
                        .FirstOrDefaultAsync(c => c.Username == employee.BossEmail);
                }
            }

            if (company == null)
            {
                throw new InvalidOperationException("No company found for user");
            }

            return company;
        }
    }
}
