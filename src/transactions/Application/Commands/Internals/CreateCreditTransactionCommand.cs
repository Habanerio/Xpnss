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

internal sealed record CreateCreditTransactionCommand(
    CreateCreditTransactionRequest Request) :
    ITransactionsCommand<Result<CreditTransactionDto>>;

/// <summary>
/// Handles the creation of a Deposit transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreateCreditTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreateCreditTransactionCommand,
    Result<CreditTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CreditTransactionDto>> Handle(
        CreateCreditTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionRequest = command.Request;

        var transaction = CreditTransaction.New(
            new UserId(transactionRequest.UserId),
            transactionRequest.TransactionType,
            new AccountId(transactionRequest.AccountId),
            new Money(transactionRequest.Amount),
            new CategoryId(transactionRequest.CategoryId),
            transactionRequest.Description,
            transactionRequest.ExtTransactionNo,
            new PayerPayeeId(transactionRequest.PayerPayee.Id),
            //new RefTransactionId(transactionRequest.RefTransactionId),
            new SubCategoryId(transactionRequest.SubCategoryId),
            transactionRequest.Tags,
            transactionRequest.TransactionDate);

        var result = await _repository.AddAsync(transaction, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors?[0].Message ??
                $"Failed to save the {nameof(CreditTransaction)} transaction");

        if (ApplicationMapper.Map(result.Value) is not CreditTransactionDto transactionDto)
            throw new InvalidCastException($"{nameof(CreateCreditTransactionCommandHandler)}: " +
                $"Failed to map {nameof(CreditTransaction)} to {nameof(CreditTransactionDto)}");

        var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
            transactionDto.Id,
            transactionDto.UserId,
            transactionDto.AccountId,
            transactionDto.CategoryId,
            transactionDto.SubCategoryId,
            transactionDto.PayerPayeeId,
            transactionDto.TransactionType,
            transactionDto.TotalAmount,

            // Use transactionRequest.TransactionDate and not
            // transaction.TransactionDate (as it's Utc) ??
            transactionRequest.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreateCreditTransactionCommand>
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