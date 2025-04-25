using FluentValidation;
using InventoryApp.Application.DTOs;

namespace InventoryApp.Application.DTOValidators;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(u => u.Email)
            .NotNull()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Must be a valid e-mail address.")
            .Length(4,50)
            .WithMessage("Email must be between 4-50 characters.");

        RuleFor(u => u.Password)
            .NotNull()
            .WithMessage("Password cannot be null.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.");
    }
}