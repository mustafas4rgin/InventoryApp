using FluentValidation;
using InventoryApp.Application.DTOs.Update;

namespace InventoryApp.Application.DTOValidators.Update;

public class UpdateSupplierDTOValidator : AbstractValidator<UpdateSupplierDTO>
{
    public UpdateSupplierDTOValidator()
    {
        RuleFor(s => s.Name)
            .NotNull()
            .WithMessage("Name cannot be null.")
            .Length(5,30)
            .WithMessage("Name must be between 5-30 characters.");
    }
}