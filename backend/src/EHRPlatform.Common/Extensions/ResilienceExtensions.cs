using EHRPlatform.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Common.Extensions;

/// <summary>
/// DI extension for wrapping <see cref="IEventPublisher"/> with Polly resilience policies.
/// Register AFTER <see cref="MessagingExtensions.AddKafkaMessaging"/> to ensure
/// the inner publisher is already in the container.
///
/// Uses manual decoration (no Scrutor dependency required).
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Wrap the registered <see cref="IEventPublisher"/> with retry + circuit-breaker.
    /// </summary>
    public static IServiceCollection AddResilientEventPublisher(this IServiceCollection services)
    {
        // Manual decorator pattern: grab existing registration, replace with resilient wrapper
        var innerDescriptor = services.LastOrDefault(d => d.ServiceType == typeof(IEventPublisher));
        if (innerDescriptor == null)
            throw new InvalidOperationException(
                "IEventPublisher must be registered before calling AddResilientEventPublisher.");

        services.RemoveAll<IEventPublisher>();

        services.AddSingleton<IEventPublisher>(sp =>
        {
            // Resolve the inner publisher directly from its implementation
            IEventPublisher inner;
            if (innerDescriptor.ImplementationInstance != null)
            {
                inner = (IEventPublisher)innerDescriptor.ImplementationInstance;
            }
            else if (innerDescriptor.ImplementationFactory != null)
            {
                inner = (IEventPublisher)innerDescriptor.ImplementationFactory(sp);
            }
            else
            {
                inner = (IEventPublisher)ActivatorUtilities.CreateInstance(sp, innerDescriptor.ImplementationType!);
            }

            return new ResilientEventPublisher(
                inner,
                sp.GetRequiredService<ILogger<ResilientEventPublisher>>());
        });

        return services;
    }
}
