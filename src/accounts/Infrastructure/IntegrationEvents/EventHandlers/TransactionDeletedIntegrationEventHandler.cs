using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Infrastructure.IntegrationEvents.EventHandlers;

/// <summary>
/// Responsible for handling the 'TransactionDeletedIntegrationEventHandler'
/// and updating the Account's Balance accordingly.
/// </summary>
/// <param name="repository"></param>
/// <param name="logger"></param>
public class TransactionDeletedIntegrationEventHandler(
    IAccountsRepository repository,
    ILogger<TransactionDeletedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<TransactionDeletedIntegrationEvent>
{
    private readonly IAccountsRepository _accountsRepository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<TransactionDeletedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task Handle(TransactionDeletedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var accountResult = await _accountsRepository.GetAsync(@event.UserId, @event.AccountId, cancellationToken);

        if (accountResult.IsFailed)
            throw new InvalidOperationException(accountResult.Errors[0]?.Message ??
                                                $"An error occurred while trying to retrieve Account '{@event.AccountId}' for User '{@event.UserId}'");

        if (accountResult.Value is null)
            throw new InvalidOperationException($"Account '{@event.AccountId}' could not be found for user '{@event.UserId}'");

        var account = accountResult.Value;

        account.AddTransactionAmount(new Money(@event.Amount), @event.TransactionType);

        //var isCreditTransaction = TransactionEnums.DoesBalanceIncrease(accountBase.AccountType, TransactionEnums.ToTransactionType(@event.TransactionType));

        //// TODO: This should be handled within the Account itself.
        //if (accountBase is BaseCreditAccount creditAccount)
        //{
        //    if (isCreditTransaction)
        //        creditAccount.RemoveWithdrawal(@event.DateOfTransactionUtc, new Money(@event.CreditLimit));
        //    else
        //        creditAccount.RemoveDeposit(@event.DateOfTransactionUtc, new Money(@event.CreditLimit));
        //}
        //else
        //{
        //    if (isCreditTransaction)
        //        accountBase.RemoveWithdrawal(@event.DateOfTransactionUtc, new Money(@event.CreditLimit));
        //    else
        //        accountBase.RemoveDeposit(@event.DateOfTransactionUtc, new Money(@event.CreditLimit));
        //}

        try
        {
            await _accountsRepository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation(@event.Id.ToString(), "A '{@transactionType}' Transaction {@transactionId} was deleted from Account {@accountId}", @event.TransactionType, @event.TransactionId, @event.AccountId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update Account '{AccountId}' for User '{UserId}'", @event.AccountId, @event.UserId);
            throw;
        }
    }
}