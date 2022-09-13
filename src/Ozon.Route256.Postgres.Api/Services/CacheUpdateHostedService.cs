using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Services;

public class CacheUpdateHostedService : BackgroundService
{
    private readonly IServiceProvider _services;

    public CacheUpdateHostedService(IServiceProvider services) => _services = services;
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Run(() => DoConsuming(ct), ct);
    }

    private void DoConsuming(CancellationToken ct)
    {
        var consumer = new ConsumerBuilder<long, OrderEvent>(
                new ConsumerConfig
                {
                    BootstrapServers = Constants.Broker,
                    GroupId = "cache-update-service",
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    EnableAutoCommit = true,
                    EnableAutoOffsetStore = false
                })
            .SetValueDeserializer(new ProtobufSerializer<OrderEvent>())
            .Build();
        consumer.Subscribe(Constants.Topic);

        while (consumer.Consume(ct) is { } result)
        {
            ct.ThrowIfCancellationRequested();

            using (var scope = _services.CreateScope())
            {
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ICacheUpdateProcessingService>();
                scopedProcessingService.ProcessConsumeResult(result);
            }

            consumer.StoreOffset(result);
        }

        consumer.Close();
    }
}
