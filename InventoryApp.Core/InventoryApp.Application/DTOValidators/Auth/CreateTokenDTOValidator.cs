using FluentValidation;
using InventoryApp.Application.DTOs;

namespace InventoryApp.Application.DTOValidators;

public class CreateTokenDTOValidator : AbstractValidator<CreateTokenDTO>
{
    public CreateTokenDTOValidator()
    {
        RuleFor(tdto => tdto.Token)
            .NotEmpty()
            .WithMessage("Token cannot be null.")
            .MinimumLength(30)
            .WithMessage("Token must be at least 30 characters.");

        RuleFor(tdto => tdto.UserId)
            .NotNull()
            .WithMessage("UserID value cannot be null")
            .GreaterThan(0)
            .WithMessage("UserID value must be greater than zero.");

        RuleFor(tdto => tdto.ExpiresAt)
            .NotNull()
            .WithMessage("Expire date cannot be null.")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Date cannot be now.");
    }
}