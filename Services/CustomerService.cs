using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICompanyResolutionService _companyResolutionService;
        private readonly ILogger<CustomerService> _logger;
        private const int MaxCustomersLimit = 10;

        public CustomerService(
            ICustomerRepository customerRepository,
            ICompanyResolutionService companyResolutionService,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _companyResolutionService = companyResolutionService ?? throw new ArgumentNullException(nameof(companyResolutionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Customer>> GetUserCustomersAsync(string userEmail, bool priorityOnly = false)
        {
            var company = await _companyResolutionService.GetUserCompanyAsync(userEmail);
            var customers = await _customerRepository.GetCustomersByUserAsync(userEmail, priorityOnly);

            if (company != null)
            {
                customers = customers.Where(c => c.CompanyId == company.Id);
            }

            return customers;
        }

        public async Task<Customer> GetCustomerDetailsAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {id} not found");
            }
            return customer;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer, string currentUserEmail)
        {
            var company = await _companyResolutionService.ResolveCompanyForUserAsync(currentUserEmail);

            customer.StatusId = 1; // Default status
            customer.CompanyId = company.Id;
            customer.EmailAddress = currentUserEmail;

            return await _customerRepository.CreateAsync(customer);
        }

        public async Task<Customer> UpdateCustomerAsync(int id, Customer customer, int companyId)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                throw new InvalidOperationException($"Customer with ID {id} not found");
            }

            // Update properties
            existingCustomer.Name = customer.Name;
            existingCustomer.Country = customer.Country;
            existingCustomer.Owner = customer.Owner;
            existingCustomer.TelephoneNumber = customer.TelephoneNumber;
            existingCustomer.CustomerTypeId = customer.CustomerTypeId;
            existingCustomer.CustomerStatusId = customer.CustomerStatusId;
            existingCustomer.DealValue = customer.DealValue;
            existingCustomer.Website = customer.Website;
            existingCustomer.DateOfLatestMeeting = customer.DateOfLatestMeeting;
            existingCustomer.PriorityLead = customer.PriorityLead;
            existingCustomer.CompanyId = companyId;

            return await _customerRepository.UpdateAsync(existingCustomer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            return await _customerRepository.DeleteAsync(id);
        }

        public async Task<bool> CheckCustomerLimitAsync(string userEmail)
        {
            var count = await _customerRepository.GetCustomerCountByUserAsync(userEmail);
            return count > MaxCustomersLimit;
        }
    }
}
