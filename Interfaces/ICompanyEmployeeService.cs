using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ICompanyEmployeeService
    {
        Task<IEnumerable<CompanyEmployee>> GetAllCompanyEmployeesAsync();
        Task<CompanyEmployee> GetCompanyEmployeeDetailsAsync(int id);
        Task<CompanyEmployee> CreateCompanyEmployeeAsync(CompanyEmployee companyEmployee, string currentUserEmail);
        Task<CompanyEmployee> CreateBossRequestAsync(CompanyEmployee companyEmployee, string currentUserEmail);
        Task<CompanyEmployee> UpdateCompanyEmployeeAsync(int id, CompanyEmployee companyEmployee);
        Task<bool> DeleteCompanyEmployeeAsync(int id);
    }
}
