namespace CustomerService.V1.Models.RequestModels;

public class CustomerCreateModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string AddressLine { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public int CityCode { get; set; }
}