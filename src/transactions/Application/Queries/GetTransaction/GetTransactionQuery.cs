using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Transactions.Application.Mappers;
using Habanerio.Xpnss.Transactions.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Transactions.Application.Queries.GetTransaction;

public sealed record GetTransactionQuery :
    ITransactionsQuery<Result<TransactionDto?>>
{
    public string UserId { get; set; } = string.Empty;

    public string TransactionId { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public GetTransactionQuery(string userId, string transactionId, string timeZone = "")
    {
        UserId = userId;
        TransactionId = transactionId;
        TimeZone = timeZone;
    }
}

public sealed class GetTransactionHandler(ITransactionsRepository repository) :
    IRequestHandler<GetTransactionQuery, Result<TransactionDto?>>
{
    private readonly ITransactionsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<TransactionDto?>> Handle(
        GetTransactionQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var docsResult = await _repository.GetAsync(
            request.UserId,
            request.TransactionId,
            cancellationToken);

        if (docsResult.IsFailed)
            return Result.Fail(docsResult.Errors);

        var dto = ApplicationMapper.Map(docsResult.Value);

        if (dto is null)
            throw new InvalidCastException("Failed to map Transaction to TransactionDto");

        return Result.Ok(dto);
    }

    public class Validator : AbstractValidator<GetTransactionQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.TransactionId).NotEmpty();
        }
    }
}
