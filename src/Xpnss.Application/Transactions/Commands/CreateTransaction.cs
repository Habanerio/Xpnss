using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Application.Merchants.DTOs;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Events;
using Habanerio.Xpnss.Domain.Transactions;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Transactions.Commands;

public record CreateTransactionCommand(
    string UserId,
    string AccountId,
    IEnumerable<TransactionItemDto> Items,
    DateTime TransactionDate,
    string TransactionType,
    string Description = "",
    MerchantDto? Merchant = null) : ITransactionsCommand<Result<TransactionDto>>, IRequest;

public class CreateTransactionHandler(
    ITransactionsRepository repository,
    //IEventDispatcher eventDispatcher,
    IMediator mediator) : IRequestHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    // Would like to use the following, but wasn't able to get it to work. Revisit later.
    //private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
    //                                                     throw new ArgumentNullException(nameof(eventDispatcher));

    private readonly IMediator _mediator = mediator ??
                                           throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

    public async Task<Result<TransactionDto>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var transactionDoc = Transaction.New(
            new UserId(command.UserId),
            new AccountId(command.AccountId),
            TransactionTypes.ToTransactionType(command.TransactionType),
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

        var transactionDto = Mapper.Map(result.Value);

        if (transactionDto is null)
            return Result.Fail("Failed to map TransactionDocument to TransactionDto");

        foreach (var @event in transactionDoc.DomainEvents)
        {
            //await _eventDispatcher.DispatchAsync(@event);
            await _mediator.Send(@event, cancellationToken);
        }

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreateTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Items).NotEmpty();
            RuleFor(x => x.TransactionDate).NotEmpty();
            RuleFor(x => x.TransactionType).NotNull();
        }
    }
}