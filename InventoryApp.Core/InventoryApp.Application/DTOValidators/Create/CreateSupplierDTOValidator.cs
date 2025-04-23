using FluentValidation;
using InventoryApp.Application.DTOs.Create;

namespace InventoryApp.Application.DTOValidators.Create;

public class CreateSupplierDTOValidator : AbstractValidator<CreateSupplierDTO>
{
    public CreateSupplierDTOValidator()
    {
        RuleFor(s => s.Name)
            .NotNull()
            .WithMessage("Name cannot be null.")
            .Length(5,30)
            .WithMessage("Name must be between 5-30 characters.");
    }
}