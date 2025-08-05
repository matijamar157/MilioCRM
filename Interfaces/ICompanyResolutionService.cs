using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ICompanyResolutionService
    {
        Task<Company> GetUserCompanyAsync(string userEmail);
        Task<Company> ResolveCompanyForUserAsync(string userEmail);
    }
}
