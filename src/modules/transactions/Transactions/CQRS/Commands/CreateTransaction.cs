using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Transactions.Common;
using Habanerio.Xpnss.Modules.Transactions.Data;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions.CQRS.Commands;

public class CreateTransaction
{
    public record Command(
        string UserId,
        string AccountId,
        IEnumerable<TransactionItemDto> Items,
        DateTime TransactionDate,
        string TransactionType,
        string Description = "",
        MerchantDto? Merchant = null) : ITransactionsCommand<Result<TransactionDto>>, IRequest;

    public class Handler(ITransactionsRepository repository) : IRequestHandler<Command, Result<TransactionDto>>
    {
        private readonly ITransactionsRepository _repository = repository ??
                                                               throw new ArgumentNullException(nameof(repository));

        public async Task<Result<TransactionDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            TransactionMerchant? merchant = null;

            if (request.Merchant != null)
            {
                if (string.IsNullOrWhiteSpace(request.Merchant.Id))
                    merchant = TransactionMerchant.New(request.Merchant.Name, request.Merchant.Location);
                else
                    merchant = new TransactionMerchant(request.Merchant.Id, request.Merchant.Name, request.Merchant.Location);
            }

            var transactionDoc = TransactionDocument.New(
                request.UserId,
                request.AccountId,
                request.TransactionDate,
                request.Items
                    .Select(i =>
                        TransactionItem.New(
                            i.Amount,
                            i.Description,
                            i.CategoryId)
                    ).ToList(),
                (TransactionTypes)Enum.Parse(typeof(TransactionTypes), request.TransactionType),
                request.Description,
                merchant);

            var result = await _repository.AddAsync(transactionDoc, cancellationToken);

            if (result.IsFailed)
                return Result.Fail(result.Errors?[0].Message ?? "Could not save the Transaction");

            var transactionDto = Mappers.DocumentToDtoMappings.Map(result.Value);

            if (transactionDto is null)
                return Result.Fail("Failed to map TransactionDocument to TransactionDto");

            return transactionDto;
        }
    }

    public class Validator : AbstractValidator<Command>
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