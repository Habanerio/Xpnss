using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs.Transactions;
using Habanerio.Xpnss.Shared.IntegrationEvents.Transactions;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.ValueObjects;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Entities.Transactions;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Commands.Internals;

internal sealed record CreatePaymentTransactionCommand :
    ITransactionsCommand<Result<PaymentTransactionDto>>
{
    public CreateTransactionRequest Request { get; }

    public CreatePaymentTransactionCommand(CreatePaymentInTransactionRequest request) =>
        Request = request;

    public CreatePaymentTransactionCommand(CreatePaymentOutTransactionRequest request) =>
        Request = request;
}

// Should maybe have a PAYMENT_IN and PAYMENT_OUT Commands?

/// <summary>
/// Handles the creation of a Withdrawal transaction
/// </summary>
/// <param name="repository"></param>
internal sealed class CreatePaymentTransactionCommandHandler(
    ITransactionsRepository repository,
    IMediator mediator) :
    IRequestHandler<CreatePaymentTransactionCommand, Result<PaymentTransactionDto>>
{
    private readonly IMediator _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));

    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<PaymentTransactionDto>> Handle(
        CreatePaymentTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var request = command.Request;

        PaymentTransactionDto transactionDto;

        //TODO: Clean this up. This can be much simplified.
        if (request is CreatePaymentOutTransactionRequest paymentOutRequest)
        {
            var paymentMadeTransaction = PaymentOutTransaction.New(
                new UserId(paymentOutRequest.UserId),
                new AccountId(paymentOutRequest.AccountId),
                new Money(paymentOutRequest.TotalAmount),
                new CategoryId(paymentOutRequest.CategoryId),
                paymentOutRequest.Description,
                paymentOutRequest.ExtTransactionNo,
                paymentOutRequest.IsToOwnAccount,
                new PayerPayeeId(paymentOutRequest.PayerPayee.Id),
                //new RefTransactionId(transactionRequest.RefTransactionId),
                new SubCategoryId(paymentOutRequest.SubCategoryId),
                paymentOutRequest.Tags,
                paymentOutRequest.Title,
                paymentOutRequest.TransactionDate);

            var paymentMadeResult = await _repository.AddAsync(paymentMadeTransaction, cancellationToken);

            if (paymentMadeResult.IsFailed || paymentMadeResult.ValueOrDefault is null)
                return Result.Fail(paymentMadeResult.Errors?[0].Message ??
                    $"Failed to save the {nameof(PaymentOutTransaction)} transaction");

            if (ApplicationMapper.Map(paymentMadeResult.Value) is not PaymentOutTransactionDto paymentDto)
                throw new InvalidCastException($"{nameof(CreatePaymentTransactionCommandHandler)}: " +
                    $"Failed to map {nameof(PaymentOutTransaction)} to {nameof(PaymentOutTransactionDto)}");

            transactionDto = paymentDto;
        }
        else if (request is CreatePaymentInTransactionRequest paymentInRequest)
        {
            var paymentReceivedTransaction = PaymentInTransaction.New(
                new UserId(paymentInRequest.UserId),
                new AccountId(paymentInRequest.AccountId),
                new Money(paymentInRequest.TotalAmount),
                new CategoryId(paymentInRequest.CategoryId),
                paymentInRequest.Description,
                paymentInRequest.ExtTransactionNo,
                paymentInRequest.IsFromOwnAccount,
                new PayerPayeeId(paymentInRequest.PayerPayee.Id),
                //new RefTransactionId(transactionRequest.RefTransactionId),
                new SubCategoryId(paymentInRequest.SubCategoryId),
                paymentInRequest.Tags,
                paymentInRequest.Title,
                paymentInRequest.TransactionDate);

            var paymentReceivedResult = await _repository.AddAsync(paymentReceivedTransaction, cancellationToken);

            if (paymentReceivedResult.IsFailed || paymentReceivedResult.ValueOrDefault is null)
                return Result.Fail(paymentReceivedResult.Errors?[0].Message ??
                    $"Failed to save the {nameof(PaymentInTransaction)} transaction");

            if (ApplicationMapper.Map(paymentReceivedResult.Value) is not PaymentInTransactionDto paymentDto)
                throw new InvalidCastException($"{nameof(CreatePaymentTransactionCommandHandler)}: " +
                    $"Failed to map {nameof(PaymentInTransaction)} to {nameof(PaymentInTransactionDto)}");

            transactionDto = paymentDto;
        }
        else
        {
            throw new InvalidOperationException("Invalid Payment Transaction Request");
        }

        //TODO: Create a `PaymentTransactionCreatedIntegrationEvent`
        // and try to update the account that the transaction was deposited into?
        // Or should I just let the user do it?
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
            request.TransactionDate);

        await _mediator.Publish(transactionCreatedIntegrationEvent, cancellationToken);

        return transactionDto;
    }

    public class Validator : AbstractValidator<CreatePaymentTransactionCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.UserId).NotEmpty();
            RuleFor(x => x.Request.AccountId).NotEmpty();
            RuleFor(x => x.Request.TotalAmount).GreaterThan(0);
            RuleFor(x => x.Request.Description).NotEmpty();
            RuleFor(x => x.Request.TransactionDate).NotEmpty();
        }
    }
}