using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for regular updates to the balance based on transactions.
/// 
/// If user makes manual changes, then use AdjustBalance instead.
/// 
/// This probably won't have its own api endpoint,
/// and instead would be called from the transaction endpoint,
/// or during the transaction process.
/// </summary>
public class UpdateInterestRate
{
    public record Command(
        string UserId,
        string AccountId,
        decimal InterestRate) : IAccountsCommand<Result<decimal>>, IRequest;

    public class Handler : IRequestHandler<Command, Result<decimal>>
    {
        private readonly IAccountsRepository _repository;

        public Handler(IAccountsRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<decimal>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail<decimal>(validationResult.Errors[0].ErrorMessage);

            var existingResult =
                await _repository.GetByIdAsync(request.UserId, request.AccountId, cancellationToken);

            if (existingResult.IsFailed)
                return Result.Fail<decimal>(existingResult.Errors);

            var existingAccount = existingResult.Value;

            var dto = Mappers.DocumentToDtoMappings.Map(existingAccount);

            if (dto is not IHasInterestRate interestRateDto)
                return Result.Fail($"The Account Type `{existingAccount.AccountTypes}` does not support Credit Limits");

            interestRateDto.InterestRate = request.InterestRate;

            existingAccount = Mappers.DtoToDocumentMappings.Map(dto);

            if (existingAccount is null)
                return Result.Fail<decimal>("Failed to map AccountDto to Account");

            var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

            if (result.IsFailed)
                return Result.Fail<decimal>(result.Errors);

            return Result.Ok(request.InterestRate);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.AccountId).NotEmpty();

            RuleFor(x => x.InterestRate)
                .InclusiveBetween(0, 100);
        }
    }
}