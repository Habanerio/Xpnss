using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public record DeleteTransactionCommand(
    string UserId,
    string TransactionId) : ITransactionsCommand<Result>, IRequest;

public class DeleteTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator
    //,IEventDispatcher eventDispatcher
    ) : IRequestHandler<DeleteTransactionCommand, Result>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    //private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    //                                                     throw new ArgumentNullException(nameof(eventDispatcher));

    private readonly ITransactionsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));
    private readonly IMediator _mediator = mediator ??
                                           throw new ArgumentNullException(nameof(mediator));

    public async Task<Result> Handle(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionResult = await _repository.GetAsync(command.UserId, command.TransactionId, cancellationToken);

        if (transactionResult.IsFailed || transactionResult.ValueOrDefault is null)
            return Result.Fail(transactionResult.Errors?[0].Message ?? "Could not find the Transaction");

        var transaction = transactionResult.ValueOrDefault;

        if (transaction.IsDeleted)
            return Result.Fail("Transaction is already deleted!");

        transaction.Delete();

        var transactionCreatedIntegrationEvent = new TransactionDeletedIntegrationEvent(
            transaction.Id.Value,
            transaction.UserId.Value,
            transaction.AccountId.Value,
            transaction.MerchantId.Value,
            transaction.TransactionType,
            transaction.TotalAmount,
            transaction.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        var updatedTransaction = await _repository.UpdateAsync(transaction, cancellationToken);

        if (updatedTransaction.IsFailed)
            return Result.Fail(updatedTransaction.Errors[0].Message);

        return updatedTransaction.Value.IsDeleted ?
            Result.Ok() :
            Result.Fail("Transaction was not deleted");
    }

    public class Validator : AbstractValidator<DeleteTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
}