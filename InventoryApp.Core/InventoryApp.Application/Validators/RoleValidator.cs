using FluentValidation;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Validators;

public class RoleValidator : AbstractValidator<Role>
{
    public RoleValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage("Name cannot be empty.")
            .Length(3,10)
            .WithMessage("Role must be between 3-10 characters");
    }
}