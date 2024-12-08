using System.Text.Json;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.MonthlyTotals.Domain.Entities;
using Habanerio.Xpnss.MonthlyTotals.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Habanerio.Xpnss.MonthlyTotals.Infrastructure.IntegrationEvents.EventHandlers;

/// <summary>
/// Responsible for handling the `TransactionCreatedIntegrationEvent` and updating the MonthlyTotals.
/// </summary>
// TODO: May need to rethink this. May need separate Handlers for Transactions with, and without items
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
            await UpdateAccountMonthlyTotalAsync(@event, cancellationToken);
            await UpdateCategoryMonthlyTotalAsync(@event, cancellationToken);
            await UpdatePayerPayeeMonthlyTotal(@event, cancellationToken);
            await UpdateUserMonthlyTotal(@event, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while trying to update Account '{AccountId}' for User '{UserId}'", @event.AccountId, @event.UserId);
        }
    }

    /// <summary>
    /// Attempt to update the MonthlyTotal for the Account.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task UpdateAccountMonthlyTotalAsync(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var isCreditTransaction = TransactionEnums.IsCreditTransaction(@event.TransactionType);

        if (!string.IsNullOrWhiteSpace(@event.AccountId))
            await UpdateMonthlyTotalAsync(
                @event.UserId,
                @event.AccountId,
                string.Empty,
                EntityEnums.Keys.ACCOUNT,
                @event.Amount,
                @event.DateOfTransaction,
            isCreditTransaction,
                cancellationToken);
    }

    /// <summary>
    /// Attempt to update the MonthlyTotal for the Category and SubCategory.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task UpdateCategoryMonthlyTotalAsync(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var isCreditTransaction = TransactionEnums.IsCreditTransaction(@event.TransactionType);

        // Update the Parent Category Monthly Total
        if (!string.IsNullOrWhiteSpace(@event.CategoryId))
            await UpdateMonthlyTotalAsync(
                @event.UserId,
                @event.CategoryId,
                string.Empty,
                EntityEnums.Keys.CATEGORY,
                @event.Amount,
                @event.DateOfTransaction,
                isCreditTransaction,
                cancellationToken);

        // Update the SubCategory Monthly Total (but place it under "Category")
        if (!string.IsNullOrWhiteSpace(@event.SubCategoryId))
            await UpdateMonthlyTotalAsync(
                @event.UserId,
                @event.CategoryId,
                @event.SubCategoryId,
                EntityEnums.Keys.CATEGORY,
                @event.Amount,
                @event.DateOfTransaction,
                isCreditTransaction,
                cancellationToken);
    }

    /// <summary>
    /// Attempt to update the MonthlyTotal for the PayerPayee.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task UpdatePayerPayeeMonthlyTotal(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var isCreditTransaction = TransactionEnums.IsCreditTransaction(@event.TransactionType);

        if (!string.IsNullOrWhiteSpace(@event.PayerPayeeId))
            await UpdateMonthlyTotalAsync(
                @event.UserId,
                @event.PayerPayeeId,
                string.Empty,
                EntityEnums.Keys.PAYER_PAYEE,
                @event.Amount,
                @event.DateOfTransaction,
                isCreditTransaction,
                cancellationToken);
    }

    /// <summary>
    /// Attempt to update the MonthlyTotal for the User.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task UpdateUserMonthlyTotal(
        TransactionCreatedIntegrationEvent @event,
        CancellationToken cancellationToken = default)
    {
        var isCreditTransaction = TransactionEnums.IsCreditTransaction(@event.TransactionType);

        if (!string.IsNullOrWhiteSpace(@event.UserId))
            await UpdateMonthlyTotalAsync(
                @event.UserId,
                @event.UserId,
                string.Empty,
                EntityEnums.Keys.USER,
                @event.Amount,
                @event.DateOfTransaction,
                isCreditTransaction,
                cancellationToken);
    }

    /// <summary>
    /// Add/Update the MonthlyTotal for the specified Entity.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task UpdateMonthlyTotalAsync(
        string userId,
        string entityId,
        string subEntityId,
        EntityEnums.Keys entityType,
        decimal amount,
        DateTime dateOfTransaction,
        bool isCreditTransaction,
        CancellationToken cancellationToken = default)
    {
        MonthlyTotal? existingMonthlyTotal = null;

        try
        {
            var monthlyTotalResult = await _repository
                .GetAsync(
                    userId,
                    entityId,
                    subEntityId,
                    entityType,
                    dateOfTransaction.Year,
                    dateOfTransaction.Month,
                    cancellationToken);

            if (monthlyTotalResult.IsFailed)
                throw new InvalidOperationException(monthlyTotalResult.Errors[0]?.Message ??
                    $"An error occurred while trying to retrieve {entityType} '{entityId}' " +
                    $"Monthly Total ({dateOfTransaction.Year}/{dateOfTransaction.Month}) for User '{userId}'");

            existingMonthlyTotal = monthlyTotalResult.ValueOrDefault;
        }
        catch (Exception e)
        {
            throw;
        }


        if (existingMonthlyTotal is null)
        {
            try
            {
                existingMonthlyTotal = MonthlyTotal.New(
                    new UserId(userId),
                    new EntityObjectId(entityId),
                    !string.IsNullOrWhiteSpace(subEntityId) ?
                        new EntityObjectId(subEntityId) :
                        EntityObjectId.Empty,
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

        var json = JsonSerializer.Serialize(existingMonthlyTotal);

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

            var y = true;
            //_logger.LogInformation("A(n) '{EntityType}' Monthly Total for '{EntityId}' was updated for user {UserId}}",
            //    entityType,
            //    entityId,
            //    userId);
        }
        catch (Exception e)
        {
            var x = true;
            //_logger.LogCritical(e, "An error occurred while trying to update {EntityType} '{AccountId}' " +
            //    "Monthly Total ({@Year}/{@Month}) for User '{@event.UserId}'. Data: {Json}",
            //    userId, entityType, entityId, dateOfTransaction.Year, dateOfTransaction.Month, json);
        }
    }
}