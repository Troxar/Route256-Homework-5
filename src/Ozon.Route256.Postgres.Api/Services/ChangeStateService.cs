using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Services;

internal class ChangeStateService : IChangeStateService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderEventService _orderEventService;

    public ChangeStateService(IOrderRepository orderRepository, IOrderEventService orderEventService)
    {
        _orderRepository = orderRepository;
        _orderEventService = orderEventService;
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

        await _orderRepository.ChangeState(orderId, state, ct);
        await _orderEventService.Add(new OrderEvent(orderId, state, DateTimeOffset.UtcNow), ct);
    }
}
