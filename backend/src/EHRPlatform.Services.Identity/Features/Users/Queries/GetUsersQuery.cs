using MediatR;

namespace EHRPlatform.Services.Identity.Features.Users.Queries;

/// <summary>
/// Query to get users.
/// </summary>
public class GetUsersQuery : IRequest<IEnumerable<object>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
