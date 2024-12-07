using System.Text.Json.Serialization;
using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Accounts.Application.Mappers;
using Habanerio.Xpnss.Accounts.Domain.Interfaces;
using Habanerio.Xpnss.Application.DTOs;
using MediatR;

namespace Habanerio.Xpnss.Accounts.Application.Commands.UpdateAccountDetails;

public sealed record UpdateAccountDetailsCommand : IAccountsCommand<Result<AccountDto>>
{
    public string UserId { get; init; } = "";
    public string AccountId { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string DisplayColor { get; init; } = "";

    [JsonConstructor]
    public UpdateAccountDetailsCommand() { }

    public UpdateAccountDetailsCommand(string userId, string accountId, string name, string description, string displayColor)
    {
        UserId = userId;
        AccountId = accountId;
        Name = name;
        Description = description;
        DisplayColor = displayColor;
    }
}

/// <summary>
/// Only deals with an Account's details, such as name, description, and display color.
/// Balance, Credit Limit, Interest Rate, and Overdraft TotalAmount are handled separately.
/// </summary>
public sealed class UpdateAccountDetails : IRequestHandler<UpdateAccountDetailsCommand, Result<AccountDto>>
{
    private readonly IAccountsRepository _repository;

    public UpdateAccountDetails(IAccountsRepository repository)
    {
        _repository = repository ??
                      throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<AccountDto>> Handle(UpdateAccountDetailsCommand request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var accountResult = await _repository.GetAsync(request.UserId, request.AccountId, cancellationToken);

        if (accountResult.IsFailed)
            return Result.Fail(accountResult.Errors);

        if (accountResult.Value is null)
            return Result.Fail("Could not find the Account");

        var account = accountResult.Value;
        account.UpdateDetails(request.Name, request.Description, request.DisplayColor);

        var result = await _repository.UpdateAsync(account, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var dto = ApplicationMapper.Map(account);

        if (dto is null)
            throw new InvalidCastException("Failed to map AccountDocument to AccountDto");

        return Result.Ok(dto);
    }

    internal sealed class Validator : AbstractValidator<UpdateAccountDetailsCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.AccountId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}