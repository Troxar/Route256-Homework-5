using System;
using System.Diagnostics;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Domain;
using StackExchange.Redis;

namespace Ozon.Route256.Postgres.Api.Services;

public class RedisService : IOrderEventCacheService, IDisposable
{
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _db;

    public RedisService()
    {
        _connection = ConnectionMultiplexer.Connect(Constants.RedisConfig);
        _db = _connection.GetDatabase();
    }

    public void Add(OrderEvent orderEvent)
    {
        _db.StringSet(orderEvent.orderId.ToString(), (int)orderEvent.state);
        Debug.WriteLine($"Redis set the value: {orderEvent}");
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
