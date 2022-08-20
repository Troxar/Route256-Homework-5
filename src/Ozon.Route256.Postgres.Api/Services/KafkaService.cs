using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Ozon.Route256.Postgres.Api.Abstractions;
using Ozon.Route256.Postgres.Api.Mapping;
using Ozon.Route256.Postgres.Api.Utils;
using Ozon.Route256.Postgres.Domain;
using GrpcEvent = Ozon.Route256.Postgres.Grpc.OrderEvent;

namespace Ozon.Route256.Postgres.Api.Services;

public class KafkaService : IOrderEventService
{
    public async ValueTask Add(OrderEvent orderEvent, CancellationToken ct)
    {
        var producer = new ProducerBuilder<long, GrpcEvent>(
                new ProducerConfig
                {
                    BootstrapServers = Constants.Broker,
                    Acks = Acks.Leader

                })
            .SetValueSerializer(new ProtobufSerializer<GrpcEvent>())
            .Build();
        await producer.ProduceAsync(Constants.Topic, new Message<long, GrpcEvent>
        {
            Key = orderEvent.orderId,
            Value = orderEvent.Map()
        }, ct);
        producer.Flush(ct);
    }
}
