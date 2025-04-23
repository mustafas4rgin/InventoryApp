using FluentValidation;
using InventoryApp.Domain.Entities;

namespace LibraryApp.Application.Validators;

public class RoleValidator : AbstractValidator<Role>
{
    public RoleValidator()
    {
        RuleFor(r => r.Id)
           .NotNull()
           .WithMessage("ID value cannot be null.")
           .GreaterThan(0)
           .WithMessage("ID value must be greater than zero.");

        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage("Name cannot be empty.")
            .Length(3,10)
            .WithMessage("Role must be between 3-10 characters");
    }
}