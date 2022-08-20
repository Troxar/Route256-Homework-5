using System.Threading;
using System.Threading.Tasks;

namespace Ozon.Route256.Postgres.Api.Abstractions;

public interface ICacheUpdateProcessingService
{
    Task DoWork(CancellationToken ct);
}
