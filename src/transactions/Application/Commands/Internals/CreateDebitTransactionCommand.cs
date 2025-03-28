using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs.Transactions;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands.Internals;

internal sealed record CreateDebitTransactionCommand(
    CreateDebitTransactionRequest Request) :
    ITransactionsCommand<Result<DebitTransactionDto>>;

/// <summary>
/// Handles the creation of a Withdrawal transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreateDebitTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreateDebitTransactionCommand, Result<DebitTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<DebitTransactionDto>> Handle(
        CreateDebitTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var withdrawalDoc = DebitTransaction.New(
            new UserId(transactionRequest.UserId),
            transactionRequest.TransactionType,
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            new CategoryId(transactionRequest.CategoryId),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            //new RefTransactionId(transactionRequest.RefTransactionId),
            new SubCategoryId(transactionRequest.SubCategoryId),
            transactionRequest.Title,
            transactionRequest.TransactionDate,
            transactionRequest.Tags);

        var result = await _repository.AddAsync(withdrawalDoc, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ??
                $"Failed to save the {nameof(DebitTransaction)} transaction");

        if (ApplicationMapper.Map(result.Value) is not DebitTransactionDto transactionDto)
            throw new InvalidCastException($"{nameof(CreateDebitTransactionCommandHandler)}: " +
                $"Failed to map {nameof(DebitTransaction)} to {nameof(DebitTransactionDto)}");

        //TODO: Create a `WithdrawalTransactionCreatedIntegrationEvent`
        // and try to update the account that the transaction was deposited into?
        // Or should I just let the user do it?
        var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
            transactionDto.Id,
            transactionDto.UserId,
            transactionDto.AccountId,
            string.Empty,
            string.Empty,
            transactionDto.PayerPayeeId,
            transactionDto.TransactionType,
            transactionDto.TotalAmount,

            // Use transactionRequest.TransactionDate and not
            // transaction.TransactionDate (as it's Utc) ??
            transactionRequest.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreateDebitTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty();
            RuleFor(x => x.Request.AccountId).NotEmpty();
            RuleFor(x => x.Request.TotalAmount).GreaterThan(0);
            RuleFor(x => x.Request.Description).NotEmpty();
            RuleFor(x => x.Request.TransactionDate).NotEmpty();
        }
    }
}