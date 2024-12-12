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

internal sealed record CreateDepositTransactionCommand(
    CreateDepositTransactionRequest Request) :
    ITransactionsCommand<Result<DepositTransactionDto>>;

/// <summary>
/// Handles the creation of a Deposit transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreateDepositTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreateDepositTransactionCommand,
    Result<DepositTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<DepositTransactionDto>> Handle(
        CreateDepositTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var transaction = CreditTransaction.NewDeposit(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            transactionRequest.Description,
            transactionRequest.ExtTransactionId,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            new RefTransactionId(transactionRequest.RefTransactionId),
            transactionRequest.Tags,
            transactionRequest.TransactionDate);

        var result = await _repository.AddAsync(transaction, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ??
                $"Failed to save the {nameof(CreditTransaction)} transaction");

        if (ApplicationMapper.Map(result.Value) is not DepositTransactionDto transactionDto)
            throw new InvalidCastException($"{nameof(CreateDepositTransactionCommandHandler)}: " +
                $"Failed to map {nameof(CreditTransaction)} to {nameof(DepositTransactionDto)}");

        var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
            transactionDto.Id,
            transactionDto.UserId,
            transactionDto.AccountId,
            // Deposits don't have categories (?)
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

    public class Validator : AbstractValidator<CreateDepositTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty();
            RuleFor(x => x.Request.AccountId).NotEmpty();
            RuleFor(x => x.Request.TransactionDate).NotEmpty();
            RuleFor(x => x.Request.TransactionType).NotNull();
        }
    }
}