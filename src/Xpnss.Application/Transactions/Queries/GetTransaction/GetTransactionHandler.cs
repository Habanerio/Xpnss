using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Application.Transactions.DTOs;
using Habanerio.Xpnss.Domain.Transactions.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Transactions.Queries.GetTransaction;

public class GetTransactionHandler(ITransactionsRepository repository) : IRequestHandler<GetTransactionQuery, Result<TransactionDto?>>
{
    private readonly ITransactionsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

    public async Task<Result<TransactionDto?>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
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

        var dto = Mapper.Map(docsResult.Value);

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