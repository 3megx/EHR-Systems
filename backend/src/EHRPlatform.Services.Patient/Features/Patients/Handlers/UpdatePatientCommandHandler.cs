using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Common.Messaging;
using EHRPlatform.Services.Patient.Features.Patients.Commands;
using EHRPlatform.Services.Patient.Application.Patients.Responses;
using EHRPlatform.Services.Patient.Application.Patients.Mappers;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Patient.Features.Patients.Handlers;

/// <summary>
/// Update patient command handler.
/// Updates patient demographics and contact information.
/// </summary>
public class UpdatePatientCommandHandler : ICommandHandler<UpdatePatientCommand, PatientResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxRepository _outbox;
    private readonly PatientMapper _mapper;
    private readonly ILogger<UpdatePatientCommandHandler> _logger;

    public UpdatePatientCommandHandler(
        IUnitOfWork unitOfWork,
        IOutboxRepository outbox,
        PatientMapper mapper,
        ILogger<UpdatePatientCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger;
    }

    public async Task<PatientResponse> Handle(UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating patient {PatientId}", command.PatientId);

        var repo = _unitOfWork.Repository<Domain.Entities.Patient>();
        var patient = await repo.GetByIdAsync(command.PatientId, cancellationToken);

        if (patient == null)
            throw new KeyNotFoundException($"Patient {command.PatientId} not found");

        // Update fields
        patient.FirstName = command.FirstName ?? patient.FirstName;
        patient.LastName = command.LastName ?? patient.LastName;
        patient.Email = command.Email ?? patient.Email;
        patient.PhoneNumber = command.PhoneNumber ?? patient.PhoneNumber;
        patient.Gender = command.Gender ?? patient.Gender;
        patient.BloodType = command.BloodType ?? patient.BloodType;
        patient.EmergencyContact = command.EmergencyContact ?? patient.EmergencyContact;
        patient.EmergencyPhone = command.EmergencyPhone ?? patient.EmergencyPhone;

        await repo.UpdateAsync(patient, cancellationToken);

        // Publish event
        var updatedEvent = new PatientUpdatedEvent(
            patient.Id, patient.FirstName, patient.LastName, patient.Email);

        await _outbox.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            AggregateId = patient.Id,
            EventType = nameof(PatientUpdatedEvent),
            EventData = System.Text.Json.JsonSerializer.Serialize(updatedEvent),
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Patient updated {PatientId}", patient.Id);

        return _mapper.MapToResponse(patient);
    }
}
