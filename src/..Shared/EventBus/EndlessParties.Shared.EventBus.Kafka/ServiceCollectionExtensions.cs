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
    public static IServiceCollection AddKafkaEventBus(this IServiceCollection services, KafkaSettings settings)
    {
        services
            .AddSingleton(settings)
            .AddSingleton<IEventBus, KafkaEventBus>()
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

        if (settings.Consumers.Count == 0)
        {
            return services;
        }

        foreach (var consumer in settings.Consumers.Values)
        {
            services
                .AddSingleton(consumer.ProxyType)
                .AddScoped(consumer.TargetInterfaceType, consumer.TargetImplementationType);
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
        private void AddConsumers(IReadOnlyDictionary<Type, KafkaConsumerSettings> consumers)
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

            foreach (var consumer in consumers.Values)
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
                            .WithBufferSize(100);

                        consumerSetup
                            .AddMiddlewares(middlewareSetup =>
                            {
                                if (consumer is KafkaConsumerBatchSettings batchSettings)
                                {
                                    middlewareSetup
                                        .AddDeserializer<JsonCoreDeserializer>()
                                        .AddBatching(batchSettings.Count, batchSettings.Timeout)
                                        .Add(resolver => (IMessageMiddleware)resolver.Resolve(batchSettings.ProxyType));
                                }
                                else
                                {
                                    middlewareSetup
                                        .AddDeserializer<JsonCoreDeserializer>()
                                        .AddTypedHandlers(handlersSetup =>
                                        {
                                            handlersSetup
                                                .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                .AddHandlers([consumer.ProxyType]);
                                        });
                                }
                            });
                    });
            }
        }
    }
}