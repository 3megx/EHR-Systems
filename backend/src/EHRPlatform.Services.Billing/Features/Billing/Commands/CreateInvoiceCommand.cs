using EHRPlatform.Common.CQRS;
using FluentValidation;

namespace EHRPlatform.Services.Billing.Features.Billing.Commands;

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
/// Record payment command.
/// </summary>
public record RecordPaymentCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public string Method { get; init; } = string.Empty; // Credit Card, Check, ACH, Insurance
    public string? Reference { get; init; }
}

/// <summary>
/// Submit to insurance command.
/// </summary>
public record SubmitToInsuranceCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public string InsuranceProvider { get; init; } = string.Empty;
    public string PolicyNumber { get; init; } = string.Empty;
}

/// <summary>
/// Cancel invoice command.
/// </summary>
public record CancelInvoiceCommand : ICommand
{
    public Guid InvoiceId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Invoice response DTO.
/// </summary>
public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<LineItemDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public List<ClaimDto> Claims { get; set; } = new();
}

public class LineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}

public class ClaimDto
{
    public Guid Id { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ApprovedAmount { get; set; }
}
