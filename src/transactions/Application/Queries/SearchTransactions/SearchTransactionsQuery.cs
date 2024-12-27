using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.Requests.Transactions;
using Habanerio.Xpnss.Shared.Responses.Transactions;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Queries.SearchTransactions;

public sealed record SearchTransactionsQuery(SearchTransactionsRequest Request) :
    ITransactionsQuery<Result<TransactionsSearchResponse>>;

public class GetTransactionsHandler(ITransactionsRepository repository) :
    IRequestHandler<SearchTransactionsQuery, Result<TransactionsSearchResponse>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<TransactionsSearchResponse>> Handle(
        SearchTransactionsQuery query,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        // Need local DateTime Kind
        var request = query.Request;

        var pageNo = request.PageNo < 1 ? 1 : request.PageNo;
        var perPage = request.PerPage < 1 ? 1 : request.PerPage;

        var toDate = (request.ToDate is null ||
                     request.ToDate.Value.Date > DateTime.Now.Date ?
                DateTime.Now.Date :
                request.ToDate).Value.Date;

        var fromDate = request.FromDate is null ||
                       request.FromDate.Value.Date > toDate ?
                DateTime.Now.AddDays(-30) :
                request.FromDate;

        var searchResults = await _repository.SearchAsync(
            request.UserId,
            request.AccountId,
            request.CategoryId,
            request.PayerPayeeId,
            fromDate,
            toDate,
            pageNo,
            perPage,
            request.TimeZone,
            cancellationToken);

        if (searchResults.IsFailed)
            return Result.Fail(searchResults.Errors);

        var searchResultValue = searchResults.Value;

        var searchResponse = new TransactionsSearchResponse();

        searchResponse.UserId = request.UserId;

        searchResponse.ForAccount = !string.IsNullOrWhiteSpace(request.AccountId)
            ? new KeyValuePair<string, string>(request.AccountId, string.Empty)
            : default;

        searchResponse.ForCategory = !string.IsNullOrWhiteSpace(request.CategoryId)
            ? new KeyValuePair<string, string>(request.CategoryId, string.Empty)
            : default;

        searchResponse.ForPayerPayee = !string.IsNullOrWhiteSpace(request.PayerPayeeId)
            ? new KeyValuePair<string, string>(request.PayerPayeeId, string.Empty)
            : default;

        searchResponse.FromDate = fromDate;
        searchResponse.ToDate = toDate;

        searchResponse.TimeZome = request.TimeZone;

        searchResponse.PageNo = searchResultValue.PageNo;
        searchResponse.PerPage = searchResultValue.PageSize;

        var transactionDtos = ApplicationMapper.MapLineItems(searchResultValue.Results);

        searchResponse.Transactions = transactionDtos;

        return Result.Ok(searchResponse);
    }

    public class Validator : AbstractValidator<SearchTransactionsQuery>
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