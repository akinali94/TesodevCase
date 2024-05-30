using FluentValidation;
using OrderService.V1.Models.CommandModels;

namespace OrderService.V1.Models.Validators;

public class OrderCreateValidator : AbstractValidator<CreateCommand>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required");
        
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity is required")
            .GreaterThan(0).WithMessage("Quantity should be more than 0")
            .InclusiveBetween(1,20).WithMessage("You can add no more than 20 product");
        
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("Price is required")
            .GreaterThan(0).WithMessage("Price should be greater than 0");
        
        RuleFor(x => x.Address.AddressLine)
            .NotEmpty().WithMessage("AddressLine is required.")
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!")
            .Length(2, 25).WithMessage("AddressLine must be between 2 and 25 characters.");

        RuleFor(x => x.Address.Country)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!")
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");

        RuleFor(x => x.Address.CityCode)
            .InclusiveBetween(10000, 100000).WithMessage("CityCode must be between 10000 and 100000.");
        
        RuleFor(x => x.Products[0].Id)
            .NotEmpty().WithMessage("Product Id is required")
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");;
        
        RuleFor(x => x.Products[0].ImageUrl)
            .NotEmpty().WithMessage("Image Url is required")
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");;
        
        RuleFor(x => x.Products[0].Name)
            .NotEmpty().WithMessage("Product Name is required")
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");;
        
    }
}