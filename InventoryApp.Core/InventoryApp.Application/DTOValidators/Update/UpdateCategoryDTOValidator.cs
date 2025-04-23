using FluentValidation;
using InventoryApp.Application.DTOs.Update;

namespace InventoryApp.Application.DTOValidators.Update;

public class UpdateCategoryDTOValidator : AbstractValidator<UpdateCategoryDTO>
{
    public UpdateCategoryDTOValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .MaximumLength(15)
            .WithMessage("Category name must be less than 15 characters.");
    }
}