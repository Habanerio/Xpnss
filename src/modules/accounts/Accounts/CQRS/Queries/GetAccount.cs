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
    public record Query(string Id, string UserId) : IAccountsQuery<Result<AccountDto>>;

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

            var dto = await _repository.GetByIdAsync(request.Id, request.UserId, cancellationToken);

            if (dto.IsFailed)
                return Result.Fail(dto.Errors[0].Message);

            return Result.Ok(dto.Value);
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
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}