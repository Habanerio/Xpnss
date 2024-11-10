using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.Events;
using Habanerio.Xpnss.Domain.Transactions;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.EventHandlers;

public class TransactionCreatedDomainEventHandler(IAccountsRepository accountsRepository) :
    IRequestHandler<TransactionCreatedDomainEvent>
{
    private readonly IAccountsRepository _accountsRepository = accountsRepository ??
                                throw new ArgumentNullException(nameof(accountsRepository));

    public async Task Handle(TransactionCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        var accountResult = await _accountsRepository.GetAsync(@event.UserId, @event.AccountId, cancellationToken);

        if (accountResult.IsFailed)
            throw new InvalidOperationException(accountResult.Errors[0]?.Message ??
                                                $"An error occurred while trying to retrieve Account '{@event.AccountId}' for User '{@event.UserId}'");

        if (accountResult.Value is null)
            throw new InvalidOperationException($"Account '{@event.AccountId}' could not be found for user '{@event.UserId}'");

        var account = accountResult.Value;

        var isCredit = TransactionTypes.IsCredit(@event.TransactionType);

        if (isCredit)
            account.Deposit(@event.DateOfTransactionUtc, new Money(@event.Amount));
        else
            account.Withdraw(@event.DateOfTransactionUtc, new Money(@event.Amount));

        await _accountsRepository.UpdateAsync(account, cancellationToken);
    }
}