using MediatR;

namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Queries;

public class GetPrescriptionsQuery : IRequest<IEnumerable<object>>
{
    public Guid PatientId { get; set; }
}
