using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Mappers;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Queries.GetAccount;

public sealed record GetAccountQuery(string UserId, string AccountId) :
    IAccountsQuery<Result<AccountDto>>;

public class GetAccountQueryHandler(IAccountsRepository repository) :
    IRequestHandler<GetAccountQuery, Result<AccountDto>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<AccountDto>> Handle(GetAccountQuery query, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var docResult = await _repository.GetAsync(query.UserId, query.AccountId, cancellationToken);

        if (docResult.IsFailed)
            return Result.Fail(docResult.Errors);

        var dto = ApplicationMapper.Map(docResult.Value);

        if (dto is null)
            return Result.Fail("Failed to map AccountDocument to AccountDto");

        return Result.Ok(dto);
    }

    internal class Validator : AbstractValidator<GetAccountQuery>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}