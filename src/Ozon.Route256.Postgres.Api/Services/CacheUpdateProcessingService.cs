using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Postgres.Api.Abstractions;

namespace Ozon.Route256.Postgres.Api.Services;

public class CacheUpdateProcessingService : ICacheUpdateProcessingService
{
    public async Task DoWork(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Debug.WriteLine(DateTime.Now);
            await Task.Delay(1000, ct);
        }
    }
}
