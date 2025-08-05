using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetUserCompaniesAsync(string username);
        Task<Company> GetCompanyDetailsAsync(int id);
        Task<Company> CreateCompanyAsync(Company company, string username);
        Task<Company> UpdateCompanyAsync(int id, Company company, string username);
        Task<bool> DeleteCompanyAsync(int id);
    }
}
