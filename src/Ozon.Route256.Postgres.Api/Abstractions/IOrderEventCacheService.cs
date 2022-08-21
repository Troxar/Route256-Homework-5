using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Abstractions;

public interface IOrderEventCacheService
{
    void Add(OrderEvent orderEvent);
    OrderState? Get(long orderId);
}
