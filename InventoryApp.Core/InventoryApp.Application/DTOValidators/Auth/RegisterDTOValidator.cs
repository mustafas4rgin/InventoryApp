using FluentValidation;
using InventoryApp.Application.DTOs;

namespace InventoryApp.Application.DTOValidators;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
    {
        RuleFor(u => u.Email)
            .NotNull()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Must be a valid e-mail address.")
            .Length(4, 50)
            .WithMessage("Email must be between 4-50 characters.");

        RuleFor(u => u.FirstName)
            .NotNull()
            .WithMessage("First name cannot be empty.")
            .Length(5, 30)
            .WithMessage("First name must be between 5-30 characters.");

        RuleFor(u => u.LastName)
            .NotNull()
            .WithMessage("Last name cannot be empty.")
            .Length(5, 30)
            .WithMessage("Last name must be between 5-30 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalı.")
            .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermeli.")
            .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermeli.")
            .Matches(@"\d+").WithMessage("Şifre en az bir rakam içermeli.")
            .Matches(@"[\!\@\#\$\%\^\&\*\(\)\-\+\=]+").WithMessage("Şifre en az bir özel karakter içermeli.");

        RuleFor(u => u.PasswordMatch)
            .NotEmpty().WithMessage("Password match is required.")
            .Must((user, passwordMatch) => passwordMatch == user.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(u => u.SupplierId)
            .NotNull()
            .WithMessage("Supplier ID cannot be null")
            .GreaterThan(0)
            .WithMessage("Supplier ID must be greater than zero.");

        RuleFor(u => u.RoleId)
            .NotNull()
            .WithMessage("Role ID cannot be null.")
            .GreaterThan(0)
            .WithMessage("Role ID value must be greater than zero.");
    }
}