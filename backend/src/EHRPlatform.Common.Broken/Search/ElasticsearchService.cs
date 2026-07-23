using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace EHRPlatform.Common.Search;

/// <summary>
/// Elasticsearch implementation of ISearchService.
/// Provides full-text search with medical terminology support.
/// Handles indexing, searching, and index management.
/// </summary>
public class ElasticsearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Search with full-text queries and filters.
    /// </summary>
    public async Task<SearchResult<T>> SearchAsync<T>(
        SearchQuery query,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNull(query, nameof(query));

        var indexName = GetIndexName<T>();
        var pageSize = Math.Min(query.PageSize, 100);
        var from = (query.PageNumber - 1) * pageSize;

        try
        {
            var searchRequest = new SearchRequest(indexName);

            // Build query
            Query q = BuildQuery(query);
            searchRequest.Query = q;

            // Pagination
            searchRequest.From = from;
            searchRequest.Size = pageSize;

            // Sorting
            if (query.SortBy?.Any() == true)
            {
                searchRequest.Sort = query.SortBy
                    .Select(s => new Sort { Field = s.field, Order = s.order == SortOrder.Ascending ? SortOrder.Asc : SortOrder.Desc })
                    .Cast<ISort>()
                    .ToList();
            }

            // Highlighting
            if (query.HighlightResults)
            {
                searchRequest.Highlight = new Highlight
                {
                    Fields = new Dictionary<Field, HighlightField>
                    {
                        { "*", new HighlightField { } }
                    }
                };
            }

            // Facets (aggregations)
            if (query.Facets?.Any() == true)
            {
                searchRequest.Aggregations = query.Facets.ToDictionary(
                    f => f,
                    f => new Aggregation { Terms = new TermsAggregation { Field = f } });
            }

            var response = await _client.SearchAsync<T>(searchRequest, cancellationToken);

            if (!response.IsSuccess())
                throw new SearchException($"Search failed: {response.ApiCallDetails?.OriginalException?.Message}");

            return MapSearchResult<T>(response, query.PageNumber, pageSize);
        }
        catch (Exception ex)
        {
            throw new SearchException($"Search error for {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Index single entity.
    /// </summary>
    public async Task IndexAsync<T>(
        string id,
        T entity,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNullOrEmpty(id, nameof(id));
        ArgumentGuard.NotNull(entity, nameof(entity));

        var indexName = GetIndexName<T>();

        try
        {
            var response = await _client.IndexAsync(
                new IndexRequest<T>(indexName) 
                { 
                    Id = id, 
                    Document = entity 
                },
                cancellationToken);

            if (!response.IsSuccess())
                throw new SearchException($"Index failed: {response.ApiCallDetails?.OriginalException?.Message}");
        }
        catch (Exception ex)
        {
            throw new SearchException($"Index error for {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Bulk index multiple entities.
    /// </summary>
    public async Task IndexBulkAsync<T>(
        IEnumerable<(string id, T entity)> items,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNull(items, nameof(items));

        var itemList = items.ToList();
        if (itemList.Count == 0)
            return;

        var indexName = GetIndexName<T>();

        try
        {
            var bulkRequest = new BulkRequest(indexName);
            var operations = new List<IBulkOperation>();

            foreach (var (id, entity) in itemList)
            {
                operations.Add(new BulkIndexOperation<T> { Id = id, Document = entity });
            }

            bulkRequest.Operations = operations;

            var response = await _client.BulkAsync(bulkRequest, cancellationToken);

            if (!response.IsSuccess())
                throw new SearchException($"Bulk index failed: {response.ApiCallDetails?.OriginalException?.Message}");
        }
        catch (Exception ex)
        {
            throw new SearchException($"Bulk index error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Delete entity from index.
    /// </summary>
    public async Task DeleteAsync<T>(
        string id,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentGuard.NotNullOrEmpty(id, nameof(id));

        var indexName = GetIndexName<T>();

        try
        {
            var response = await _client.DeleteAsync(indexName, id, cancellationToken);

            if (!response.IsSuccess())
                throw new SearchException($"Delete failed: {response.ApiCallDetails?.OriginalException?.Message}");
        }
        catch (Exception ex)
        {
            throw new SearchException($"Delete error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Rebuild entire index (delete and reindex).
    /// Used after schema changes or migrations.
    /// </summary>
    public async Task RebuildIndexAsync<T>(
        CancellationToken cancellationToken = default) where T : class
    {
        var indexName = GetIndexName<T>();

        try
        {
            // Delete existing index
            if (await IndexExistsAsync<T>(cancellationToken))
            {
                await _client.Indices.DeleteAsync(indexName, cancellationToken);
            }

            // Create new index with mappings
            await CreateIndexAsync<T>(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new SearchException($"Rebuild index error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Check if index exists.
    /// </summary>
    public async Task<bool> IndexExistsAsync<T>(
        CancellationToken cancellationToken = default) where T : class
    {
        var indexName = GetIndexName<T>();

        try
        {
            var response = await _client.Indices.ExistsAsync(indexName, cancellationToken);
            return response.Exists;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Delete entire index.
    /// </summary>
    public async Task DeleteIndexAsync<T>(
        CancellationToken cancellationToken = default) where T : class
    {
        var indexName = GetIndexName<T>();

        try
        {
            var response = await _client.Indices.DeleteAsync(indexName, cancellationToken);

            if (!response.IsSuccess())
                throw new SearchException($"Delete index failed: {response.ApiCallDetails?.OriginalException?.Message}");
        }
        catch (Exception ex)
        {
            throw new SearchException($"Delete index error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get search statistics.
    /// </summary>
    public async Task<SearchStatistics> GetStatisticsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.Indices.GetMappingAsync(new GetMappingRequest(), cancellationToken);
            
            var stats = new SearchStatistics();

            if (response.Indices != null)
            {
                foreach (var (index, mapping) in response.Indices)
                {
                    stats.IndexDocumentCounts[index.Name] = 0; // Would need stats API
                }
            }

            return stats;
        }
        catch (Exception ex)
        {
            throw new SearchException($"Statistics error: {ex.Message}", ex);
        }
    }

    #region Helpers

    private string GetIndexName<T>() where T : class
    {
        return typeof(T).Name.ToLower() + "-index";
    }

    private Query BuildQuery(SearchQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.QueryText) && (query.FieldFilters?.Count ?? 0) == 0)
            return new MatchAllQuery();

        var filters = new List<Query>();

        // Full-text search
        if (!string.IsNullOrWhiteSpace(query.QueryText))
        {
            filters.Add(new MultiMatchQuery 
            { 
                Query = query.QueryText,
                Fields = new[] { "*" }.ToList()
            });
        }

        // Field filters
        if (query.FieldFilters?.Any() == true)
        {
            foreach (var (field, value) in query.FieldFilters)
            {
                filters.Add(new TermQuery { Field = field, Value = value });
            }
        }

        // Date range
        if (query.DateRange.HasValue)
        {
            var (from, to) = query.DateRange.Value;
            filters.Add(new DateRangeQuery 
            { 
                Field = "CreatedAt",
                Gte = from,
                Lte = to
            });
        }

        return filters.Count switch
        {
            0 => new MatchAllQuery(),
            1 => filters[0],
            _ => new BoolQuery { Must = filters }
        };
    }

    private SearchResult<T> MapSearchResult<T>(
        SearchResponse<T> response,
        int pageNumber,
        int pageSize) where T : class
    {
        var result = new SearchResult<T>
        {
            TotalCount = response.Total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Hits = response.Hits
                ?.Select(h => new SearchHit<T>
                {
                    Id = h.Id,
                    Document = h.Source,
                    Score = h.Score ?? 0,
                    Highlights = h.Highlight?.ToDictionary(kv => kv.Key.Name, kv => kv.Value.ToArray())
                })
                .ToList() ?? new()
        };

        return result;
    }

    private async Task CreateIndexAsync<T>(CancellationToken cancellationToken) where T : class
    {
        var indexName = GetIndexName<T>();
        var request = new CreateIndexRequest(indexName);

        // Set analyzer for medical text
        request.Settings = new IndexSettings
        {
            Analysis = new Analysis
            {
                Analyzers = new Dictionary<string, IAnalyzer>
                {
                    {
                        "medical_analyzer",
                        new CustomAnalyzer
                        {
                            Tokenizer = "standard",
                            Filters = new[] { "lowercase", "medical_synonyms" }
                        }
                    }
                },
                TokenFilters = new Dictionary<string, ITokenFilter>
                {
                    {
                        "medical_synonyms",
                        new SynonymTokenFilter
                        {
                            Synonyms = new[]
                            {
                                "diabetes => dm",
                                "hypertension => htn",
                                "congestive heart failure => chf",
                                "myocardial infarction => mi"
                            }
                        }
                    }
                }
            }
        };

        await _client.Indices.CreateAsync(request, cancellationToken);
    }

    #endregion
}

/// <summary>
/// Search-specific exception.
/// </summary>
public class SearchException : Exception
{
    public SearchException(string message) : base(message) { }
    public SearchException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Argument validation helper.
/// </summary>
internal static class ArgumentGuard
{
    public static void NotNull<T>(T? argument, string parameterName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(parameterName);
    }

    public static void NotNullOrEmpty(string? argument, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("Value cannot be null or empty", parameterName);
    }
}
