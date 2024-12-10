using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Shared.Requests;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands.Internals;

internal sealed record CreateDepositTransactionCommand(
    CreateDepositTransactionApiRequest ApiRequest) :
    ITransactionsCommand<Result<CreditTransactionDto>>;

/// <summary>
/// Handles the creation of a Deposit transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreateDepositTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreateDepositTransactionCommand,
    Result<CreditTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CreditTransactionDto>> Handle(
        CreateDepositTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.ApiRequest;

        var transaction = CreditTransaction.NewDeposit(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            transactionRequest.TransactionDate,
            transactionRequest.Tags,
            transactionRequest.ExtTransactionId);

        var result = await _repository.AddAsync(transaction, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Deposit Transaction");

        var transactionDto = ApplicationMapper.Map(result.Value);

        if (transactionDto == null)
            throw new InvalidCastException($"{nameof(CreateDepositTransactionCommandHandler)}: " +
                $"Failed to map {nameof(CreditTransaction)} to {nameof(DepositTransactionDto)}");

        if (transactionDto is not CreditTransactionDto creditDto)
            // || depositTransactionDto.TransactionType.Equals(TransactionEnums.TransactionKeys.DEPOSIT))
            throw new InvalidCastException($"{nameof(CreateDepositTransactionCommandHandler)}: " +
                $"Failed to map {nameof(CreditTransaction)} to {nameof(DepositTransactionDto)}");

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

        return creditDto;
    }

    public class Validator : AbstractValidator<CreateDepositTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ApiRequest.UserId).NotEmpty();
            RuleFor(x => x.ApiRequest.AccountId).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionDate).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionType).NotNull();
        }
    }
}