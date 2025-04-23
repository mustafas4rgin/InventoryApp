using FluentValidation;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Validators;

public class SupplierValidator : AbstractValidator<Supplier>
{
    public SupplierValidator()
    {
        
        RuleFor(s => s.Name)
            .NotNull()
            .WithMessage("Name cannot be null.")
            .Length(5,30)
            .WithMessage("Name must be between 5-30 characters.");

    }
}