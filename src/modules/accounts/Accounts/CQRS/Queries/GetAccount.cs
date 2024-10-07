using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Queries;

/// <summary>
/// MediatR query to get an account by its id and user id.
/// </summary>
public class GetAccount
{
    public record Query(string UserId, string AccountId) : IAccountsQuery<Result<AccountDto>>;

    public class Handler(IAccountsRepository repository) : IRequestHandler<Query, Result<AccountDto>>
    {
        private readonly IAccountsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

        public async Task<Result<AccountDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var docResult = await _repository.GetByIdAsync(request.UserId, request.AccountId, cancellationToken);

            if (docResult.IsFailed)
                return Result.Fail(docResult.Errors);

            var dto = Mappers.DocumentToDtoMappings.Map(docResult.Value);

            if (dto is null)
                return Result.Fail("Failed to map AccountDocument to AccountDto");

            return Result.Ok(dto);
        }
    }

    public Task<Result<AccountDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}