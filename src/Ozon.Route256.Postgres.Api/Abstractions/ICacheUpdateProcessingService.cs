using Confluent.Kafka;
using Ozon.Route256.Postgres.Grpc;

namespace Ozon.Route256.Postgres.Api.Abstractions;

public interface ICacheUpdateProcessingService
{
    void ProcessConsumeResult(ConsumeResult<long, OrderEvent> consumeResult);
}
