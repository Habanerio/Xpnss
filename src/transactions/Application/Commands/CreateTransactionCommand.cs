using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Transactions.Application.Commands.Internals;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreateTransactionCommand(
    string UserId,
    CreateTransactionRequest Request) :
    ITransactionsCommand<Result<TransactionDto>>;

/// <summary>
/// Handles the creation of a Transactions
/// </summary>
/// <param name="mediator"></param>
public sealed class CreateTransactionCommandHandler(IMediator mediator) :
    IRequestHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(mediator);

        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        Result<TransactionDto> result;

        if (command.Request is CreatePurchaseTransactionApiRequest purchaseRequest)
        {
            result = await SendSpecificCommand
                <CreatePurchaseTransactionCommand, PurchaseTransactionDto>(
                new CreatePurchaseTransactionCommand(purchaseRequest),
                cancellationToken);
        }
        else if (command.Request is CreateDepositTransactionApiRequest depositRequest)
        {
            result = await SendSpecificCommand
                <CreateDepositTransactionCommand, DepositTransactionDto>(
                new CreateDepositTransactionCommand(depositRequest),
                cancellationToken);
        }
        else if (command.Request is CreateWithdrawalTransactionApiRequest withdrawalRequest)
        {
            result = await SendSpecificCommand
                <CreateWithdrawalTransactionCommand, WithdrawalTransactionDto>(
                    new CreateWithdrawalTransactionCommand(withdrawalRequest),
                    cancellationToken);
        }
        else
        {
            return Result.Fail($"{nameof(CreateTransactionCommandHandler)}: " +
            $"Invalid Transaction Request. '{command.Request.TransactionType}' " +
            $"is not (yet) a support type");
        }

        return result;
    }

    private async Task<Result<TransactionDto>> SendSpecificCommand<TCommand, TDto>(
        TCommand command,
        CancellationToken cancellationToken)
        where TCommand : ITransactionsCommand<Result<TDto>>
        where TDto : TransactionDto
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ??
                               "Failed to create the transaction");

        TransactionDto transactionDto = result.ValueOrDefault;

        if (transactionDto is null)
            throw new InvalidOperationException();

        return Result.Ok(transactionDto);
    }

    public class Validator : AbstractValidator<CreateTransactionCommand>
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