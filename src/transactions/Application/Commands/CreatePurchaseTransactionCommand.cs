using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreatePurchaseTransactionCommand(
    CreatePurchaseTransactionApiRequest ApiRequest) :
    ITransactionsCommand<Result<PurchaseTransactionDto>>;

public sealed class CreatePurchaseTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreatePurchaseTransactionCommand, Result<PurchaseTransactionDto>>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    ///private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    ///  throw new ArgumentNullException(nameof(eventDispatcher));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<PurchaseTransactionDto>> Handle(
        CreatePurchaseTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.ApiRequest;

        var transactionDoc = PurchaseTransaction.New(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            transactionRequest.Items
                .Select(i =>
                    TransactionItem.New(
                        new Money(i.Amount),
                        new CategoryId(i.CategoryId),
                        new SubCategoryId(i.SubCategoryId),
                        i.Description)
                ).ToList(),
            transactionRequest.TransactionDate,
            transactionRequest.Tags);

        var result = await _repository.AddAsync(transactionDoc, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Purchase Transaction");

        var transaction = result.Value;

        if (transaction is not PurchaseTransaction purchaseTransaction)
            throw new InvalidCastException("Failed to map Transaction to PurchaseTransaction");

        if (ApplicationMapper.Map(result.Value) is not PurchaseTransactionDto purchaseTransactionDto)
            throw new InvalidCastException("Failed to map PurchaseTransaction to PurchaseTransactionDto");

        // Iterate over all PurchaseTransaction Items and publish
        // TransactionCreatedIntegrationEvent for category/amount
        foreach (var transactionItem in purchaseTransaction.Items)
        {
            var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
                purchaseTransaction.Id,
                purchaseTransaction.UserId,
                purchaseTransaction.AccountId,
                // transactionItem
                transactionItem.CategoryId,
                transactionItem.SubCategoryId,
                purchaseTransaction.PayerPayeeId,
                purchaseTransaction.TransactionType,
                // transactionItem
                transactionItem.Amount,

                // Use transactionApiRequest.TransactionDate and not
                // transaction.TransactionDate (as it's Utc) ??
                transactionRequest.TransactionDate);

            await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);
        }

        return purchaseTransactionDto;
    }

    public class Validator : AbstractValidator<CreatePurchaseTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ApiRequest.UserId).NotEmpty();
            RuleFor(x => x.ApiRequest.AccountId).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionDate).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionType).NotNull();
            RuleFor(x => x.ApiRequest.Items).NotEmpty();
            RuleFor(x => x.ApiRequest.Items
                    .TrueForAll(i => i.Amount >= 0))
                .Equal(true);
        }
    }
}