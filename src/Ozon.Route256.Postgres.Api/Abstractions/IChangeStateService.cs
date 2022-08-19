using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Abstractions;

public interface IChangeStateService
{
    ValueTask ChangeState(long orderId, OrderState state, CancellationToken cancellationToken);
}
