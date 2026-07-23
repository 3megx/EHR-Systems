using MediatR;
using EHRPlatform.Services.Patient.Features.Patients.Queries;

namespace EHRPlatform.Services.Patient.Features.Patients.Handlers;

public class GetPatientsHandler : IRequestHandler<GetPatientsQuery, IEnumerable<object>>
{
    public Task<IEnumerable<object>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
