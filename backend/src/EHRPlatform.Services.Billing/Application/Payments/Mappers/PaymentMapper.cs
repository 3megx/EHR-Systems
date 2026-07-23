using Mapster;
using EHRPlatform.Common.Mapping;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Billing.Application.Payments.Mappers;

/// <summary>
/// Payment Mapper
/// Single Responsibility: Convert between Payment domain model and DTOs.
/// Handles only Payments feature mappings.
/// </summary>
public class PaymentMapper : MappingServiceBase<Payment, PaymentResponseDto>
{
    public PaymentMapper(ILogger<PaymentMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single payment to response DTO.
    /// </summary>
    public PaymentResponseDto MapToResponseDto(Payment payment)
    {
        return MapToDto(payment);
    }

    /// <summary>
    /// Map collection of payments to response DTO list.
    /// </summary>
    public List<PaymentResponseDto> MapToResponseDtoList(ICollection<Payment> payments)
    {
        Logger.LogDebug("Mapping {Count} payments to response DTO list", payments.Count);
        return payments.Adapt<List<PaymentResponseDto>>();
    }
}
