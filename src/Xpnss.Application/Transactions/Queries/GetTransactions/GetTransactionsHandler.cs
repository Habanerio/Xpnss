using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsHandler(ITransactionsRepository repository) : IRequestHandler<GetTransactionsQuery, Result<IEnumerable<TransactionDto>>>
{
    private readonly ITransactionsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
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
            request.TimeZone,
            cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        if (!docsResult.Value.Any())
            return Result.Ok(Enumerable.Empty<TransactionDto>());

        var dtos = Mapper.Map(docsResult.Value);

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetTransactionsQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}