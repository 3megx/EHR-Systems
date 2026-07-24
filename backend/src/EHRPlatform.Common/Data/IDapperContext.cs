#nullable enable

namespace EHRPlatform.Common.Data;

/// <summary>
/// Thin Dapper façade over the service's existing database connection.
/// Use for complex reporting queries, bulk operations, and anything where
/// EF Core's LINQ translation produces inefficient SQL.
///
/// The connection is owned by the EF Core DbContext so it participates in
/// the same transaction when one is open via <see cref="IUnitOfWork"/>.
/// </summary>
public interface IDapperContext
{
    /// <summary>Execute a query and return a typed sequence.</summary>
    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? parameters          = null,
        CancellationToken ct        = default);

    /// <summary>Return the first row or default.</summary>
    Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? parameters          = null,
        CancellationToken ct        = default);

    /// <summary>Execute a non-query (INSERT/UPDATE/DELETE) and return rows affected.</summary>
    Task<int> ExecuteAsync(
        string sql,
        object? parameters          = null,
        CancellationToken ct        = default);

    /// <summary>Execute a scalar query (COUNT, SUM, …).</summary>
    Task<T?> ExecuteScalarAsync<T>(
        string sql,
        object? parameters          = null,
        CancellationToken ct        = default);

    /// <summary>
    /// Multi-result query — maps two joined tables into a single result type.
    /// </summary>
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        object? parameters          = null,
        string splitOn              = "Id",
        CancellationToken ct        = default);
}
