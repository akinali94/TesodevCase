using CustomerService.Models;
using CustomerService.V1.Models.RequestModels;
using CustomerService.V1.Models.ResponseModels;

namespace CustomerService.Mappers;

public class CustomerMapper : ICustomerMapper
{
    public Customer CreateModelToModel(CustomerCreateModel createModel)
    {    
        return new Customer
        {
            Name = createModel.Name,
            Email = createModel.Email,
            Address = new List<Address>
            {
                new Address
                {
                    AddressLine = createModel.AddressLine,
                    City = createModel.City,
                    Country = createModel.Country,
                    CityCode = createModel.CityCode
                }
            },
            UpdatedAt = null
        };
    }

    public CustomerGetModel ModelToGetModel(Customer customer)
    {
        return new CustomerGetModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Addresses = customer.Address?.Select(x => new CustomerGetModel.AddressGetModel
            {
                AddressLine = x.AddressLine,
                City = x.City,
                Country = x.Country,
                CityCode = x.CityCode
            }).ToList(),
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    public IEnumerable<CustomerGetModel> ModelToGetAllModel(IEnumerable<Customer> customers)
    {
        return customers.Select(c => new CustomerGetModel
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Addresses = c.Address?.Select(a => new CustomerGetModel.AddressGetModel
            {
                AddressLine = a.AddressLine,
                City = a.City,
                Country = a.Country,
                CityCode = a.CityCode
            }).ToList(),
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public Customer PatchModelToModel(CustomerPatchModel patchModel, Customer oldCustomer)
    {
        
        oldCustomer.Name = patchModel.Name ?? oldCustomer.Name;
        oldCustomer.Email = patchModel.Email ?? oldCustomer.Email;
        
        for (int i = 0; i < oldCustomer.Address.Count; i++)
        {
            oldCustomer.Address[i].AddressLine = patchModel.Addresses[i].AddressLine ?? oldCustomer.Address[i].AddressLine;
            oldCustomer.Address[i].Country = patchModel.Addresses[i].Country ?? oldCustomer.Address[i].Country;
            oldCustomer.Address[i].CityCode = patchModel.Addresses[i].CityCode ?? oldCustomer.Address[i].CityCode;
            oldCustomer.Address[i].City = patchModel.Addresses[i].City ?? oldCustomer.Address[i].City;
        }

        return oldCustomer;
    }
}