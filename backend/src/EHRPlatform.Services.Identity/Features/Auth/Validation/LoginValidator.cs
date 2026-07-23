using FluentValidation;
using EHRPlatform.Services.Identity.Features.Auth.Commands;

namespace EHRPlatform.Services.Identity.Features.Auth.Validation;

/// <summary>
/// Validator for LoginCommand.
/// </summary>
public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}

public class LoginCommand
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
