using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Clinical.Features.ClinicalNotes.Commands;

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
