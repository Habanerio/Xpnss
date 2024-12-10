using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.Accounts.DTOs;
using Habanerio.Xpnss.Application.Mappers;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Queries.GetAccounts;

/// <summary>
/// MediatR query to get accounts by user id.
/// </summary>
public class GetAccountsHandler(IAccountsRepository repository) : IRequestHandler<GetAccountsQuery, Result<IEnumerable<AccountDto>>>
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

            if (!docsResult.Value.Any())
                return Result.Ok(Enumerable.Empty<AccountDto>());

            var dtos = Mapper.Map(docsResult.Value);

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