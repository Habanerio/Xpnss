using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.DTOs;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// Only deals with an Account's details, such as name, description, and display color.
/// Balance, Credit Limit, Interest Rate, and Overdraft Amount are handled separately.
/// </summary>
public class UpdateAccountDetails
{
    public record Command(
        string UserId,
        string AccountId,
        string Name,
        string Description,
        string DisplayColor) : IAccountsCommand<Result<AccountDto>>, IRequest;

    public class Handler : IRequestHandler<Command, Result<AccountDto>>
    {
        private readonly IAccountsRepository _repository;

        public Handler(IAccountsRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<AccountDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail<AccountDto>(validationResult.Errors[0].ErrorMessage);

            var result = await _repository.UpdateDetailsAsync(request.UserId,
                request.AccountId,
                request.Name,
                request.Description,
                request.DisplayColor, cancellationToken);

            if (result.IsFailed)
                return Result.Fail<AccountDto>(result.Errors);

            var dto = Mappers.DocumentToDtoMappings.Map(result.Value);

            if (dto is null)
                return Result.Fail<AccountDto>("Failed to map AccountDocument to AccountDto");

            return Result.Ok(dto);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}