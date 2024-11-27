using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Mappers;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Queries.GetAccounts;

public sealed record GetAccountsQuery(string UserId) : IAccountsQuery<Result<IEnumerable<AccountDto>>>;

/// <summary>
/// MediatR query to get accounts by user id.
/// </summary>
public class GetAccountsQueryHandler(IAccountsRepository repository) :
    IRequestHandler<GetAccountsQuery, Result<IEnumerable<AccountDto>>>
{
    private readonly IAccountsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<AccountDto>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        try
        {
            var docsResult = await _repository.ListAsync(request.UserId, cancellationToken);

            if (docsResult.IsFailed)
                return Result.Fail(docsResult.Errors);

            if (docsResult.ValueOrDefault is null || !docsResult.Value.Any())
                return Result.Ok(Enumerable.Empty<AccountDto>());

            var dtos = ApplicationMapper.Map(docsResult.Value);

            return Result.Ok(dtos);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    internal sealed class Validator : AbstractValidator<GetAccountsQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}