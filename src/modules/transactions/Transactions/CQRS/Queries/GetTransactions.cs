using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Transactions.DTOs;
using Habanerio.Xpnss.Modules.Transactions.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Transactions.CQRS.Queries;

public class GetTransactions
{
    public record Query(
        string UserId,
        string AccountId = "",
        DateTimeOffset? FromDate = null,
        DateTimeOffset? ToDate = null) : ITransactionsQuery<Result<IEnumerable<TransactionDto>>>;

    public class Handler : IRequestHandler<Query, Result<IEnumerable<TransactionDto>>>
    {
        private readonly ITransactionsRepository _repository;

        public Handler(ITransactionsRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<IEnumerable<TransactionDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var docsResult = await _repository.ListAsync(
                request.UserId,
                request.AccountId,
                request.FromDate,
                request.ToDate,
                cancellationToken);

            if (docsResult.IsFailed)
                return Result.Fail(docsResult.Errors);

            if (!docsResult.Value.Any())
                return Result.Ok(Enumerable.Empty<TransactionDto>());

            var dtos = Mappers.DocumentToDtoMappings.Map(docsResult.Value);

            return Result.Ok(dtos);
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty();
            }
        }
    }
}