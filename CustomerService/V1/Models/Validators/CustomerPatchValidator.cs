using CustomerService.V1.Models.RequestModels;
using FluentValidation;

namespace CustomerService.V1.Models.Validators;

public class CustomerPatchValidator : AbstractValidator<CustomerPatchModel>
{
    public CustomerPatchValidator()
    {
        RuleFor(x => x.Name)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Addresses[0].AddressLine)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!")
            .Length(2, 25).WithMessage("AddressLine must be between 2 and 25 characters.");

        RuleFor(x => x.Addresses[0].Country)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");

        RuleFor(x => x.Addresses[0].City)
            .Must(x => x != "string").WithMessage("string word is not a valid name")
            .Must(x => x != "").WithMessage("It can not be blank!");

        RuleFor(x => x.Addresses[0].CityCode)
            .InclusiveBetween(10000, 100000).WithMessage("CityCode must be between 10000 and 100000.");
    }
    
}