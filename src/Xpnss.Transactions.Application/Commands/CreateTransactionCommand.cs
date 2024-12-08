using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public sealed record CreateTransactionCommand(CreateTransactionApiRequest Request) :
    ITransactionsCommand<Result<TransactionDto>>;

/// <summary>
/// Handles the creation of a Transactions
/// </summary>
/// <param name="mediator"></param>
public sealed class CreateTransactionCommandHandler(IMediator mediator) :
    IRequestHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<TransactionDto>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        TransactionDto transactionDto;

        if (command.Request is CreatePurchaseTransactionApiRequest purchaseRequest)
        {
            //// Allowed to NOT assign a Category to a Purchase,
            //// but if a Category is assigned, a SubCategory must also be assigned.
            //// Temporary may? This is due to updating the Monthly Totals.
            //// Maybe need a TransactionItemCreatedEvent?
            //if (purchaseRequest.Items
            //    .Any(i =>
            //        !string.IsNullOrWhiteSpace(i.CategoryId) &&
            //        string.IsNullOrWhiteSpace(i.SubCategoryId)))
            //{
            //    return Result.Fail("Cannot assign a Purchase to a Parent Category.");
            //}

            var newCommand = new CreatePurchaseTransactionCommand(purchaseRequest);
            var result = await _mediator.Send(newCommand, cancellationToken);

            if (result.IsFailed || result.ValueOrDefault is null)
                return Result.Fail(result.Errors?[0].Message ??
                                   "Failed to create purchase transaction");

            transactionDto = result.ValueOrDefault;

            if (transactionDto is null)
                return Result.Fail("Failed to create purchase transaction");
        }
        //else if (command.Request is CreateDepositTransactionApiRequest depositRequest)
        //{

        //}
        else
        {
            return Result.Fail($"Invalid Transaction Request. " +
                               $"'{command.Request.TransactionType}' is not (yet) a support type");
        }

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