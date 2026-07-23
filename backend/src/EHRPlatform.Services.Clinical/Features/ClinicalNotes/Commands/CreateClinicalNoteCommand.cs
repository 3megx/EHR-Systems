using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

/// <summary>
/// Create clinical note command.
/// Initializes SOAP note in draft status.
/// </summary>
public record CreateClinicalNoteCommand : ICommand<ClinicalNoteResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid ProviderId { get; init; }
    public DateTime EncounterDate { get; init; }
    public string EncounterType { get; init; } = string.Empty; // Office, Telehealth, Emergency, Hospital
}

public class CreateClinicalNoteCommandValidator : AbstractValidator<CreateClinicalNoteCommand>
{
    public CreateClinicalNoteCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ProviderId).NotEmpty();
        RuleFor(x => x.EncounterDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EncounterType).Must(t => new[] { "Office", "Telehealth", "Emergency", "Hospital" }.Contains(t));
    }
}

/// <summary>
/// Add diagnosis command.
/// </summary>
public record AddDiagnosisCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
    public string DiagnosisCode { get; init; } = string.Empty; // ICD-10
    public string DiagnosisText { get; init; } = string.Empty;
    public string DiagnosisType { get; init; } = "Secondary"; // Principal or Secondary
}

public class AddDiagnosisCommandValidator : AbstractValidator<AddDiagnosisCommand>
{
    public AddDiagnosisCommandValidator()
    {
        RuleFor(x => x.ClinicalNoteId).NotEmpty();
        RuleFor(x => x.DiagnosisCode).NotEmpty().Matches(@"^[A-Z][0-9]{2}(\.[0-9]{1,2})?$");
        RuleFor(x => x.DiagnosisText).NotEmpty();
        RuleFor(x => x.DiagnosisType).Must(t => new[] { "Principal", "Secondary" }.Contains(t));
    }
}

/// <summary>
/// Record vital signs command.
/// </summary>
public record RecordVitalsCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
    public decimal Temperature { get; init; } // Celsius
    public int SystolicBP { get; init; }
    public int DiastolicBP { get; init; }
    public int HeartRate { get; init; }
    public int RespiratoryRate { get; init; }
    public decimal? Weight { get; init; }
}

public class RecordVitalsCommandValidator : AbstractValidator<RecordVitalsCommand>
{
    public RecordVitalsCommandValidator()
    {
        RuleFor(x => x.ClinicalNoteId).NotEmpty();
        RuleFor(x => x.Temperature).GreaterThan(35).LessThan(42); // Normal range + fever
        RuleFor(x => x.SystolicBP).GreaterThan(60).LessThan(250);
        RuleFor(x => x.DiastolicBP).GreaterThan(40).LessThan(150);
        RuleFor(x => x.HeartRate).GreaterThan(20).LessThan(250);
        RuleFor(x => x.RespiratoryRate).GreaterThan(8).LessThan(60);
        RuleFor(x => x.Weight).GreaterThan(5).LessThan(500).When(x => x.Weight.HasValue);
    }
}

/// <summary>
/// Add procedure command.
/// </summary>
public record AddProcedureCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
    public string ProcedureName { get; init; } = string.Empty;
    public string ProcedureCode { get; init; } = string.Empty; // CPT or SNOMED
    public string Result { get; init; } = string.Empty;
}

public class AddProcedureCommandValidator : AbstractValidator<AddProcedureCommand>
{
    public AddProcedureCommandValidator()
    {
        RuleFor(x => x.ClinicalNoteId).NotEmpty();
        RuleFor(x => x.ProcedureName).NotEmpty();
        RuleFor(x => x.ProcedureCode).NotEmpty();
    }
}

/// <summary>
/// Update SOAP note command.
/// </summary>
public record UpdateSOAPCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
    public string? Subjective { get; init; }
    public string? Objective { get; init; }
    public string? Assessment { get; init; }
    public string? Plan { get; init; }
}

/// <summary>
/// Finalize clinical note command.
/// Locks note for editing, publishes event.
/// </summary>
public record FinalizeClinicalNoteCommand : ICommand
{
    public Guid ClinicalNoteId { get; init; }
}

/// <summary>
/// Clinical note response DTO.
/// </summary>
public class ClinicalNoteResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public List<VitalSignsDto> VitalSigns { get; set; } = new();
    public List<DiagnosisDto> Diagnoses { get; set; } = new();
    public List<ProcedureDto> Procedures { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class VitalSignsDto
{
    public Guid Id { get; set; }
    public DateTime RecordedAt { get; set; }
    public decimal Temperature { get; set; }
    public int SystolicBP { get; set; }
    public int DiastolicBP { get; set; }
    public int HeartRate { get; set; }
    public int RespiratoryRate { get; set; }
    public decimal? Weight { get; set; }
}

public class DiagnosisDto
{
    public Guid Id { get; set; }
    public string DiagnosisCode { get; set; } = string.Empty;
    public string DiagnosisText { get; set; } = string.Empty;
    public string DiagnosisType { get; set; } = string.Empty;
}

public class ProcedureDto
{
    public Guid Id { get; set; }
    public string ProcedureName { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string Result { get; set; } = string.Empty;
}
