using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreateDepositTransactionCommand(CreateDepositTransactionRequest Request) :
    ITransactionsCommand<Result<DepositTransactionDto>>;

/// <summary>
/// Handles the creation of a Deposit transaction
/// </summary>
/// <param name="repository"></param>
public sealed class CreateDepositTransactionCommandHandler(
    ITransactionsRepository repository) :
    IRequestHandler<CreateDepositTransactionCommand,
    Result<DepositTransactionDto>>
{
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

        var transactionDoc = DepositTransaction.New(
            new UserId(transactionRequest.UserId),
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.TotalAmount),
            transactionRequest.Description,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            transactionRequest.TransactionDate,
            transactionRequest.Tags);

        var result = await _repository.AddAsync(transactionDoc, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Deposit Transaction");

        if (ApplicationMapper.Map(result.Value) is not DepositTransactionDto depositTransactionDto)
            throw new InvalidCastException("Failed to map DepositTransaction to DepositTransactionDto");

        return depositTransactionDto;
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