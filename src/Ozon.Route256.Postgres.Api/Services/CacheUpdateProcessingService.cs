using System.Threading;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Mapping;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Services;

public class CacheUpdateProcessingService : ICacheUpdateProcessingService
{
    private readonly IOrderEventCacheService _cacheService;
    private readonly ILogger<CacheUpdateProcessingService> _logger;

    public CacheUpdateProcessingService(IOrderEventCacheService cacheService, ILogger<CacheUpdateProcessingService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public void DoWork(CancellationToken ct)
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

            _logger.LogInformation("Got message from Kafka: {value}", result.Message.Value);

            var value = _cacheService.Get(result.Message.Value.OrderId);
            var orderEvent = result.Message.Value.Map();

            if (value != orderEvent.state)
            {
                _logger.LogInformation("Message contains new state for order id {orderId}; " +
                                       "the cache should be updated", orderEvent.orderId);
                _cacheService.Add(orderEvent.orderId, orderEvent.state);

            }
            else
            {
                _logger.LogInformation("Message contains the same state for order id {orderId} as in the cache; " +
                                       "no need to update the cache", orderEvent.orderId);
            }

            consumer.StoreOffset(result);
        }

        consumer.Close();
    }
}
