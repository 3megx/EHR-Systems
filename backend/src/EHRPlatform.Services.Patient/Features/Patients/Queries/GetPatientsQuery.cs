using MediatR;

namespace EHRPlatform.Services.Patient.Features.Patients.Queries;

public class GetPatientsQuery : IRequest<IEnumerable<object>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
