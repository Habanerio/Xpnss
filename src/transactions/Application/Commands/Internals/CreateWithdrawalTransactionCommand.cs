using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands.Internals;

internal sealed record CreateWithdrawalTransactionCommand(
    CreateWithdrawalTransactionApiRequest ApiRequest) :
    ITransactionsCommand<Result<WithdrawalTransactionDto>>;

/// <summary>
/// Handles the creation of a Withdrawal transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreateWithdrawalTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreateWithdrawalTransactionCommand, Result<WithdrawalTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<WithdrawalTransactionDto>> Handle(
        CreateWithdrawalTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.ApiRequest;

        var withdrawalDoc = DebitTransaction.NewWithdrawal(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            new RefTransactionId(transactionRequest.RefTransactionId),
            transactionRequest.TransactionDate,
            transactionRequest.Tags,
            transactionRequest.ExtTransactionId);

        var result = await _repository.AddAsync(withdrawalDoc, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ??
                $"Failed to save the {nameof(DebitTransaction)} transaction");

        if (ApplicationMapper.Map(result.Value) is not WithdrawalTransactionDto transactionDto)
            throw new InvalidCastException($"{nameof(CreateWithdrawalTransactionCommandHandler)}: " +
                $"Failed to map {nameof(DebitTransaction)} to {nameof(WithdrawalTransactionDto)}");

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

            // Use transactionApiRequest.TransactionDate and not
            // transaction.TransactionDate (as it's Utc) ??
            transactionRequest.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreateWithdrawalTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ApiRequest.UserId).NotEmpty();
            RuleFor(x => x.ApiRequest.AccountId).NotEmpty();
            RuleFor(x => x.ApiRequest.TotalAmount).GreaterThan(0);
            RuleFor(x => x.ApiRequest.Description).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionDate).NotEmpty();
        }
    }
}