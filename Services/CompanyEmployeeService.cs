using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Services
{
    public class CompanyEmployeeService : ICompanyEmployeeService
    {
        private readonly ICompanyEmployeeRepository _repository;
        private readonly ILogger<CompanyEmployeeService> _logger;

        public CompanyEmployeeService(ICompanyEmployeeRepository repository, ILogger<CompanyEmployeeService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CompanyEmployee>> GetAllCompanyEmployeesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CompanyEmployee> GetCompanyEmployeeDetailsAsync(int id)
        {
            var companyEmployee = await _repository.GetByIdAsync(id);
            if (companyEmployee == null)
            {
                throw new InvalidOperationException($"Company employee with ID {id} not found");
            }
            return companyEmployee;
        }

        public async Task<CompanyEmployee> CreateCompanyEmployeeAsync(CompanyEmployee companyEmployee, string currentUserEmail)
        {
            companyEmployee.Email = currentUserEmail;
            return await _repository.CreateAsync(companyEmployee);
        }

        public async Task<CompanyEmployee> CreateBossRequestAsync(CompanyEmployee companyEmployee, string currentUserEmail)
        {
            companyEmployee.Email = currentUserEmail;
            return await _repository.CreateAsync(companyEmployee);
        }

        public async Task<CompanyEmployee> UpdateCompanyEmployeeAsync(int id, CompanyEmployee companyEmployee)
        {
            var existingEmployee = await _repository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                throw new InvalidOperationException($"Company employee with ID {id} not found");
            }

            existingEmployee.Company = companyEmployee.Company;
            existingEmployee.Employee = companyEmployee.Employee;
            existingEmployee.BossEmail = companyEmployee.BossEmail;
            existingEmployee.Email = companyEmployee.Email;

            return await _repository.UpdateAsync(existingEmployee);
        }

        public async Task<bool> DeleteCompanyEmployeeAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
