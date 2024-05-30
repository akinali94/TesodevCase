using FluentValidation;
using OrderService.V1.Models.CommandModels;

namespace OrderService.V1.Models.Validators;

public class OrderUpdateValidator : AbstractValidator<UpdateCommand>
{
    public OrderUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Customer Id can not be empty")
            .Must(x => x.Length == 36).WithMessage("Id must be 36 Characters");
        RuleFor(x => x.CustomerId)
            .Must(x => x.Length == 36).WithMessage("Id must be 36 Characters")
            .NotEmpty().WithMessage("Customer Id can not be empty");
        
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity can not be empty")
            .GreaterThan(0).WithMessage("Quantity should be more than 0")
            .InclusiveBetween(1,20).WithMessage ("You can add no more than 20 product");
        
        RuleFor(x => x.Addresses)
            .NotEmpty().WithMessage("Address can not be empty");
        
        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("Products can not be empty");
        
        RuleFor(x => x.Addresses.AddressLine)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
        RuleFor(x => x.Addresses.City)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
        RuleFor(x => x.Addresses.Country)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
        RuleFor(x => x.Addresses.CityCode)
            .InclusiveBetween(10000, 100000).WithMessage("CityCode must be between 10000 and 100000.");

        RuleFor(x => x.Products[0].Id)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
        RuleFor(x => x.Products[0].ImageUrl)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
        RuleFor(x => x.Products[0].Name)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");
    }
}