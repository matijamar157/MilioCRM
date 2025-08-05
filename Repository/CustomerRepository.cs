using AFuturaCRMV2.Data;
using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;
using Microsoft.EntityFrameworkCore;

namespace AFuturaCRMV2.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Customer>> GetCustomersByUserAsync(string userEmail, bool priorityOnly = false)
        {
            var query = _context.Customers
                .Include(c => c.CustomerStatus)
                .Include(c => c.CustomerType)
                .Include(c => c.Company)
                .Where(c => c.EmailAddress == userEmail);

            if (priorityOnly)
            {
                query = query.Where(c => c.PriorityLead);
            }

            return await query.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.CustomerStatus)
                .Include(c => c.CustomerType)
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetCustomerCountByUserAsync(string userEmail)
        {
            return await _context.Customers
                .Where(c => c.EmailAddress == userEmail)
                .CountAsync();
        }
    }
}
