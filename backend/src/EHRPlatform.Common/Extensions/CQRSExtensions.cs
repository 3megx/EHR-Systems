namespace EHRPlatform.Common.Extensions;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Behaviors;
using System.Reflection;

/// <summary>
/// Extension methods for registering CQRS infrastructure in the DI container.
/// </summary>
public static class CQRSExtensions
{
    /// <summary>
    /// Registers all CQRS handlers and pipeline behaviors from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembliesToScan">Assemblies to scan for handlers and validators.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCQRS(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        if (!assembliesToScan.Any())
        {
            throw new ArgumentException("At least one assembly must be provided to scan for handlers.", nameof(assembliesToScan));
        }

        // Register MediatR with handlers from specified assemblies
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembliesToScan);
        });

        // Register FluentValidation validators from specified assemblies
        services.AddValidatorsFromAssemblies(assembliesToScan);

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }

    /// <summary>
    /// Registers CQRS handlers from the current assembly (caller's assembly).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCQRSFromCurrentAssembly(this IServiceCollection services)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        return services.AddCQRS(callingAssembly);
    }

    /// <summary>
    /// Registers CQRS handlers from multiple assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblyNames">Names of assemblies to scan.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCQRSFromAssemblyNames(
        this IServiceCollection services,
        params string[] assemblyNames)
    {
        var assemblies = assemblyNames
            .Select(name => Assembly.Load(name))
            .ToArray();

        return services.AddCQRS(assemblies);
    }
}
