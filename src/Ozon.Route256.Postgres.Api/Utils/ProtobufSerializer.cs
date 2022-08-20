using System;
using Confluent.Kafka;
using Google.Protobuf;

namespace Ozon.Route256.Postgres.Api.Utils;

public sealed class ProtobufSerializer<T> : ISerializer<T>, IDeserializer<T>
where T: IMessage<T>, new ()
{
    private static readonly MessageParser<T> s_parser = new MessageParser<T>(() => new T());

    public byte[] Serialize(T data, SerializationContext context) => data.ToByteArray();

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(data), "Null data encountered");

        return s_parser.ParseFrom(data);
    }
}
