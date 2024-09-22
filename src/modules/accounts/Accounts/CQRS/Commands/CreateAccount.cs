using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Modules.Accounts.Common;
using Habanerio.Xpnss.Modules.Accounts.Data;
using Habanerio.Xpnss.Modules.Accounts.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Modules.Accounts.CQRS.Commands;

/// <summary>
/// MediatR Command to Create a new Account
/// </summary>
/// <remarks>
/// Needs to be cleaned up
/// </remarks>
public class CreateAccount
{
    public record Command(
        string UserId,
        AccountType AccountType,
        string Name,
        string Description,
        decimal Balance,
        decimal CreditLimit = 0,
        decimal InterestRate = 0,
        decimal OverDraftAmount = 0,
        string DisplayColor = "") : IAccountsCommand<Result<string>>, IRequest;

    public class Handler(IAccountsRepository repository) : IRequestHandler<Command, Result<string>>
    {
        private readonly IAccountsRepository _repository = repository ??
                                                           throw new ArgumentNullException(nameof(repository));

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validator = new Validator();

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return Result.Fail(validationResult.Errors[0].ErrorMessage);

            var newDoc = AccountDocument.New(
                request.UserId,
                request.Name,
                request.AccountType,
                request.Description,
                request.Balance,
                request.DisplayColor);

            var extendedProps = new List<KeyValuePair<string, object?>>();

            foreach (var prop in request.GetType().GetProperties())
            {
                if (string.IsNullOrWhiteSpace(prop.Name) ||
                    prop.Name == nameof(request.UserId) ||
                    prop.Name == nameof(request.Name) ||
                    prop.Name == nameof(request.AccountType) ||
                    prop.Name == nameof(request.Balance) ||
                    prop.Name == nameof(request.Description) ||
                    prop.Name == nameof(request.DisplayColor)
                   )
                    continue;

                var value = prop.GetValue(request) ?? default;

                extendedProps.Add(new KeyValuePair<string, object?>(prop.Name, value));
            }

            newDoc.ExtendedProps = extendedProps;

            try
            {
                _repository.Add(newDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var result = await _repository.SaveAsync(cancellationToken);

            if (!result.IsSuccess)
                return Result.Fail(result.Errors?[0].Message ?? "Could not save the Account");

            return Result.Ok(newDoc.Id.ToString());
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountType).IsInEnum();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}