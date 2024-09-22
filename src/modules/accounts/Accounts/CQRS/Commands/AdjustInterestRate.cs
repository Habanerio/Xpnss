using System.Globalization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// This is for manual changes to the Interest Rate, which is logged.
/// 
/// This should have its own Api endpoint.
/// </summary>
public class AdjustInterestRate
{
    public record Command(
        string UserId,
        string AccountId,
        decimal InterestRate,
        string Reason = "") : IAccountsCommand<Result<decimal>>, IRequest;

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
                return Result.Fail("Account does not have a Interest Rate");

            var previousInterestRate = interestRateDto.InterestRate;

            interestRateDto.InterestRate = request.InterestRate;

            existingAccount = Mappers.DtoToDocumentMappings.Map(dto);

            if (existingAccount is null)
                return Result.Fail<decimal>("Failed to map AccountDto to Account");

            existingAccount.AddChangeHistory(
                existingAccount.UserId,
                nameof(existingAccount.Balance),
                previousInterestRate.ToString(CultureInfo.InvariantCulture),
                interestRateDto.InterestRate.ToString(CultureInfo.InvariantCulture),
                request.Reason);

            _repository.Update(existingAccount);

            var result = await _repository.SaveAsync(cancellationToken);

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