using _2C2P.DEMO.Domain.Events;
using _2C2P.DEMO.Infrastructure.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace _2C2P.DEMO.WebApp.Services.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {
            using var seriveProvider = services.BuildServiceProvider();
            var configuration = seriveProvider.GetService<IConfiguration>();
            var producerConfig = new ProducerConfig();
            var consumerConfig = new ConsumerConfig();
            configuration.Bind("ProducerConfig", producerConfig);
            configuration.Bind("ConsumerConfig", consumerConfig);
            services.AddSingleton<ProducerConfig>(producerConfig);
            services.AddSingleton<ConsumerConfig>(consumerConfig);
            services.AddSingleton<IBusClient, KafkaBusClient>();
            var env = configuration.GetValue<string>("Env");
            if (string.IsNullOrWhiteSpace(env))
            {
                throw new ArgumentNullException(nameof(env));
            }

            var bootstrapServers = producerConfig.BootstrapServers;
            if (string.IsNullOrWhiteSpace(bootstrapServers))
            {
                bootstrapServers = consumerConfig.BootstrapServers;
            }

            int numPartitions = 4;
            int customPartition = configuration.GetValue<int>("KafkaConfig:Partition");
            if (customPartition > 0)
            {
                numPartitions = customPartition;
            }

            short replicationFactor = 1;
            short customReplicationFactor = configuration.GetValue<short>("KafkaConfig:ReplicationFactor");
            if (customReplicationFactor > 0)
            {
                replicationFactor = customReplicationFactor;
            }

            short inSyncReplicas = 1;
            short customInSyncReplicas = configuration.GetValue<short>("KafkaConfig:MinInSyncReplicas");
            if (customInSyncReplicas > 0)
            {
                inSyncReplicas = customInSyncReplicas;
            }
            // create topic with default 4 partition for both producer and consumer to prevent the case where consumer run before producer.
            if (!string.IsNullOrWhiteSpace(bootstrapServers))
            {
                KafkaHelper.CreateTopics(bootstrapServers, env, numPartitions, replicationFactor, inSyncReplicas);
            }
            return services;
        }
    }
}
