using FluentValidation;
using InventoryApp.Application.DTOs.Update;

namespace InventoryApp.Application.DTOValidators.Update;

public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
{
    public UpdateProductDTOValidator()
    {
        RuleFor(p => p.CategoryId)
            .NotNull()
            .WithMessage("CategoryID cannot be null.")
            .GreaterThan(0)
            .WithMessage("CategoryID must be greater than zero.");

        RuleFor(p => p.SupplierId)
            .NotNull()
            .WithMessage("SupplierID cannot be null.")
            .GreaterThan(0)
            .WithMessage("SupplierID must be greater than zero.");

        RuleFor(p => p.Name)
            .NotNull()
            .WithMessage("Name cannot be null.")
            .Length(5,50)
            .WithMessage("Name must be between 5-50 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(p => p.Stock)
            .GreaterThan(0)
            .WithMessage("Stock must be greater than zero.");
    }
}