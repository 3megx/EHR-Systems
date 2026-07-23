using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Identity.Features.Users.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace EHRPlatform.Services.Identity.Features.Auth.Commands;

/// <summary>
/// Login command handler.
/// Validates credentials, creates JWT tokens, logs authentication.
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for user {Email}", command.Email);

        var userRepo = _unitOfWork.Repository<User>();

        // Find user by email
        var user = await userRepo.FirstOrDefaultAsync(
            q => q.Where(u => u.Email == command.Email),
            cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed: user {Email} not found", command.Email);
            throw new InvalidOperationException("Invalid credentials");
        }

        // Check if locked
        if (user.IsLocked())
        {
            throw new InvalidOperationException("Account is locked. Try again later.");
        }

        // Check if inactive
        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is inactive");
        }

        // Verify password
        if (!VerifyPassword(command.Password, user.PasswordHash, user.PasswordSalt))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.Lock();
            }
            await userRepo.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Login failed: invalid password for {Email}", command.Email);
            throw new InvalidOperationException("Invalid credentials");
        }

        // Check MFA
        if (user.MfaEnabled && string.IsNullOrEmpty(command.MfaCode))
        {
            // MFA required
            var response = new AuthResponseDto { MfaRequired = true };
            return response;
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LastLogin = DateTime.UtcNow;

        // Generate tokens
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken(user);

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();
        await refreshTokenRepo.AddAsync(refreshTokenEntity, cancellationToken);

        // Log successful login
        var loginAudit = new LoginAudit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Email = user.Email,
            Success = true,
            IpAddress = "127.0.0.1", // TODO: Get from HttpContext
            UserAgent = "unknown" // TODO: Get from HttpContext
        };

        var auditRepo = _unitOfWork.Repository<LoginAudit>();
        await auditRepo.AddAsync(loginAudit, cancellationToken);

        await userRepo.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login successful for user {Email}", command.Email);

        // Get user roles and permissions
        var roles = user.Roles.Select(r => r.Role.Name).ToList();
        var permissions = user.Roles
            .SelectMany(r => r.Role.Permissions.Select(p => p.Permission.Name))
            .ToList();

        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Roles = roles,
            Permissions = permissions
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = user.Roles.Select(r => r.Role.Name).ToList();
        var permissions = user.Roles
            .SelectMany(r => r.Role.Permissions.Select(p => p.Permission.Name))
            .ToList();

        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, user.Email),
            new(System.Security.Claims.ClaimTypes.GivenName, user.FirstName),
            new(System.Security.Claims.ClaimTypes.Surname, user.LastName)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new(System.Security.Claims.ClaimTypes.Role, role));
        }

        // Add permission claims
        foreach (var permission in permissions)
        {
            claims.Add(new("permission", permission));
        }

        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(User user)
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    private bool VerifyPassword(string password, string hash, string salt)
    {
        // TODO: Use proper password hashing (bcrypt, Argon2, etc.)
        // This is simplified for example
        var saltBytes = Convert.FromBase64String(salt);
        var rfc2898 = new System.Security.Cryptography.Rfc2898DeriveBytes(
            password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashBytes = rfc2898.GetBytes(32);
        var hashString = Convert.ToBase64String(hashBytes);
        return hashString == hash;
    }
}
