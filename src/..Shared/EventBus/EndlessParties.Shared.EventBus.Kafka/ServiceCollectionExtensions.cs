using Confluent.Kafka;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.EventBus.Kafka.Settings;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace EndlessParties.Shared.EventBus.Kafka;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления шины событий на основе очереди Kafka
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление шины событий на основе очереди Kafka
    /// </summary>
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services, KafkaEventBusSettings settings)
    {
        services
            .AddKafka(kafkaSetup =>
            {
                kafkaSetup
                    .AddCluster(clusterSetup =>
                    {
                        clusterSetup
                            .WithBrokers([settings.BrokerAddress])
                            .AddProducers(settings.Producers)
                            .AddConsumers(settings.Consumers);
                    });
            });

        services
            .AddSingleton(settings)
            .AddSingleton<IEventBus, KafkaEventBus>();

        if (settings.Consumers.Count == 0)
        {
            return services;
        }

        foreach (var consumer in settings.Consumers)
        {
            var eventBusBatchGenericType = typeof(IEventBusBatchConsumer<>).MakeGenericType(consumer.EventType);

            if (!consumer.ConsumerType.IsAssignableTo(eventBusBatchGenericType))
            {
                continue;
            }

            var messageBatchHandlerGenericType = typeof(MessageBatchMiddleware<>).MakeGenericType(consumer.EventType);
            services
                .AddSingleton(messageBatchHandlerGenericType)
                .AddScoped(eventBusBatchGenericType, consumer.ConsumerType);
        }

        services.AddHostedService<KafkaBusLifecycleManager>();

        return services;
    }

    extension(IClusterConfigurationBuilder builder)
    {
        /// <summary>
        /// Добавление поставщиков
        /// </summary>
        private IClusterConfigurationBuilder AddProducers(IReadOnlyDictionary<Type, KafkaProducerSettings> producers)
        {
            var сonfig = new ProducerConfig
            {
                Acks = Confluent.Kafka.Acks.All,
                EnableIdempotence = true,
                LingerMs = 20,
                CompressionType = CompressionType.Snappy,
                MessageSendMaxRetries = int.MaxValue,
                RetryBackoffMs = 100,
                MessageTimeoutMs = 30000,
                RequestTimeoutMs = 10000
            };

            foreach (var producer in producers.Values)
            {
                builder
                    .CreateTopicIfNotExists(producer.TopicName, 1, 1)
                    .AddProducer(producer.Name, producerSetup =>
                    {
                        producerSetup
                            .DefaultTopic(producer.TopicName)
                            .WithProducerConfig(сonfig)
                            .AddMiddlewares(middlewareSetup => middlewareSetup
                                .AddSerializer<JsonCoreSerializer>());
                    });
            }

            return builder;
        }

        /// <summary>
        /// Добавление потребителей
        /// </summary>
        private void AddConsumers(IReadOnlyList<KafkaConsumerSettings> consumers)
        {
            var config = new ConsumerConfig
            {
                EnableAutoCommit = true,
                AutoCommitIntervalMs = 5000,
                QueuedMinMessages = 1000,
                MaxPartitionFetchBytes = 5242880,
                FetchMinBytes = 10240,
                FetchWaitMaxMs = 500,
                EnablePartitionEof = false
            };

            foreach (var consumer in consumers)
            {
                builder
                    .CreateTopicIfNotExists(consumer.TopicName, 1, 1)
                    .AddConsumer(consumerSetup =>
                    {
                        consumerSetup
                            .WithConsumerConfig(config)
                            .Topic(consumer.TopicName)
                            .WithGroupId(consumer.GroupId)
                            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                            .WithWorkersCount(1)
                            .WithBufferSize(100)
                            .AddMiddlewares(middlewareSetup =>
                            {
                                var messageBatchHandlerGenericType = typeof(MessageBatchMiddleware<>).MakeGenericType(consumer.EventType);

                                middlewareSetup
                                    .AddDeserializer<JsonCoreDeserializer>()
                                    .AddBatching(10, TimeSpan.FromSeconds(3))
                                    .Add(resolver => (IMessageMiddleware)resolver.Resolve(messageBatchHandlerGenericType));
                            });
                    });
            }
        }
    }
}