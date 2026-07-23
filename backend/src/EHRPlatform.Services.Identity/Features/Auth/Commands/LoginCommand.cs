using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Identity.Features.Auth.Commands;

/// <summary>
/// Login command - authenticate user with email/password.
/// Returns JWT access token + refresh token on success.
/// </summary>
public record LoginCommand : ICommand<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? MfaCode { get; init; }
}

/// <summary>
/// Login command validator.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}

/// <summary>
/// Register command - create new user account.
/// </summary>
public record RegisterCommand : ICommand<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

/// <summary>
/// Register validator.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Password must contain digit");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords must match");
    }
}

/// <summary>
/// Refresh token command - get new JWT using refresh token.
/// </summary>
public record RefreshTokenCommand : ICommand<AuthResponseDto>
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// Logout command - revoke refresh tokens and end session.
/// </summary>
public record LogoutCommand : ICommand
{
    public Guid UserId { get; init; }
    public string? RefreshToken { get; init; }
}

/// <summary>
/// Authentication response with JWT tokens.
/// </summary>
public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public bool MfaRequired { get; set; }
}
