using System;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Services;

internal class ChangeStateService : IChangeStateService
{
    private readonly IOrderRepository _orderRepository;

    public ChangeStateService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public async ValueTask ChangeState(long orderId, OrderState state, CancellationToken cancellationToken)
    {
        if (orderId <= 0)
            throw new ArgumentException("Invalid OrderId", nameof(orderId));

        if (!Enum.IsDefined(state))
            throw new ArgumentException("State is not defined", nameof(state));

        await _orderRepository.ChangeState(orderId, state, cancellationToken);
    }
}
