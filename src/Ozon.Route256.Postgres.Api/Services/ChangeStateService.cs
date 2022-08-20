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

    public ChangeStateService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

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
    }
}
