using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;
using Habanerio.Xpnss.Shared.Requests.Transactions;

namespace Habanerio.Xpnss.Transactions.Application.Commands.Internals;

internal sealed record CreatePurchasesTransactionCommand(
    CreatePurchasesTransactionRequest Request) :
    ITransactionsCommand<Result<PurchasesTransactionDto>>;

internal sealed class CreatePurchasesTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreatePurchasesTransactionCommand, Result<PurchasesTransactionDto>>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    ///private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    ///  throw new ArgumentNullException(nameof(eventDispatcher));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<PurchasesTransactionDto>> Handle(
        CreatePurchasesTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var transactionEntity = PurchasesTransaction.New(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            transactionRequest.Description,
            transactionRequest.ExtTransactionNo,
            transactionRequest.Items
                .Select(i =>
                    TransactionItem.New(
                        new Money(i.Amount),
                        new CategoryId(i.CategoryId),
                        new SubCategoryId(i.SubCategoryId),
                        i.Description)
                ).ToList(),
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            //new RefTransactionId(transactionRequest.RefTransactionId),
            transactionRequest.Tags,
            transactionRequest.TransactionDate);

        var result = await _repository.AddAsync(transactionEntity, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Purchase Transaction");

        var transaction = result.Value;

        if (transaction is not PurchasesTransaction purchasesTransaction)
            throw new InvalidCastException($"Failed to add the {nameof(Transaction)}");

        if (ApplicationMapper.Map(result.Value) is not PurchasesTransactionDto purchasesTransactionDto)
            throw new InvalidCastException($"Failed to map {nameof(PurchasesTransaction)} to {nameof(PurchasesTransactionDto)}");

        // Iterate over all PurchasesTransaction Items and publish
        // TransactionCreatedIntegrationEvent for category/amount
        foreach (var transactionItem in purchasesTransaction.Items)
        {
            var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
                purchasesTransaction.Id.Value,
                purchasesTransaction.UserId.Value,
                purchasesTransaction.AccountId.Value,

                // transactionItem
                transactionItem.CategoryId.Value,
                transactionItem.SubCategoryId.Value,

                purchasesTransaction.PayerPayeeId.Value,
                purchasesTransaction.TransactionType,

                // transactionItem
                transactionItem.Amount.Value,

                transactionRequest.TransactionDate);

            await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);
        }

        return purchasesTransactionDto;
    }

    public class Validator : AbstractValidator<CreatePurchasesTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty();
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