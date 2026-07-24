#nullable enable

using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Exceptions;
using EHRPlatform.Common.Security;
using EHRPlatform.Services.Identity.Application.Identity.DTOs.Responses;
using EHRPlatform.Services.Identity.Domain.Entities;
using EHRPlatform.Services.Identity.Features.Auth.Commands;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Identity.Features.Auth.Handlers;

/// <summary>
/// Handler for user login command.
/// Validates credentials, generates JWT token, and handles MFA requirement.
/// HIPAA-compliant with audit logging.
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        IEncryptionService encryptionService,
        ILogger<LoginCommandHandler> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle login request by validating credentials and generating tokens.
    /// </summary>
    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var userRepo = _uow.Repository<User>();
        var user = await userRepo.FirstOrDefaultAsync(
            q => q.Where(u => u.Email == request.Email && u.IsActive),
            cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password");

        // Check if account is locked
        if (user.IsLocked())
        {
            _logger.LogWarning("Login attempt on locked account for email: {Email}", request.Email);
            throw new UnauthorizedException("Account is temporarily locked due to multiple failed attempts");
        }

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.Lock();
                _logger.LogWarning("Account locked after 5 failed attempts: {Email}", request.Email);
            }

            await _uow.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Invalid email or password");
        }

        // If MFA is enabled, don't generate tokens yet
        if (user.MfaEnabled)
        {
            _logger.LogInformation("MFA required for user: {UserId}", user.Id);
            return new LoginResponse
            {
                MfaRequired = true,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                ExpiresIn = 0
            };
        }

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Create and store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = _passwordHasher.Hash(refreshToken, ""),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedBy = user.Id
        };

        var rtRepo = _uow.Repository<RefreshToken>();
        await rtRepo.AddAsync(refreshTokenEntity, cancellationToken);

        // Update user login info
        user.LastLogin = DateTime.UtcNow;
        user.FailedLoginAttempts = 0;
        user.UpdatedBy = user.Id;

        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login successful for user: {UserId}", user.Id);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            MfaRequired = false
        };
    }

    /// <summary>
    /// Generate JWT access token with user claims.
    /// </summary>
    private static string GenerateAccessToken(User user)
    {
        // TODO: Implement JWT token generation with claims
        // Should include: user ID, email, roles, permissions, subject, issued at, expiration
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
    }

    /// <summary>
    /// Generate secure random refresh token.
    /// </summary>
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    }
}
