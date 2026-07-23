using MediatR;
using EHRPlatform.Services.Analytics.Features.Analytics.Queries;

namespace EHRPlatform.Services.Analytics.Features.Analytics.Handlers;

/// <summary>
/// Handler for GetDashboardsQuery.
/// </summary>
public class GetDashboardsHandler : IRequestHandler<GetDashboardsQuery, IEnumerable<object>>
{
    public Task<IEnumerable<object>> Handle(GetDashboardsQuery request, CancellationToken cancellationToken)
    {
        // Implementation would go here
        throw new NotImplementedException();
    }
}
