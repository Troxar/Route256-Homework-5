using System;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Mapping;

internal static class OrderMapping
{
    public static Order Map(this Domain.Order order) =>
        new Order
        {
            Id = order.Id,
            Amount = order.Amount.ToMoney(),
            State = order.State.ToGrpc(),
            Items = { order.Items.Select(Map) }
        };

    private static Order.Types.Item Map(this Domain.Order.Item item) =>
        new Order.Types.Item
        {
            SkuId = item.SkuId,
            Quantity = item.Quantity,
            Price = item.Price.ToMoney()
        };

    private static OrderState ToGrpc(this Domain.OrderState state) =>
        state switch {
            Domain.OrderState.Unknown => OrderState.Unknown,
            Domain.OrderState.Created => OrderState.Created,
            Domain.OrderState.Paid => OrderState.Paid,
            Domain.OrderState.Boxing => OrderState.Boxing,
            Domain.OrderState.WaitForPickup => OrderState.WaitForPickup,
            Domain.OrderState.InDelivery => OrderState.InDelivery,
            Domain.OrderState.WaitForClient => OrderState.WaitForClient,
            Domain.OrderState.Completed => OrderState.Completed,
            Domain.OrderState.Cancelled => OrderState.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

    public static Domain.OrderState ToDomain(this OrderState state) =>
        state switch {
            OrderState.Unknown => Domain.OrderState.Unknown,
            OrderState.Created => Domain.OrderState.Created,
            OrderState.Paid => Domain.OrderState.Paid,
            OrderState.Boxing =>Domain.OrderState.Boxing,
            OrderState.WaitForPickup => Domain.OrderState.WaitForPickup,
            OrderState.InDelivery => Domain.OrderState.InDelivery,
            OrderState.WaitForClient => Domain.OrderState.WaitForClient,
            OrderState.Completed => Domain.OrderState.Completed,
            OrderState.Cancelled => Domain.OrderState.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

    public static OrderEvent Map(this Domain.OrderEvent orderEvent) =>
        new ()
        {
            OrderId = orderEvent.orderId,
            State = orderEvent.state.ToGrpc(),
            Timestamp = Timestamp.FromDateTimeOffset(orderEvent.timestamp),
        };

    public static Domain.OrderEvent Map(this OrderEvent orderEvent) =>
        new
        (
            orderEvent.OrderId,
            orderEvent.State.ToDomain(),
            orderEvent.Timestamp.ToDateTimeOffset()
        );
}
