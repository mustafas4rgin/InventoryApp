using FluentValidation;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .NotNull()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Must be a valid e-mail address.")
            .Length(4,50)
            .WithMessage("Email must be between 4-50 characters.");

        RuleFor(u => u.FirstName)
            .NotNull()
            .WithMessage("First name cannot be empty.")
            .Length(5,30)
            .WithMessage("First name must be between 5-30 characters.");

        RuleFor(u => u.LastName)
            .NotNull()
            .WithMessage("Last name cannot be empty.")
            .Length(5,30)
            .WithMessage("Last name must be between 5-30 characters.");

        RuleFor(u => u.SupplierId)
            .NotNull()
            .WithMessage("Supplier ID cannot be null")
            .GreaterThan(0)
            .WithMessage("Supplier ID must be greater than zero.");

        RuleFor(u => u.RoleId)
            .NotNull()
            .WithMessage("Role ID cannot be null.")
            .GreaterThan(0)
            .WithMessage("Role ID value must be greater than zero.");
    }
}