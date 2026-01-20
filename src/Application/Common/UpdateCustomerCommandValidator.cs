using FluentValidation;

namespace CustomerApi.Validators;

// This is where the "Rules" are born
public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerDTO.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        RuleFor(x => x.CustomerDTO.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        RuleFor(x => x.CustomerDTO.Email)
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.CustomerDTO.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}