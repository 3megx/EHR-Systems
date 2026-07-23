using MediatR;
using EHRPlatform.Services.Prescription.Features.Prescriptions.Commands;

namespace EHRPlatform.Services.Prescription.Features.Prescriptions.Handlers;

public class CreatePrescriptionHandler : IRequestHandler<CreatePrescriptionCommand, object>
{
    public Task<object> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class CreatePrescriptionCommand : IRequest<object>
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? MedicationName { get; set; }
}
