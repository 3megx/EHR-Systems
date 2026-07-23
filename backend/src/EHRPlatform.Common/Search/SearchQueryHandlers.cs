using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Caching;

namespace EHRPlatform.Common.Search;

/// <summary>
/// Base search query for entities.
/// Automatically cached and searchable via Elasticsearch.
/// </summary>
public record SearchEntitiesQuery : IQuery<SearchResult<Dictionary<string, object>>>, ICachedQuery
{
    public string? QueryText { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public List<(string field, string value)>? Filters { get; init; }
    public string EntityType { get; init; } = "Patient";

    public string CacheKey => $"search:{EntityType}:{QueryText}:{PageNumber}:{PageSize}".ToLower();
    public TimeSpan? Duration => TimeSpan.FromMinutes(10); // Search results cached longer
}

/// <summary>
/// Handler for search queries.
/// Delegates to ISearchService for Elasticsearch operations.
/// Results automatically cached via CachingBehavior.
/// </summary>
public class SearchEntitiesQueryHandler : 
    IQueryHandler<SearchEntitiesQuery, SearchResult<Dictionary<string, object>>>
{
    private readonly ISearchService _searchService;

    public SearchEntitiesQueryHandler(ISearchService searchService)
    {
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public async Task<SearchResult<Dictionary<string, object>>> Handle(
        SearchEntitiesQuery query,
        CancellationToken cancellationToken)
    {
        var searchQuery = new SearchQuery
        {
            QueryText = query.QueryText,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            HighlightResults = true
        };

        return await _searchService.SearchAsync<Dictionary<string, object>>(
            searchQuery,
            cancellationToken);
    }
}

/// <summary>
/// Search patients by name, email, or MRN.
/// Cached for 10 minutes.
/// </summary>
public record SearchPatientsQuery : IQuery<SearchResult<PatientSearchDto>>, ICachedQuery
{
    public string? SearchText { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string CacheKey => $"patients:search:{SearchText}:{PageNumber}:{PageSize}".ToLower();
    public TimeSpan? Duration => TimeSpan.FromMinutes(10);
}

public record PatientSearchDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public double SearchScore { get; set; }
}

/// <summary>
/// Patient search handler using Elasticsearch.
/// </summary>
public class SearchPatientsQueryHandler : 
    IQueryHandler<SearchPatientsQuery, SearchResult<PatientSearchDto>>
{
    private readonly ISearchService _searchService;

    public SearchPatientsQueryHandler(ISearchService searchService)
    {
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public async Task<SearchResult<PatientSearchDto>> Handle(
        SearchPatientsQuery query,
        CancellationToken cancellationToken)
    {
        var searchQuery = new SearchQuery
        {
            QueryText = query.SearchText,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            FieldFilters = new Dictionary<string, string>
            {
                // Search in name, email, MRN fields
            }
        };

        // TODO: Map from Elasticsearch results to PatientSearchDto
        var result = await _searchService.SearchAsync<PatientSearchDto>(
            searchQuery,
            cancellationToken);

        return result;
    }
}

/// <summary>
/// Search SOAP notes by clinical findings.
/// Uses medical terminology analyzers.
/// </summary>
public record SearchSoapNotesQuery : IQuery<SearchResult<SoapNoteSearchDto>>, ICachedQuery
{
    public string? ClinicalText { get; init; }
    public Guid? PatientId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string CacheKey => $"soapnotes:search:{ClinicalText}:{PatientId}:{PageNumber}".ToLower();
    public TimeSpan? Duration => TimeSpan.FromMinutes(10);
}

public record SoapNoteSearchDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public double SearchScore { get; set; }
}

/// <summary>
/// SOAP notes search handler.
/// Searches clinical findings using medical terminology.
/// </summary>
public class SearchSoapNotesQueryHandler : 
    IQueryHandler<SearchSoapNotesQuery, SearchResult<SoapNoteSearchDto>>
{
    private readonly ISearchService _searchService;

    public SearchSoapNotesQueryHandler(ISearchService searchService)
    {
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public async Task<SearchResult<SoapNoteSearchDto>> Handle(
        SearchSoapNotesQuery query,
        CancellationToken cancellationToken)
    {
        var searchQuery = new SearchQuery
        {
            QueryText = query.ClinicalText,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            DateRange = (query.StartDate, query.EndDate),
            HighlightResults = true,
            SortBy = new() { ("CreatedAt", SortOrder.Descending) }
        };

        var result = await _searchService.SearchAsync<SoapNoteSearchDto>(
            searchQuery,
            cancellationToken);

        return result;
    }
}
