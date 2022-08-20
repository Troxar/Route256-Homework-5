using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Api.Abstractions;

public interface IOrderEventService
{
    ValueTask Add(OrderEvent orderEvent, CancellationToken cancellationToken);
}
