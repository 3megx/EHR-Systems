using MediatR;
using EHRPlatform.Services.Audit.Features.Audit.Queries;

namespace EHRPlatform.Services.Audit.Features.Audit.Handlers;

/// <summary>
/// Handler for GetAuditEntriesQuery.
/// </summary>
public class GetAuditEntriesHandler : IRequestHandler<GetAuditEntriesQuery, IEnumerable<object>>
{
    public Task<IEnumerable<object>> Handle(GetAuditEntriesQuery request, CancellationToken cancellationToken)
    {
        // Implementation would go here
        throw new NotImplementedException();
    }
}
