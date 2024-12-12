using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record DeleteTransactionCommand(
    string UserId,
    string TransactionId) : ITransactionsCommand<Result>, IRequest;

public sealed class DeleteTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator
    //,IEventDispatcher eventDispatcher
    ) : IRequestHandler<DeleteTransactionCommand, Result>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    //private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    //  throw new ArgumentNullException(nameof(eventDispatcher));

    public async Task<Result> Handle(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(repository);

        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionResult = await repository.GetAsync(command.UserId, command.TransactionId, cancellationToken);

        if (transactionResult.IsFailed || transactionResult.ValueOrDefault is null)
            return Result.Fail(transactionResult.Errors?[0].Message ?? "Could not find the Transaction");

        var transaction = transactionResult.ValueOrDefault;

        // Do we want to check the Account Id?
        //if(!transaction.AccountId.Value.Equals(command.AccountId))

        if (transaction.IsDeleted)
            return Result.Fail("Transaction is already deleted!");

        transaction.Delete();

        if (!transaction.IsDeleted)
            return Result.Fail("Transaction was not deleted");

        var updatedTransaction = await repository.UpdateAsync(transaction, cancellationToken);

        if (updatedTransaction.IsFailed)
            return Result.Fail(updatedTransaction.Errors[0].Message);

        if (!updatedTransaction.Value.IsDeleted)
            Result.Fail("Transaction was not deleted");

        var transactionDeletedIntegrationEvent = new TransactionDeletedIntegrationEvent(
            transaction.Id.Value,
            transaction.UserId.Value,
            transaction.AccountId.Value,
            transaction.PayerPayeeId.Value,
            transaction.TransactionType,
            transaction.TotalAmount.Value,
            transaction.TransactionDate);

        await mediator.Publish(transactionDeletedIntegrationEvent, cancellationToken);

        return Result.Ok();
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