using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Repository
{
    public class CompanyEmployeeRepository : ICompanyEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyEmployeeRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CompanyEmployee>> GetAllAsync()
        {
            return await _context.CompanyEmployees.ToListAsync();
        }

        public async Task<CompanyEmployee> GetByIdAsync(int id)
        {
            return await _context.CompanyEmployees.FindAsync(id);
        }

        public async Task<CompanyEmployee> GetByEmailAsync(string email)
        {
            return await _context.CompanyEmployees
                .FirstOrDefaultAsync(ce => ce.Email == email);
        }

        public async Task<CompanyEmployee> CreateAsync(CompanyEmployee companyEmployee)
        {
            _context.CompanyEmployees.Add(companyEmployee);
            await _context.SaveChangesAsync();
            return companyEmployee;
        }

        public async Task<CompanyEmployee> UpdateAsync(CompanyEmployee companyEmployee)
        {
            _context.CompanyEmployees.Update(companyEmployee);
            await _context.SaveChangesAsync();
            return companyEmployee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var companyEmployee = await _context.CompanyEmployees.FindAsync(id);
            if (companyEmployee == null) return false;

            _context.CompanyEmployees.Remove(companyEmployee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CompanyEmployees.AnyAsync(ce => ce.Id == id);
        }
    }
}
