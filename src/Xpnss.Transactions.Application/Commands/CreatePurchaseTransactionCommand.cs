using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreatePurchaseTransactionCommand(
    string UserId,
    CreatePurchaseTransactionRequest Request) :
    ITransactionsCommand<Result<PurchaseTransactionDto>>;

public sealed class CreatePurchaseTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator) : IRequestHandler<CreatePurchaseTransactionCommand, Result<PurchaseTransactionDto>>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    ///private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    ///  throw new ArgumentNullException(nameof(eventDispatcher));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<PurchaseTransactionDto>> Handle(CreatePurchaseTransactionCommand command, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var transactionDoc = PurchaseTransaction.New(
            new UserId(command.UserId),
            new AccountId(transactionRequest.AccountId),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            transactionRequest.Items
                .Select(i =>
                    TransactionItem.New(
                        new Money(i.Amount),
                        new CategoryId(i.CategoryId),
                        i.Description)
                ).ToList(),
            transactionRequest.TransactionDate,
            transactionRequest.Tags);

        var result = await _repository.AddAsync(transactionDoc, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Purchase Transaction");

        var transaction = result.Value;

        if (transaction is not PurchaseTransaction purchaseTransaction)
            return Result.Fail("Failed to map Transaction to PurchaseTransaction");

        if (ApplicationMapper.Map(result.Value) is not PurchaseTransactionDto purchaseTransactionDto)
            return Result.Fail("Failed to map PurchaseTransaction to PurchaseTransactionDto");

        // Iterate over all PurchaseTransaction Items and publish TransactionCreatedIntegrationEvent for category/amount
        foreach (var transactionItem in purchaseTransaction.Items)
        {
            var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
                purchaseTransaction.Id,
                purchaseTransaction.UserId,
                purchaseTransaction.AccountId,
                // transactionItem
                transactionItem.CategoryId,
                purchaseTransaction.PayerPayeeId,
                purchaseTransaction.TransactionType,
                // transactionItem
                transactionItem.Amount,

                // Use transactionRequest.TransactionDate and not transaction.TransactionDate (as it's Utc) ??
                transactionRequest.TransactionDate);

            await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);
        }

        return purchaseTransactionDto;
    }

    public class Validator : AbstractValidator<CreatePurchaseTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Request.AccountId).NotEmpty();
            RuleFor(x => x.Request.TransactionDate).NotEmpty();
            RuleFor(x => x.Request.TransactionType).NotNull();
            RuleFor(x => x.Request.Items).NotEmpty();
            RuleFor(x => x.Request.Items
                    .TrueForAll(i => i.Amount >= 0))
                .Equal(true);
        }
    }
}