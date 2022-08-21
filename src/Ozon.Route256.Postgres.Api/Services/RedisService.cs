using System;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Domain;
using StackExchange.Redis;

namespace Ozon.Route256.Postgres.Api.Services;

public class RedisService : IOrderEventCacheService, IDisposable
{
    private readonly ILogger<RedisService> _logger;
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _db;

    public RedisService(ILogger<RedisService> logger)
    {
        _logger = logger;
        _connection = ConnectionMultiplexer.Connect(Constants.RedisConfig);
        _db = _connection.GetDatabase();
    }

    public void Add(OrderEvent orderEvent)
    {
        _db.StringSet(orderEvent.orderId.ToString(), (int)orderEvent.state);
        _logger.LogInformation("The value has been set: {value}", orderEvent);
    }

    public OrderState? Get(long orderId)
    {
        var value = _db.StringGet(orderId.ToString());
        if (value == RedisValue.Null)
            return null;

        return (OrderState)(int.Parse(value!));
    }

    public void Dispose() => _connection.Dispose();
}
