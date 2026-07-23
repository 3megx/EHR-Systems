using Mapster;

namespace EHRPlatform.Services.Billing.Application.Reports.Mappers;

/// <summary>
/// Mapster registration profile for Reports feature.
/// Handles conversion to report aggregate DTOs (OutstandingBalance, BillingReport).
/// Single Responsibility: Configure Reports-related type mappings only.
/// </summary>
public class ReportMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Invoice → InvoiceResponseDto (for reports)
        config.NewConfig<Invoice, InvoiceResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.InvoiceNumber, src => src.InvoiceNumber)
            .Map(dest => dest.PatientId, src => src.PatientId)
            .Map(dest => dest.AppointmentId, src => src.AppointmentId)
            .Map(dest => dest.ServiceDate, src => src.ServiceDate)
            .Map(dest => dest.DueDate, src => src.DueDate)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.SubTotal, src => src.SubTotal)
            .Map(dest => dest.TaxAmount, src => src.TaxAmount)
            .Map(dest => dest.InsuranceResponsibility, src => src.InsuranceResponsibility)
            .Map(dest => dest.PatientResponsibility, src => src.PatientResponsibility)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.AmountPaid, src => src.AmountPaid)
            .Map(dest => dest.BalanceDue, src => src.BalanceDue)
            .Map(dest => dest.InsuranceProvider, src => src.InsuranceProvider)
            .Map(dest => dest.InsurancePolicyNumber, src => src.InsurancePolicyNumber)
            .Map(dest => dest.Notes, src => src.Notes)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.LineItems, src => src.LineItems)
            .Map(dest => dest.Payments, src => src.Payments)
            .Map(dest => dest.Claims, src => src.InsuranceClaims);
    }
}
