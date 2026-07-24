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
/// Handler for change password command.
/// Verifies current password and updates to new secure password.
/// Publishes event for audit logging.
/// </summary>
public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle password change request.
    /// </summary>
    public async Task<ChangePasswordResponse> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password change request for user: {UserId}", request.UserId);

        var userRepo = _uow.Repository<User>();
        var user = await userRepo.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        // Verify current password
        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Password change failed: invalid current password for user: {UserId}", request.UserId);
            throw new UnauthorizedException("Current password is incorrect");
        }

        // Ensure new password is different
        if (request.CurrentPassword == request.NewPassword)
        {
            throw new ValidationException("New password must be different from current password");
        }

        // Hash new password
        var (newHash, newSalt) = _passwordHasher.HashWithSalt(request.NewPassword);

        // Update user
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;
        user.UpdatedBy = request.UserId;

        // Publish domain event
        user.RaiseDomainEvent(new PasswordChangedEvent
        {
            UserId = user.Id,
            ChangedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow
        });

        await _uow.SaveChangesWithEventPublishingAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

        return new ChangePasswordResponse
        {
            Message = "Password changed successfully",
            UpdatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Domain event published when password is changed.
/// </summary>
public class PasswordChangedEvent : EHRPlatform.Common.Entities.DomainEvent
{
    public Guid UserId { get; set; }
    public DateTime ChangedAt { get; set; }
}
