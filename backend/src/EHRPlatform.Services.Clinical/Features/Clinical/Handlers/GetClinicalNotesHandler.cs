using MediatR;
using EHRPlatform.Services.Clinical.Features.Clinical.Queries;

namespace EHRPlatform.Services.Clinical.Features.Clinical.Handlers;

public class GetClinicalNotesHandler : IRequestHandler<GetClinicalNotesQuery, IEnumerable<object>>
{
    public Task<IEnumerable<object>> Handle(GetClinicalNotesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
