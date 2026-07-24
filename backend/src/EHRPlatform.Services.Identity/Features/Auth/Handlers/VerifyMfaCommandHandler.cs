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
/// Handler for MFA verification command.
/// Verifies TOTP code and enables MFA for user account.
/// </summary>
public class VerifyMfaCommandHandler : ICommandHandler<VerifyMfaCommand, VerifyMfaResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<VerifyMfaCommandHandler> _logger;

    public VerifyMfaCommandHandler(
        IUnitOfWork uow,
        IEncryptionService encryptionService,
        ILogger<VerifyMfaCommandHandler> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle MFA verification request.
    /// </summary>
    public async Task<VerifyMfaResponse> Handle(
        VerifyMfaCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("MFA verification attempt for user: {UserId}", request.UserId);

        var userRepo = _uow.Repository<User>();
        var user = await userRepo.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        if (string.IsNullOrEmpty(user.MfaSecret))
        {
            _logger.LogWarning("MFA verification attempt but MFA not setup for user: {UserId}", request.UserId);
            throw new BusinessRuleException("MFA has not been configured for this account");
        }

        // Decrypt and verify TOTP secret
        var totpSecret = _encryptionService.Decrypt(user.MfaSecret);

        // TODO: Verify TOTP code with tolerance window (30 seconds before/after)
        // For now, accept any 6-digit code as proof of concept
        if (!ValidateTotpCode(request.Code, totpSecret))
        {
            _logger.LogWarning("Invalid TOTP code for user: {UserId}", request.UserId);
            throw new ValidationException("Invalid verification code");
        }

        // Enable MFA
        user.MfaEnabled = true;
        user.UpdatedBy = request.UserId;

        // Publish domain event
        user.RaiseDomainEvent(new MfaEnabledEvent
        {
            UserId = user.Id,
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow
        });

        await _uow.SaveChangesWithEventPublishingAsync(cancellationToken);

        _logger.LogInformation("MFA enabled for user: {UserId}", request.UserId);

        return new VerifyMfaResponse
        {
            Success = true,
            Message = "Multi-factor authentication has been enabled successfully"
        };
    }

    /// <summary>
    /// Validate TOTP code against secret.
    /// </summary>
    private static bool ValidateTotpCode(string code, string secret)
    {
        // TODO: Implement proper TOTP validation using RFC 4226/6238
        // For now, just check format
        if (string.IsNullOrEmpty(code) || code.Length != 6)
        {
            return false;
        }

        return int.TryParse(code, out _);
    }
}

/// <summary>
/// Domain event published when MFA is enabled.
/// </summary>
public class MfaEnabledEvent : EHRPlatform.Common.Entities.DomainEvent
{
    public Guid UserId { get; set; }
}
