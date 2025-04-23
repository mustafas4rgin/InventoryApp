using FluentValidation;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage("ID value cannot be null.")
            .GreaterThan(0)
            .WithMessage("ID value must be greater than zero.");

        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MaximumLength(15)
            .WithMessage("Category name must be less than 15 characters.");
    }
}