using CustomerService.Mappers;
using CustomerService.Models;
using CustomerService.Repositories;
using CustomerService.V1.Models.RequestModels;
using CustomerService.V1.Models.ResponseModels;

namespace CustomerService.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ICustomerMapper _mapper;

    public CustomerService(ICustomerRepository repository, ICustomerMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Guid> Create(CustomerCreateModel createdModel)
    {
        Customer newCustomer = _mapper.CreateModelToModel(createdModel);
        
        newCustomer.Id = Guid.NewGuid();
        newCustomer.CreatedAt = DateTime.Now;
        
        return await _repository.CreateAsync(newCustomer);
    }

    public async Task<bool> Update(Guid id, CustomerPatchModel updatedModel)
    {
        if (await _repository.GetByIdAsync(id) is null)
            throw new KeyNotFoundException("Customer not found on Update Service");
        
        Customer updatedCustomer = _mapper.PatchModelToModel(updatedModel);

        return await _repository.UpdateAsync(id, updatedCustomer);

    }

    public async Task<bool> Delete(Guid id)
    {
        if (await _repository.GetByIdAsync(id) is null)
            throw new KeyNotFoundException("Customer not found on Delete Service");

        return await _repository.DeleteAsync(id);
    }

    public async Task<CustomerGetModel> GetById(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        
        if (customer == null)
            throw new KeyNotFoundException("No Customer Found with this Id");
        
        return _mapper.ModelToGetModel(customer);
    }

    public async Task<IEnumerable<CustomerGetModel>> GetAll()
    {
        var customers = await _repository.GetAllAsync();
        
        if (customers == null)
            throw new KeyNotFoundException("No Customers Found");

        return _mapper.ModelToGetAllModel(customers);
    }

    public async Task<bool> Validate(Guid id)
    {
        var customer = _repository.GetByIdAsync(id);
        if (customer is null)
            return false;
        
        return true;
    }
}