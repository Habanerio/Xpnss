using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreateDepositTransactionCommand(
    string UserId,
    CreateDepositTransactionRequest Request) :
    ITransactionsCommand<Result<DepositTransactionDto>>;

public sealed class CreateDepositTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) : IRequestHandler<CreateDepositTransactionCommand,
    Result<DepositTransactionDto>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<DepositTransactionDto>> Handle(
        CreateDepositTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var transactionDoc = DepositTransaction.New(
            new UserId(command.UserId),
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            transactionRequest.TransactionDate,
            transactionRequest.Tags);

        var result = await _repository.AddAsync(transactionDoc, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Deposit Transaction");

        var transaction = result.Value;

        if (ApplicationMapper.Map(result.Value) is not DepositTransactionDto depositTransactionDto)
            return Result.Fail("Failed to map DepositTransaction to DepositTransactionDto");

        var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
            depositTransactionDto.Id,
            depositTransactionDto.UserId,
            depositTransactionDto.AccountId,
            string.Empty,
            depositTransactionDto.PayerPayeeId,
            transaction.TransactionType,
            depositTransactionDto.TotalAmount,
            // Use transactionRequest.TransactionDate and not transaction.TransactionDate (as it's Utc) ??
            transactionRequest.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return depositTransactionDto;
    }

    public class Validator : AbstractValidator<CreateDepositTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Request.AccountId).NotEmpty();
            RuleFor(x => x.Request.TransactionDate).NotEmpty();
            RuleFor(x => x.Request.TransactionType).NotNull();
        }
    }
}