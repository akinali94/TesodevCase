using CustomerService.V1.Models.RequestModels;
using CustomerService.V1.Models.ResponseModels;

namespace CustomerService.Services;

public interface ICustomerService
{
    Task<Guid> Create(CustomerCreateModel newCustomer);
    Task<bool> Update(Guid id, CustomerPatchModel updatedCustomer);
    Task<bool> Delete(Guid id);
    Task<CustomerGetModel> GetById(Guid id);
    Task<IEnumerable<CustomerGetModel>> GetAll();
    Task<bool> Validate(Guid id);
}