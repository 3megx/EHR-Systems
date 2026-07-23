using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Features.Invoicing.Dtos.Responses;
using FluentValidation;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Commands;

/// <summary>
/// Create invoice command.
/// </summary>
public record CreateInvoiceCommand : ICommand<InvoiceResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public DateTime ServiceDate { get; init; }
    public List<LineItemRequest> LineItems { get; init; } = new();
    public string? InsuranceProvider { get; init; }
    public string? InsurancePolicyNumber { get; init; }
    public string? Notes { get; init; }
}

public class LineItemRequest
{
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.ServiceDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.LineItems).NotEmpty().WithMessage("At least one line item required");
        RuleForEach(x => x.LineItems).SetValidator(new LineItemValidator());
    }
}

public class LineItemValidator : AbstractValidator<LineItemRequest>
{
    public LineItemValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CPTCode).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThan(0);
    }
}

/// <summary>
/// Invoice response DTO.
/// </summary>
public record InvoiceCommandDto
{
    public Guid Id { get; set; }
}
