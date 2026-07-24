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
/// Handler for refresh token command.
/// Validates existing refresh token and issues new access token.
/// </summary>
public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle token refresh request.
    /// </summary>
    public async Task<RefreshTokenResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token refresh request");

        try
        {
            // TODO: Validate access token format and extract claims
            // For now, we'll validate based on refresh token lookup
            
            var rtRepo = _uow.Repository<RefreshToken>();
            var userRepo = _uow.Repository<User>();

            // Find refresh token - NOTE: In real implementation, should validate against hashed token
            var refreshTokenEntity = await rtRepo.FirstOrDefaultAsync(
                q => q.Where(rt => rt.Token == request.RefreshToken && rt.ExpiresAt > DateTime.UtcNow),
                cancellationToken)
                ?? throw new UnauthorizedException("Invalid or expired refresh token");

            // Get user
            var user = await userRepo.GetByIdAsync(refreshTokenEntity.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(User), refreshTokenEntity.UserId);

            if (!user.IsActive)
            {
                throw new UnauthorizedException("User account is inactive");
            }

            // Generate new access token
            var newAccessToken = GenerateAccessToken(user);

            _logger.LogInformation("Token refreshed for user: {UserId}", user.Id);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                ExpiresIn = 3600,
                TokenType = "Bearer"
            };
        }
        catch (UnauthorizedException)
        {
            _logger.LogWarning("Invalid refresh token attempt");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    /// <summary>
    /// Generate JWT access token with user claims.
    /// </summary>
    private static string GenerateAccessToken(User user)
    {
        // TODO: Implement JWT token generation with user claims
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
    }
}
