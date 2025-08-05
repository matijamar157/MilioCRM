using AFuturaCRMV2.Interfaces;
using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ICompanyRepository companyRepository, ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Company>> GetUserCompaniesAsync(string username)
        {
            _logger.LogInformation("Retrieving companies for user: {Username}", username);
            return await _companyRepository.GetCompaniesByUserAsync(username);
        }

        public async Task<Company> GetCompanyDetailsAsync(int id)
        {
            _logger.LogInformation("Retrieving company details for ID: {CompanyId}", id);
            var company = await _companyRepository.GetCompanyByIdAsync(id);

            if (company == null)
            {
                _logger.LogWarning("Company not found with ID: {CompanyId}", id);
                throw new InvalidOperationException($"Company with ID {id} not found");
            }

            return company;
        }

        public async Task<Company> CreateCompanyAsync(Company company, string username)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username is required", nameof(username));

            company.Username = username;

            _logger.LogInformation("Creating new company: {CompanyName} for user: {Username}", company.Name, username);
            return await _companyRepository.CreateCompanyAsync(company);
        }

        public async Task<Company> UpdateCompanyAsync(int id, Company company, string username)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username is required", nameof(username));

            var existingCompany = await _companyRepository.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                _logger.LogWarning("Attempted to update non-existent company with ID: {CompanyId}", id);
                throw new InvalidOperationException($"Company with ID {id} not found");
            }

            // Update properties
            existingCompany.Name = company.Name;
            existingCompany.Address = company.Address;
            existingCompany.Country = company.Country;
            existingCompany.Username = username;

            _logger.LogInformation("Updating company: {CompanyId} for user: {Username}", id, username);
            return await _companyRepository.UpdateCompanyAsync(existingCompany);
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            _logger.LogInformation("Deleting company with ID: {CompanyId}", id);
            var result = await _companyRepository.DeleteCompanyAsync(id);

            if (!result)
            {
                _logger.LogWarning("Attempted to delete non-existent company with ID: {CompanyId}", id);
            }

            return result;
        }
    }
}
