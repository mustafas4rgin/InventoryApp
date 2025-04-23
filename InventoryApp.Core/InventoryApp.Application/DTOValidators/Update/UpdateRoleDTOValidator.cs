using FluentValidation;
using LibraryApp.Application.DTOs.Update;

namespace InventoryApp.Application.DTOValidators.Update;

public class UpdateRoleDTOValidator : AbstractValidator<UpdateRoleDTO>
{
    public UpdateRoleDTOValidator()
    {
        RuleFor(r => r.Name)
            .NotNull()
            .WithMessage("Name cannot be empty.")
            .Length(3,10)
            .WithMessage("Role must be between 3-10 characters");
    }
}