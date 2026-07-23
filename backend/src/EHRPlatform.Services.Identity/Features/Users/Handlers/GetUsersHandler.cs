using MediatR;
using EHRPlatform.Services.Identity.Features.Users.Queries;

namespace EHRPlatform.Services.Identity.Features.Users.Handlers;

/// <summary>
/// Handler for GetUsersQuery.
/// </summary>
public class GetUsersHandler : IRequestHandler<GetUsersQuery, IEnumerable<object>>
{
    public Task<IEnumerable<object>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Implementation would go here
        throw new NotImplementedException();
    }
}
