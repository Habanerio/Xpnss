using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.Types;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.Infrastructure.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands;

public record CreatePurchaseTransactionCommand(
    string UserId,
    string AccountId,
    IEnumerable<TransactionItemDto> Items,
    DateTime TransactionDate,
    string Description = "",
    MerchantDto? Merchant = null) : ITransactionsCommand<Result<TransactionDto>>, IRequest;

public class CreatePurchaseTransactionHandler(
    ITransactionsRepository repository,
    IMediator mediator
    ) : IRequestHandler<CreatePurchaseTransactionCommand, Result<TransactionDto>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    public async Task<Result<TransactionDto>> Handle(
        CreatePurchaseTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionDoc = Transaction.New(
            new UserId(command.UserId),
            new AccountId(command.AccountId),
            TransactionTypes.Keys.PURCHASE,
            command.Description,
            new MerchantId(command.Merchant?.Id),
            command.Items
                .Select(i =>
                    TransactionItem.New(
                        new Money(i.Amount),
                        new CategoryId(i.CategoryId),
                        i.Description)
                ).ToList(),
            command.TransactionDate);

        var result = await _repository.AddAsync(transactionDoc, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors?[0].Message ?? "Could not save the Transaction");

        var transaction = result.Value;

        var transactionDto = Mapper.Map(result.Value);

        if (transactionDto is null)
            return Result.Fail("Failed to map TransactionDocument to TransactionDto");

        var transactionCreatedIntegrationEvent = new TransactionCreatedIntegrationEvent(
            transaction.Id.Value,
            transaction.UserId.Value,
            transaction.AccountId.Value,
            transaction.MerchantId.Value,
            transaction.TransactionType,
            transaction.TotalAmount,
            // Use command.TransactionDate and not transaction.TransactionDate (as it's Utc)
            command.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreatePurchaseTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Items).NotEmpty();
            RuleFor(x => x.TransactionDate).NotEmpty();
        }
    }
}