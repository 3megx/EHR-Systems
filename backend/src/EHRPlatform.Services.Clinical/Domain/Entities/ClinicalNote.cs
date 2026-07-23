using EHRPlatform.Common.Entities;
using EHRPlatform.Common.Events;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Domain;

/// <summary>
/// Clinical note aggregate - SOAP format (Subjective, Objective, Assessment, Plan).
/// </summary>
public class ClinicalNote : AuditableEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty; // Office, Telehealth, Emergency, Hospital
    public string Status { get; set; } = "Draft"; // Draft, Finalized, Locked

    // SOAP components
    public string Subjective { get; set; } = string.Empty; // Patient complaint, symptoms
    public string Objective { get; set; } = string.Empty; // Physical exam, observations, lab results
    public string Assessment { get; set; } = string.Empty; // Diagnosis, impression
    public string Plan { get; set; } = string.Empty; // Treatment, medications, follow-up

    // Collections
    public ICollection<VitalSigns> VitalSigns { get; } = new List<VitalSigns>();
    public ICollection<ClinicalDiagnosis> Diagnoses { get; } = new List<ClinicalDiagnosis>();
    public ICollection<ClinicalProcedure> Procedures { get; } = new List<ClinicalProcedure>();

    private readonly List<IntegrationEvent> _domainEvents = new();

    public void AddDiagnosis(string diagnosisCode, string diagnosisText, string type)
    {
        var diagnosis = new ClinicalDiagnosis
        {
            Id = Guid.NewGuid(),
            ClinicalNoteId = Id,
            DiagnosisCode = diagnosisCode,
            DiagnosisText = diagnosisText,
            DiagnosisType = type // Principal, Secondary
        };
        Diagnoses.Add(diagnosis);

        RaiseEvent(new DiagnosisRecordedEvent(Id, PatientId, diagnosisCode, diagnosisText));
    }

    public void RecordVitals(decimal temperature, int systolic, int diastolic, int heartRate, int respiratoryRate, decimal? weight = null)
    {
        var vitals = new VitalSigns
        {
            Id = Guid.NewGuid(),
            ClinicalNoteId = Id,
            RecordedAt = DateTime.UtcNow,
            Temperature = temperature,
            SystolicBP = systolic,
            DiastolicBP = diastolic,
            HeartRate = heartRate,
            RespiratoryRate = respiratoryRate,
            Weight = weight
        };
        VitalSigns.Add(vitals);

        RaiseEvent(new VitalSignsRecordedEvent(Id, PatientId, systolic, diastolic, heartRate));
    }

    public void AddProcedure(string procedureName, string procedureCode, string result = "")
    {
        var procedure = new ClinicalProcedure
        {
            Id = Guid.NewGuid(),
            ClinicalNoteId = Id,
            ProcedureName = procedureName,
            ProcedureCode = procedureCode,
            Result = result,
            PerformedAt = DateTime.UtcNow
        };
        Procedures.Add(procedure);

        RaiseEvent(new ProcedurePerformedEvent(Id, PatientId, procedureName, procedureCode));
    }

    public void Finalize()
    {
        if (Status != "Draft")
            throw new InvalidOperationException("Only draft notes can be finalized");

        Status = "Finalized";
        RaiseEvent(new ClinicalNoteCompletedEvent(Id, PatientId, EncounterDate));
    }

    public void RaiseEvent(IntegrationEvent @event) => _domainEvents.Add(@event);
    public IReadOnlyList<IntegrationEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Vital signs measurement.
/// </summary>
public class VitalSigns : BaseEntity
{
    public Guid ClinicalNoteId { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Temperature { get; set; } // Celsius
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; } // kg
    public ClinicalNote ClinicalNote { get; set; } = null!;

    public string GetBloodPressure() => $"{SystolicBP}/{DiastolicBP}";
}

/// <summary>
/// Clinical diagnosis (ICD-10).
/// </summary>
public class ClinicalDiagnosis : BaseEntity
{
    public Guid ClinicalNoteId { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty; // ICD-10 code
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty; // Principal, Secondary
    public ClinicalNote ClinicalNote { get; set; } = null!;
}

/// <summary>
/// Clinical procedure performed.
/// </summary>
public class ClinicalProcedure : BaseEntity
{
    public Guid ClinicalNoteId { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty; // CPT or SNOMED code
    public DateTime PerformedAt { get; set; }
    public string Result { get; set; } = string.Empty;
    public ClinicalNote ClinicalNote { get; set; } = null!;
}

/// <summary>
/// Domain events.
/// </summary>
public record DiagnosisRecordedEvent : IntegrationEvent
{
    public Guid ClinicalNoteId { get; set; }
    public Guid PatientId { get; set; }
    public string DiagnosisCode { get; set; }
    public string DiagnosisText { get; set; }

    public DiagnosisRecordedEvent(Guid noteId, Guid patientId, string code, string text)
    {
        ClinicalNoteId = noteId;
        PatientId = patientId;
        DiagnosisCode = code;
        DiagnosisText = text;
    }
}

public record VitalSignsRecordedEvent : IntegrationEvent
{
    public Guid ClinicalNoteId { get; set; }
    public Guid PatientId { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }

    public VitalSignsRecordedEvent(Guid noteId, Guid patientId, int systolic, int diastolic, int hr)
    {
        ClinicalNoteId = noteId;
        PatientId = patientId;
        SystolicBP = systolic;
        DiastolicBP = diastolic;
        HeartRate = hr;
    }
}

public record ProcedurePerformedEvent : IntegrationEvent
{
    public Guid ClinicalNoteId { get; set; }
    public Guid PatientId { get; set; }
    public string ProcedureName { get; set; }
    public string ProcedureCode { get; set; }

    public ProcedurePerformedEvent(Guid noteId, Guid patientId, string name, string code)
    {
        ClinicalNoteId = noteId;
        PatientId = patientId;
        ProcedureName = name;
        ProcedureCode = code;
    }
}

public record ClinicalNoteCompletedEvent : IntegrationEvent
{
    public Guid ClinicalNoteId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime EncounterDate { get; set; }

    public ClinicalNoteCompletedEvent(Guid noteId, Guid patientId, DateTime encounterDate)
    {
        ClinicalNoteId = noteId;
        PatientId = patientId;
        EncounterDate = encounterDate;
    }
}
