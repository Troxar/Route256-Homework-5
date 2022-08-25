using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Services;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Domain;
using Ozon.Route256.Postgres.Persistence;
using Ozon.Route256.Postgres.Persistence.Common;

namespace Ozon.Route256.Postgres.Api;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderState>("order_state");

        var connectionString = _configuration["ConnectionString"];

        services
            .AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            })
            .Services
            .AddGrpcReflection()
            .AddFluentMigrator(
                connectionString,
                typeof(SqlMigration).Assembly)
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Constants.RedisConfig;
                options.InstanceName = "StackExchangeRedis";
            });

        services.AddScoped<IOrderRepository, OrderRepository>(provider => new(connectionString,
            provider.GetRequiredService<ILogger<OrderRepository>>()));
        services.AddScoped<IChangeStateService, ChangeStateService>();
        services.AddTransient<IOrderEventService, KafkaService>();
        services.AddHostedService<CacheUpdateHostedService>();
        services.AddScoped<ICacheUpdateProcessingService, CacheUpdateProcessingService>();

        //todo: put in config - RedisService or DistributedCacheService;
        services.AddScoped<IOrderEventCacheService, DistributedCacheService>();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env) =>
        app
            .UseRouting()
            .UseEndpoints(
                endpoints =>
                {
                    endpoints.MapGrpcService<OrderGrpcService>();
                    endpoints.MapGrpcReflectionService();
                });
}
