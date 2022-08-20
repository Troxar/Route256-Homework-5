using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Postgres.Api.Abstractions;

namespace Ozon.Route256.Postgres.Api.Services;

public class CacheUpdateHostedService : BackgroundService
{
    private readonly IServiceProvider _services;

    public CacheUpdateHostedService(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Run(() => DoWork(ct), ct);
    }

    private void DoWork(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ICacheUpdateProcessingService>();
        scopedProcessingService.DoWork(ct);
    }
}
