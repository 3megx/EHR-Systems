using Mapster;

namespace EHRPlatform.Services.Billing.Application.Claims.Mappers;

/// <summary>
/// Mapster registration profile for Claims feature.
/// Handles conversion between InsuranceClaim domain model and DTOs.
/// Single Responsibility: Configure Claims-related type mappings only.
/// </summary>
public class ClaimMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // InsuranceClaim → ClaimResponseDto
        config.NewConfig<InsuranceClaim, ClaimResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.InvoiceId, src => src.InvoiceId)
            .Map(dest => dest.InsuranceProvider, src => src.InsuranceProvider)
            .Map(dest => dest.ClaimNumber, src => src.ClaimNumber)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.SubmittedAt, src => src.SubmittedAt)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);

        // ClaimResponseDto → InsuranceClaim (for updates/inserts)
        config.NewConfig<ClaimResponseDto, InsuranceClaim>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.InvoiceId, src => src.InvoiceId)
            .Map(dest => dest.InsuranceProvider, src => src.InsuranceProvider)
            .Map(dest => dest.ClaimNumber, src => src.ClaimNumber)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.SubmittedAt, src => src.SubmittedAt);
    }
}
