using FluentValidation;
using InventoryApp.Application.DTOs.Create;

namespace InventoryApp.Application.DTOValidators.Create;

public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
{
    public CreateCategoryDTOValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MaximumLength(15)
            .WithMessage("Category name must be less than 15 characters.");
    }
}