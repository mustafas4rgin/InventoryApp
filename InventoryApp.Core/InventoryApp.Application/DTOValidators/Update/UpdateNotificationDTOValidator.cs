using FluentValidation;
using InventoryApp.Application.DTOs.Update;

namespace InventoryApp.Application.DTOValidators.Update;

public class UpdateNotificationDTOValidator : AbstractValidator<UpdateNotificationDTO>
{
    public UpdateNotificationDTOValidator()
    {
        RuleFor(n => n.UserId)
            .NotNull()
            .WithMessage("UserID cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserID must be greater than zero.");

        RuleFor(n => n.Message)
            .NotNull()
            .WithMessage("Message field cannot be empty.")
            .Length(5,500)
            .WithMessage("Message must be between 5-500 characters");

        RuleFor(n => n.Title)
            .NotNull()
            .WithMessage("Title cannot be empty.")
            .Length(5,100)
            .WithMessage("Title must be between 5-100 characters.");
    }
}