using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Application.Requests;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Queries.GetTransactions;

public sealed record GetTransactionsQuery(SearchTransactionsApiRequest Request) :
    ITransactionsQuery<Result<IEnumerable<TransactionDto>>>;

public class GetTransactionsHandler(ITransactionsRepository repository) :
    IRequestHandler<GetTransactionsQuery, Result<IEnumerable<TransactionDto>>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<TransactionDto>>> Handle(
        GetTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var request = query.Request;

        if (request.FromDate is null
            || request.FromDate?.Date.ToUniversalTime() > (request.ToDate ?? DateTime.UtcNow))
            request.FromDate = DateTime.UtcNow.AddDays(-30);

        if (request.ToDate is null
            || request.ToDate?.Date.ToUniversalTime() > DateTime.UtcNow)
            request.ToDate = DateTime.UtcNow;

        var docsResult = await _repository.FindAsync(
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

        var dtos = ApplicationMapper.Map(docsResult.Value);

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetTransactionsQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Request.UserId).NotEmpty();
            RuleFor(x => x.Request.FromDate)
                .LessThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.Request.ToDate)
                .GreaterThanOrEqualTo(x => x.Request.FromDate);
        }
    }
}