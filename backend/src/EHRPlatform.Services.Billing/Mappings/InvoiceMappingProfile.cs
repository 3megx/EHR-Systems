using Mapster;
using EHRPlatform.Services.Billing.Domain;
using EHRPlatform.Services.Billing.Features.Invoicing.Dtos.Responses;
using EHRPlatform.Services.Billing.Features.Reports.Dtos.Responses;

namespace EHRPlatform.Services.Billing.Mappings;

/// <summary>
/// Mapster registration profile for Invoice entity mappings.
/// Handles conversion between domain models and DTOs.
/// Single Responsibility: Configure all Invoice-related type mappings.
/// </summary>
public class InvoiceMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Invoice → InvoiceResponseDto
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

        // LineItem → LineItemDto
        config.NewConfig<LineItem, LineItemDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CPTCode, src => src.CPTCode)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice)
            .Map(dest => dest.Amount, src => src.Amount);

        // Payment → PaymentDto
        config.NewConfig<Payment, PaymentDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Method, src => src.Method)
            .Map(dest => dest.Reference, src => src.Reference)
            .Map(dest => dest.ReceivedAt, src => src.ReceivedAt);

        // InsuranceClaim → InsuranceClaimDto
        config.NewConfig<InsuranceClaim, InsuranceClaimDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.InsuranceProvider, src => src.InsuranceProvider)
            .Map(dest => dest.ClaimNumber, src => src.ClaimNumber)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Amount, src => src.Amount);

        // InvoiceResponseDto → Invoice (for updates/inserts)
        config.NewConfig<InvoiceResponseDto, Invoice>()
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
            .Map(dest => dest.InsuranceProvider, src => src.InsuranceProvider)
            .Map(dest => dest.InsurancePolicyNumber, src => src.InsurancePolicyNumber)
            .Map(dest => dest.Notes, src => src.Notes);
    }
}
