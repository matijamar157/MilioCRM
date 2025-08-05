using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ICompanyEmployeeRepository
    {
        Task<IEnumerable<CompanyEmployee>> GetAllAsync();
        Task<CompanyEmployee> GetByIdAsync(int id);
        Task<CompanyEmployee> CreateAsync(CompanyEmployee companyEmployee);
        Task<CompanyEmployee> UpdateAsync(CompanyEmployee companyEmployee);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<CompanyEmployee> GetByEmailAsync(string email);
    }
}
