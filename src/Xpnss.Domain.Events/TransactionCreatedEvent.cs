using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Domain.Events;

internal class TransactionCreatedEvent(AccountId accountId, Money amount) : IRequest
{ }