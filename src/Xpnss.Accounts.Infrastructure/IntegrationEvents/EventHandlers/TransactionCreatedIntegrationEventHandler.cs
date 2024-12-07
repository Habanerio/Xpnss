using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.Accounts.Infrastructure.IntegrationEvents.EventHandlers;

/// <summary>
/// Responsible for handling the 'TransactionCreatedIntegrationEvent'
/// and updating the Account's Balance and the MonthlyTotal.
/// </summary>
/// <param name="repository"></param>
/// <param name="logger"></param>
public class TransactionCreatedIntegrationEventHandler(
    IAccountsRepository repository,
    ILogger<TransactionCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<TransactionCreatedIntegrationEvent>
{
    private readonly IAccountsRepository _accountsRepository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<TransactionCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task Handle(
        TransactionCreatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            await UpdateAccountBalanceAsync(@event, cancellationToken);

            _logger.LogInformation(@event.Id.ToString(),
                "A '{@transactionType}' Transaction ({@transactionId}) was added to Account {@accountId} for the amount of {@Amount}",
                @event.TransactionType,
                @event.TransactionId,
                @event.AccountId,
                @event.Amount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update Account '{ExtAcctId}' for User '{UserId}'", @event.AccountId, @event.UserId);

            throw;
        }
    }

    private async Task UpdateAccountBalanceAsync(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var accountResult = await _accountsRepository.GetAsync(@event.UserId, @event.AccountId, cancellationToken);

        if (accountResult.IsFailed)
            throw new InvalidOperationException(accountResult.Errors[0]?.Message ??
                $"An error occurred while trying to retrieve Account '{@event.AccountId}' for User '{@event.UserId}'");

        if (accountResult.Value is null)
            throw new InvalidOperationException(
                $"Account '{@event.AccountId}' could not be found for user '{@event.UserId}'");

        var account = accountResult.Value;

        account.AddTransactionAmount(new Money(@event.Amount), @event.TransactionType);

        try
        {
            await _accountsRepository.UpdateAsync(account, cancellationToken);


        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Error occurred while trying to update the Balance for Account '{ExtAcctId}'", @event.AccountId);
        }
    }
}