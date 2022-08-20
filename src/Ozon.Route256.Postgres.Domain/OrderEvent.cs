using System;
using Ozon.Route256.Postgres.Domain;

namespace Ozon.Route256.Postgres.Domain;

public sealed record OrderEvent(long orderId, OrderState state, DateTimeOffset timestamp);
