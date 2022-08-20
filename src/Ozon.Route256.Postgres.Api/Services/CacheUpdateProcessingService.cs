using System.Diagnostics;
using System.Threading;
using Confluent.Kafka;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Services;

public class CacheUpdateProcessingService : ICacheUpdateProcessingService
{
    public void DoWork(CancellationToken ct)
    {
        var consumer = new ConsumerBuilder<long, OrderEvent>(
                new ConsumerConfig
                {
                    BootstrapServers = Constants.Broker,
                    GroupId = "cache-update-service",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    EnableAutoOffsetStore = false
                })
            .SetValueDeserializer(new ProtobufSerializer<OrderEvent>())
            .Build();
        consumer.Subscribe(Constants.Topic);

        while (consumer.Consume(ct) is { } result)
        {
            ct.ThrowIfCancellationRequested();
            Debug.WriteLine(result.Message.Value);
            consumer.StoreOffset(result);
        }

        consumer.Close();
    }
}
