using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Domain.Accounts.Interfaces;
using Habanerio.Xpnss.Domain.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.Application.Accounts.Commands.AdjustBalance;

public sealed class AdjustBalanceHandler(IAccountsRepository repository)
    : IRequestHandler<AdjustBalanceCommand, Result<decimal>>
{
    private readonly IAccountsRepository _repository = repository ??
                                                       throw new ArgumentNullException(nameof(repository));

    public async Task<Result<decimal>> Handle(AdjustBalanceCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var existingResult =
            await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (existingResult.IsFailed)
            return Result.Fail(existingResult.Errors);

        var existingAccount = existingResult.Value;

        existingAccount.AdjustBalance(new Money(request.Balance), request.DateOfChange, request.Reason);

        var result = await _repository.UpdateAsync(existingAccount, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(request.Balance);
    }

    internal class Validator : AbstractValidator<AdjustBalanceCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
        }
    }
}