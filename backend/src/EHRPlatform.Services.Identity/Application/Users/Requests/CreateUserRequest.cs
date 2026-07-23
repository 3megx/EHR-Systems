namespace EHRPlatform.Services.Identity.Application.Users.Requests;

/// <summary>
/// Request DTO for creating a user.
/// </summary>
public class CreateUserRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
