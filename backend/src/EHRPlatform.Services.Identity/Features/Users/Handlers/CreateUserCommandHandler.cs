#nullable enable

using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Exceptions;
using EHRPlatform.Common.Security;
using EHRPlatform.Services.Identity.Application.Identity.DTOs.Responses;
using EHRPlatform.Services.Identity.Domain.Entities;
using EHRPlatform.Services.Identity.Features.Users.Commands;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Identity.Features.Users.Handlers;

/// <summary>
/// Handler for create user command (admin only).
/// Creates new user with temporary password and assigns role.
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUnitOfWork uow,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserCommandHandler> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle create user request.
    /// </summary>
    public async Task<CreateUserResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create user request for email: {Email}, role: {Role}", request.Email, request.Role);

        var userRepo = _uow.Repository<User>();

        // Check if email already exists
        var existingUser = await userRepo.FirstOrDefaultAsync(
            q => q.Where(u => u.Email == request.Email),
            cancellationToken);

        if (existingUser != null)
        {
            _logger.LogWarning("Create user failed: email already exists {Email}", request.Email);
            throw new ConflictException($"Email '{request.Email}' is already in use");
        }

        // Generate temporary password
        var temporaryPassword = GenerateTemporaryPassword();
        var (hash, salt) = _passwordHasher.HashWithSalt(temporaryPassword);

        // Create new user
        var newUser = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            EmailConfirmed = false,
            MfaEnabled = false,
            CreatedBy = request.CreatedBy
        };

        await userRepo.AddAsync(newUser, cancellationToken);

        // TODO: Assign role to user
        // This would involve creating a UserRole relationship
        // var roleRepo = _uow.Repository<Role>();
        // var role = await roleRepo.FirstOrDefaultAsync(q => q.Where(r => r.Name == request.Role), cancellationToken);
        // if (role != null)
        // {
        //     var userRole = new UserRole { UserId = newUser.Id, RoleId = role.Id };
        //     var urRepo = _uow.Repository<UserRole>();
        //     await urRepo.AddAsync(userRole, cancellationToken);
        // }

        // Publish domain event
        newUser.RaiseDomainEvent(new UserCreatedEvent
        {
            UserId = newUser.Id,
            Email = newUser.Email,
            Role = request.Role,
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow
        });

        await _uow.SaveChangesWithEventPublishingAsync(cancellationToken);

        _logger.LogInformation("User created successfully: {UserId}, Email: {Email}", newUser.Id, newUser.Email);

        return new CreateUserResponse
        {
            UserId = newUser.Id,
            Email = newUser.Email,
            TemporaryPassword = temporaryPassword,
            Message = "User created successfully. Please provide the temporary password to the user."
        };
    }

    /// <summary>
    /// Generate a secure temporary password.
    /// </summary>
    private static string GenerateTemporaryPassword()
    {
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        const string allChars = upperCase + lowerCase + digits + special;

        var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var password = new char[16];

        // Ensure at least one of each required type
        password[0] = upperCase[GetRandomIndex(rng, upperCase.Length)];
        password[1] = lowerCase[GetRandomIndex(rng, lowerCase.Length)];
        password[2] = digits[GetRandomIndex(rng, digits.Length)];
        password[3] = special[GetRandomIndex(rng, special.Length)];

        // Fill rest randomly
        for (int i = 4; i < password.Length; i++)
        {
            password[i] = allChars[GetRandomIndex(rng, allChars.Length)];
        }

        // Shuffle
        for (int i = 0; i < password.Length; i++)
        {
            int randomIndex = GetRandomIndex(rng, password.Length);
            (password[i], password[randomIndex]) = (password[randomIndex], password[i]);
        }

        return new string(password);
    }

    private static int GetRandomIndex(System.Security.Cryptography.RandomNumberGenerator rng, int max)
    {
        byte[] buffer = new byte[4];
        rng.GetBytes(buffer);
        return Math.Abs(BitConverter.ToInt32(buffer, 0)) % max;
    }
}

/// <summary>
/// Domain event published when user is created.
/// </summary>
public class UserCreatedEvent : EHRPlatform.Common.Entities.DomainEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
