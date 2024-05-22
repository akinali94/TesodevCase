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

    public Customer PatchModelToModel(CustomerPatchModel patchModel)
    {
        Customer updatedCustomer = new Customer();
        updatedCustomer.Name = patchModel.Name;
        updatedCustomer.Email = patchModel.Email;

        if (patchModel.Addresses != null)
        {
            foreach (var addressModel in patchModel.Addresses)
            {
                var address = new Address
                {
                    AddressLine = addressModel.AddressLine,
                    City = addressModel.City,
                    Country = addressModel.Country,
                    CityCode = addressModel.CityCode ?? default(int)
                };
                updatedCustomer.Address.Add(address);
            }
        }

        return updatedCustomer;
    }
}