using AFuturaCRMV2.Models;

namespace AFuturaCRMV2.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<CustomerStatus>> GetCustomerStatusesAsync();
        Task<IEnumerable<CustomerType>> GetCustomerTypesAsync();
        Task<IEnumerable<DocumentType>> GetDocumentTypesAsync();
        Task<IEnumerable<Customer>> GetCompanyCustomersAsync(int companyId);
    }
}
