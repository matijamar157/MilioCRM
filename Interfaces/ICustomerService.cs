using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetUserCustomersAsync(string userEmail, bool priorityOnly = false);
        Task<Customer> GetCustomerDetailsAsync(int id);
        Task<Customer> CreateCustomerAsync(Customer customer, string currentUserEmail);
        Task<Customer> UpdateCustomerAsync(int id, Customer customer, int companyId);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> CheckCustomerLimitAsync(string userEmail);
    }
}
