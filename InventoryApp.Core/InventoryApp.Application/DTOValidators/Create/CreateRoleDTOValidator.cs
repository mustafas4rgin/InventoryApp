using FluentValidation;
using InventoryApp.Application.DTOs.Create;

namespace InventoryApp.Application.DTOValidators.Create;

public class CreateRoleDTOValidator : AbstractValidator<CreateRoleDTO>
{
    public CreateRoleDTOValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage("Name cannot be empty.")
            .Length(3,10)
            .WithMessage("Role must be between 3-10 characters");
    }
}