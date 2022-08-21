using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Services;

internal class ChangeStateService : IChangeStateService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderEventService _orderEventService;
    private readonly IOrderEventCacheService _cacheService;
    private readonly ILogger<ChangeStateService> _logger;

    public ChangeStateService(IOrderRepository orderRepository, IOrderEventService orderEventService,
        IOrderEventCacheService cacheService, ILogger<ChangeStateService> logger)
    {
        _orderRepository = orderRepository;
        _orderEventService = orderEventService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async ValueTask ChangeState(long orderId, OrderState state, CancellationToken ct)
    {
        if (orderId <= 0)
            throw new ArgumentException("Invalid OrderId", nameof(orderId));

        if (await _orderRepository.Get(new[] { orderId }, ct)
                .FirstOrDefaultAsync(ct) is null)
        {
            throw new ArgumentException("OrderId not found", nameof(orderId));
        }

        var value = _cacheService.Get(orderId);

        if (value == state)
        {
            _logger.LogInformation("New state for order id {orderId} is the same as in the cache; " +
                                   "no need to change", orderId);
            return;
        }

        if (value == null)
        {
            _logger.LogInformation("The cache does not contain order id {orderId}; " +
                                   "state should be updated in the db", orderId);
        }
        else
        {
            _logger.LogInformation("The cache contains another state for order id {orderId}; " +
                                   "state should be updated in the db", orderId);
        }

        await _orderRepository.ChangeState(orderId, state, ct);
        await _orderEventService.Add(new OrderEvent(orderId, state, DateTimeOffset.UtcNow), ct);
    }
}
