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

public sealed record CreateWithdrawalCommand(CreateWithdrawalTransactionRequest Request) :
    ITransactionsCommand<Result<WithdrawalTransactionDto>>;

/// <summary>
/// Handles the creation of a Withdrawal transaction
/// </summary>
/// <param name="repository"></param>
public sealed class CreateWithdrawalCommandHandler(
    ITransactionsRepository repository) :
    IRequestHandler<CreateWithdrawalCommand, Result<WithdrawalTransactionDto>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<WithdrawalTransactionDto>> Handle(
        CreateWithdrawalCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var withdrawalDoc = WithdrawalTransaction.New(
            new UserId(command.Request.UserId),
            new AccountId(command.Request.AccountId),
            new Money(command.Request.TotalAmount),
            command.Request.Description,
            new PayerPayeeId(command.Request.PayerPayee.Id),
            command.Request.TransactionDate,
            command.Request.Tags,
            command.Request.ExtTransactionId);

        var result = await _repository.AddAsync(withdrawalDoc, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ?? "Failed to create withdrawal transaction");

        if (ApplicationMapper.Map(result.Value) is not WithdrawalTransactionDto transactionDto)
            throw new InvalidCastException("Failed to map DepositTransaction to DepositTransactionDto");

        return Result.Ok(transactionDto);
    }

    public class Validator : AbstractValidator<CreateWithdrawalCommand>
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