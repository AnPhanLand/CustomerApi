using FluentValidation;

namespace CustomerApp.Validators;

// This is where the "Rules" are born
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        // Rule 1: Name cannot be empty
        RuleFor(x => x.CustomerDTO.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        RuleFor(x => x.CustomerDTO.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        // Rule 2: If you had an Email property
        RuleFor(x => x.CustomerDTO.Email)
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.CustomerDTO.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}