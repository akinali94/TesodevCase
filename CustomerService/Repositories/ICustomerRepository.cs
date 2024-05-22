using CustomerService.Models;

namespace CustomerService.Repositories;

public interface ICustomerRepository
{
     Task<Guid> CreateAsync(Customer newCustomer);
     
    Task<bool> UpdateAsync(Guid id, Customer updatedCustomer);
    
    Task<bool> DeleteAsync(Guid id);

    Task<Customer> GetByIdAsync(Guid id);

    Task<IEnumerable<Customer>> GetAllAsync();
}