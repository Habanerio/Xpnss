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

public sealed record CreateWithdrawalCommand(CreateWithdrawalTransactionApiRequest ApiRequest) :
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
            new UserId(command.ApiRequest.UserId),
            new AccountId(command.ApiRequest.AccountId),
            new Money(command.ApiRequest.TotalAmount),
            command.ApiRequest.Description,
            new PayerPayeeId(command.ApiRequest.PayerPayee.Id),
            command.ApiRequest.TransactionDate,
            command.ApiRequest.Tags,
            command.ApiRequest.ExtTransactionId);

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
            RuleFor(x => x.ApiRequest.UserId).NotEmpty();
            RuleFor(x => x.ApiRequest.AccountId).NotEmpty();
            RuleFor(x => x.ApiRequest.TotalAmount).GreaterThan(0);
            RuleFor(x => x.ApiRequest.Description).NotEmpty();
            RuleFor(x => x.ApiRequest.TransactionDate).NotEmpty();
        }
    }
}