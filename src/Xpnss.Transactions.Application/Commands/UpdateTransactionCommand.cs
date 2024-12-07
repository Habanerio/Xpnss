//using FluentResults;
//using FluentValidation;
//using Habanerio.Xpnss.Application.DTOs;
//using Habanerio.Xpnss.Transactions.Domain.Interfaces;
//using MediatR;

//namespace Habanerio.Xpnss.Transactions.Application.Commands;


//// Easiest way to Update a Transaction is to delete the existing one and create a new one????
//public record UpdateTransactionCommand(
//    string UserId,
//    string TransactionId,
//    string ExtAcctId,
//    IEnumerable<TransactionItemDto> Items,
//    DateTime TransactionDate,
//    string TransactionType,
//    string Description,
//    MerchantDto? PayerPayee) : ITransactionsCommand<Result<TransactionDto>>, IRequest;

//public class UpdateTransactionHandler(
//    ITransactionsRepository repository,
//    IMediator mediator
//    //,IEventDispatcher eventDispatcher
//    ) : IRequestHandler<UpdateTransactionCommand, Result<TransactionDto>>
//{
//    // Would like to use the following, but wasn't able to get it to work. Revisit later.
//    //private readonly IEventDispatcher _eventDispatcher = eventDispatcher ??
//    //                                                     throw new ArgumentNullException(nameof(eventDispatcher));

//    private readonly ITransactionsRepository _repository = repository ??
//           throw new ArgumentNullException(nameof(repository));
//    private readonly IMediator _mediator = mediator ??
//                                           throw new ArgumentNullException(nameof(mediator));

//    public async Task<Result<TransactionDto>> Handle(
//        UpdateTransactionCommand command,
//        CancellationToken cancellationToken)
//    {
//        var validator = new Validator();

//        var validationResult = await validator.ValidateAsync(command, cancellationToken);

//        if (!validationResult.IsValid)
//            return Result.Fail(validationResult.Errors[0].ErrorMessage);

//        var existingTransactionResult = await _repository.GetAsync(command.UserId, command.TransactionId, cancellationToken);

//        if (existingTransactionResult.IsFailed || existingTransactionResult.ValueOrDefault is null)
//            return Result.Fail(existingTransactionResult.Errors?[0].Message ??
//                               "Transaction not found");

//        var existingTransaction = existingTransactionResult.ValueOrDefault;
//        //var existingTransactionPayments = existingTransaction!.Payments;

//        var deleteTransactionCommand = new DeleteTransactionCommand(command.UserId, command.TransactionId);
//        var deleteTransactionResult = await _mediator.Send<Result>(deleteTransactionCommand, cancellationToken);

//        if (deleteTransactionResult.IsFailed)
//            return Result.Fail(deleteTransactionResult.Errors?[0].Message ?? "Could not update the Transaction");

//        var createTransactionCommand = new CreateTransactionCommand(
//            command.UserId,
//            command.ExtAcctId,
//            command.Items,
//            command.TransactionDate,
//            command.TransactionType,
//            command.Description,
//            command.PayerPayee);

//        var createTransactionResult = await _mediator.Send<Result<TransactionDto>>(createTransactionCommand, cancellationToken);

//        if (createTransactionResult.IsFailed || createTransactionResult.ValueOrDefault is null)
//            return Result.Fail(createTransactionResult.Errors?[0].Message ?? "Could not update the Transaction");

//        var transactionDto = createTransactionResult.ValueOrDefault;

//        return transactionDto;
//    }

//    public class Validator : AbstractValidator<UpdateTransactionCommand>
//    {
//        public Validator()
//        {
//            RuleFor(x => x.UserId).NotEmpty();
//            RuleFor(x => x.ExtAcctId).NotEmpty();
//            RuleFor(x => x.Items).NotEmpty();
//            RuleFor(x => x.TransactionDate).NotEmpty();
//            RuleFor(x => x.TransactionType).NotNull();
//        }
//    }
//}