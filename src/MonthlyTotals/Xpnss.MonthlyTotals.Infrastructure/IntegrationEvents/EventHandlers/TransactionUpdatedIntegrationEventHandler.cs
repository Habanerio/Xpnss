//using Habanerio.Xpnss.Domain.ValueObjects;
//using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
//using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
//using Microsoft.Extensions.Logging;

//namespace Habanerio.Xpnss.PayerPayees.Infrastructure.IntegrationEvents.EventHandlers;

///// <summary>
///// Responsible for handling the 'TransactionUpdatedIntegrationEventHandler'
///// and updating the Account's Balance accordingly.
///// </summary>
///// <param name="accountsRepository"></param>
///// <param name="logger"></param>
//public class TransactionUpdatedIntegrationEventHandler(
//    IAccountsRepository accountsRepository,
//    ILogger<TransactionUpdatedIntegrationEventHandler> logger) :
//    IIntegrationEventHandler<TransactionUpdatedIntegrationEvent>
//{
//    private readonly IAccountsRepository _accountsRepository = accountsRepository ??
//                                throw new ArgumentNullException(nameof(accountsRepository));

//    private readonly ILogger<TransactionUpdatedIntegrationEventHandler> _logger = logger ??
//                                throw new ArgumentNullException(nameof(logger));

//    public async Task Handle(TransactionUpdatedIntegrationEvent @event, CancellationToken cancellationToken)
//    {
//        var accountResult = await _accountsRepository.GetAsync(@event.UserId, @event.AccountId, cancellationToken);

//        if (accountResult.IsFailed)
//            throw new InvalidOperationException(accountResult.Errors[0]?.Message ??
//                                                $"An error occurred while trying to retrieve Account '{@event.AccountId}' for User '{@event.UserId}'");

//        if (accountResult.Value is null)
//            throw new InvalidOperationException($"Account '{@event.AccountId}' could not be found for user '{@event.UserId}'");

//        var account = accountResult.Value;

//        // If @event.NewAmount is 0, then Transaction is considered deleted?
//        // If so, it should have been validated prior, and not reach this point.
//        account.ApplyTransactionAmount(new Money(@event.Difference), @event.TransactionType);

//        //var isCreditTransaction = TransactionTypes.DoesBalanceIncrease(account.AccountType, TransactionTypes.ToTransactionType(@event.TransactionType));

//        //// TODO: This should be handled within the Account itself.
//        //if (account is BaseCreditAccount creditAccount)
//        //{
//        //    if (isCreditTransaction)
//        //        creditAccount.AddDeposit(@event.DateOfTransactionUtc, new Money(@event.Difference));
//        //    else
//        //        creditAccount.AddWithdrawal(@event.DateOfTransactionUtc, new Money(@event.Difference));
//        //}
//        //else
//        //{
//        //    if (isCreditTransaction)
//        //        account.AddDeposit(@event.DateOfTransactionUtc, new Money(@event.Difference));
//        //    else
//        //        account.AddWithdrawal(@event.DateOfTransactionUtc, new Money(@event.Difference));
//        //}

//        try
//        {
//            await _accountsRepository.UpdateAsync(account, cancellationToken);

//            _logger.LogInformation(@event.Id.ToString(), "A '{@transactionType}' Transaction {@transactionId} was added to Account {@accountId}", @event.TransactionType, @event.TransactionId, @event.AccountId);
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "An error occurred while trying to update Account '{@accountId}' for User '{@userId}'", @event.AccountId, @event.UserId);
//            throw;
//        }

//    }
//}