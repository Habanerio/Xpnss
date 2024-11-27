using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;
using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.IntegrationEvents.EventHandlers;

/// <summary>
/// Responsible for handling the `TransactionCreatedIntegrationEvent` and updating the MonthlyTotals.
/// </summary>
/// <param name="repository"></param>
/// <param name="logger"></param>
public class TransactionCreatedIntegrationEventHandler(
    IMonthlyTotalsRepository repository,
    ILogger<TransactionCreatedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<TransactionCreatedIntegrationEvent>
{
    private readonly IMonthlyTotalsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<TransactionCreatedIntegrationEventHandler> _logger = logger ??
        throw new ArgumentNullException(nameof(logger));

    public async Task Handle(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            var isCreditTransaction = TransactionTypes.IsCreditTransaction(@event.TransactionType);

            if (!string.IsNullOrWhiteSpace(@event.AccountId))
                await UpdateMonthlyTotalAsync(
                    @event.UserId,
                    @event.AccountId,
                    EntityTypes.Keys.ACCOUNT,
                    @event.Amount,
                    @event.DateOfTransaction,
                    isCreditTransaction,
                    cancellationToken);

            if (!string.IsNullOrWhiteSpace(@event.CategoryId))
                await UpdateMonthlyTotalAsync(
                    @event.UserId,
                    @event.CategoryId,
                    EntityTypes.Keys.CATEGORY,
                    @event.Amount,
                    @event.DateOfTransaction,
                    isCreditTransaction,
                    cancellationToken);

            if (!string.IsNullOrWhiteSpace(@event.PayerPayeeId))
                await UpdateMonthlyTotalAsync(
                    @event.UserId,
                    @event.PayerPayeeId,
                    EntityTypes.Keys.PAYER_PAYEE,
                    @event.Amount,
                    @event.DateOfTransaction,
                    isCreditTransaction,
                    cancellationToken);

            // This is the Total for the User across all Accounts
            if (!string.IsNullOrWhiteSpace(@event.UserId))
                await UpdateMonthlyTotalAsync(
                    @event.UserId,
                    @event.UserId,
                    EntityTypes.Keys.USER,
                    @event.Amount,
                    @event.DateOfTransaction,
                    isCreditTransaction,
                    cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update Account '{@AccountId}' for User '{@UserId}'", @event.AccountId, @event.UserId);
        }
    }

    /// <summary>
    /// Add/Update the MonthlyTotal for the specified Entity.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task UpdateMonthlyTotalAsync(
        string userId,
        string entityId,
        EntityTypes.Keys entityType,
        decimal amount,
        DateTime dateOfTransaction,
        bool isCreditTransaction,
        CancellationToken cancellationToken = default)
    {
        var monthlyTotalResult = await _repository
            .GetAsync(
                userId,
                entityId,
                entityType,
                dateOfTransaction.Year,
                dateOfTransaction.Month,
                cancellationToken);

        if (monthlyTotalResult.IsFailed)
            throw new InvalidOperationException(monthlyTotalResult.Errors[0]?.Message ??
                $"An error occurred while trying to retrieve {entityType} '{entityId}' " +
                $"Monthly Total ({dateOfTransaction.Year}/{dateOfTransaction.Month}) for User '{userId}'");

        var existingMonthlyTotal = monthlyTotalResult.ValueOrDefault;

        if (existingMonthlyTotal is null)
        {
            try
            {
                existingMonthlyTotal = MonthlyTotal.New(
                    new UserId(userId),
                    entityId,
                    entityType,
                    dateOfTransaction.Year,
                    dateOfTransaction.Month,
                    isCreditTransaction,
                    new Money(amount));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            // May want to add business logic to the MonthlyTotal entity to do the updates
            if (isCreditTransaction)
            {
                existingMonthlyTotal.CreditCount += 1;
                existingMonthlyTotal.CreditTotalAmount += new Money(amount);
            }
            else
            {
                existingMonthlyTotal.DebitCount += 1;
                existingMonthlyTotal.DebitTotalAmount += new Money(amount);
            }
        }

        try
        {
            var result = await _repository.AddAsync(existingMonthlyTotal, cancellationToken);

            if (result.IsFailed)
            {
                _logger.LogError("An error occurred while trying to update {EntityType} '{AccountId}' " +
                              "Monthly Total ({@Year}/{@Month}) for User '{@event.UserId}'.\n" +
                              "Error:{Message}",
                                userId, entityType, entityId, dateOfTransaction.Year, dateOfTransaction.Month, result.Errors[0]?.Message);

                return;
            }

            _logger.LogInformation("A(n) '{EntityType}' Monthly Total for '{EntityId}' was updated for user {UserId}}",
                entityType,
                entityId,
                userId);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "An error occurred while trying to update {EntityType} '{AccountId}' " +
                                "Monthly Total ({@Year}/{@Month}) for User '{@event.UserId}'.",
                                userId, entityType, entityId, dateOfTransaction.Year, dateOfTransaction.Month);
        }
    }
}