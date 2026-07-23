using EHRPlatform.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Confluent.Kafka;

namespace EHRPlatform.Common.Extensions;

/// <summary>
/// DI extensions for Kafka messaging and outbox pattern.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Register Kafka event publisher and outbox processor.
    /// </summary>
    public static IServiceCollection AddKafkaMessaging(
        this IServiceCollection services,
        string bootstrapServers,
        string environment = "production")
    {
        ArgumentGuard.NotNullOrEmpty(bootstrapServers, nameof(bootstrapServers));

        // Create producer
        var producerConfig = KafkaConfigBuilder.CreateProducerConfig(bootstrapServers);
        var producer = new ProducerBuilder<string, string>(producerConfig).Build();
        services.AddSingleton(producer);

        // Register event publisher
        services.AddSingleton<IEventPublisher>(sp =>
            new KafkaEventPublisher(
                producer,
                environment,
                sp.GetRequiredService<ILogger<KafkaEventPublisher>>()));

        // Register outbox processor
        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    /// <summary>
    /// Register Kafka consumer for event type.
    /// </summary>
    public static IServiceCollection AddKafkaConsumer<TConsumer, TEvent>(
        this IServiceCollection services,
        string bootstrapServers,
        string groupId,
        string topicName)
        where TConsumer : KafkaConsumerBase<TEvent>
        where TEvent : IntegrationEvent
    {
        ArgumentGuard.NotNullOrEmpty(bootstrapServers, nameof(bootstrapServers));
        ArgumentGuard.NotNullOrEmpty(groupId, nameof(groupId));

        // Create consumer
        var consumerConfig = KafkaConfigBuilder.CreateConsumerConfig(bootstrapServers, groupId);
        var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        services.AddSingleton(consumer);

        // Register consumer as hosted service
        services.AddHostedService<TConsumer>();

        return services;
    }
}

/// <summary>
/// Argument validation.
/// </summary>
internal static class ArgumentGuard
{
    public static void NotNullOrEmpty(string? argument, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("Value cannot be null or empty", parameterName);
    }
}
