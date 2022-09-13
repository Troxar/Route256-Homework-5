using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Services;

public class DistributedCacheService : IOrderEventCacheService
{
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly IDistributedCache _cache;

    public DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger)
    {
        _logger = logger;
        _cache = cache;
    }

    public void Add(long orderId, OrderState state)
    {
        _cache.Set(orderId.ToString(), BitConverter.GetBytes((int)state));
        _logger.LogInformation("State {state} has been set for the key {orderId}", state, orderId);
    }

    public OrderState? Get(long orderId)
    {
        var value = _cache.Get(orderId.ToString());
        if (value == null)
            return null;

        return (OrderState)BitConverter.ToInt32(value);
    }
}
