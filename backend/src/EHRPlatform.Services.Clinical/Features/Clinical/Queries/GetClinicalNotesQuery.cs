using MediatR;

namespace EHRPlatform.Services.Clinical.Features.Clinical.Queries;

public class GetClinicalNotesQuery : IRequest<IEnumerable<object>>
{
    public Guid PatientId { get; set; }
}
