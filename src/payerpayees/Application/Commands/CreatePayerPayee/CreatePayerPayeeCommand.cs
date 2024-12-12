using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.ValueObjects;
using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;

public sealed record CreatePayerPayeeCommand(
    string UserId,
    string PayerPayeeId,
    string PayerPayeeName) :
    IPayerPayeesCommand<Result<PayerPayeeDto?>>;

/// <summary>
/// Will attempt to get the PayerPayee by name first.
/// If one exists for this user, then it will be returned.
/// </summary>
/// <param name="repository"></param>
public class CreatePayerPayeeCommandHandler(IPayerPayeesRepository repository) :
    IRequestHandler<CreatePayerPayeeCommand, Result<PayerPayeeDto?>>
{
    private readonly IPayerPayeesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<PayerPayeeDto?>> Handle(CreatePayerPayeeCommand command, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        if (string.IsNullOrWhiteSpace(command.PayerPayeeName))
            return Result.Fail("Id or Name must be provided");

        PayerPayee? payerPayee;

        // If there's an Id, I'm going to assume that it's the correct Id
        if (!string.IsNullOrWhiteSpace(command.PayerPayeeId))
        {
            var getResult =
                await _repository.GetAsync(command.UserId, command.PayerPayeeId, cancellationToken);

            payerPayee = getResult.ValueOrDefault;

            if (getResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                return Result.Ok(ApplicationMapper.Map(payerPayee));
            }
        }

        // Now if the Id fails, then let's see if the name exists ...
        if (!string.IsNullOrWhiteSpace(command.PayerPayeeName))
        {
            var getResult =
                await _repository.GetByNameAsync(command.UserId, command.PayerPayeeName, cancellationToken);

            payerPayee = getResult.ValueOrDefault;

            if (getResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                return Result.Ok(ApplicationMapper.Map(payerPayee));
            }
        }

        // So if a PayerPayee can't be found by its Name, and then by its Id, then create a new one.
        // Going to assume that it's the Id of an existing user Account (?).
        if (!string.IsNullOrWhiteSpace(command.PayerPayeeId) && !string.IsNullOrWhiteSpace(command.PayerPayeeName))
        {
            payerPayee = PayerPayee.Load(
                new PayerPayeeId(command.PayerPayeeId),
                new UserId(command.UserId),
                new PayerPayeeName(command.PayerPayeeName),
                string.Empty, string.Empty,
                DateTime.UtcNow, null, null);
        }
        else
        {
            payerPayee = PayerPayee.New(
                new UserId(command.UserId),
                new PayerPayeeName(command.PayerPayeeName));
        }

        if (payerPayee is null)
            //TODO: Should log it
            return Result.Ok<PayerPayeeDto?>(default);


        var newResult = await _repository.AddAsync(payerPayee, cancellationToken);

        if (newResult.IsFailed || newResult.ValueOrDefault is null)
            return Result.Fail(newResult.Errors[0].Message ?? "Could not save the PayerPayee");

        var payerPayeeDto = ApplicationMapper.Map(payerPayee);

        if (payerPayeeDto is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map PayerPayeeDocument to PayerPayeeDto");

        return Result.Ok<PayerPayeeDto?>(payerPayeeDto);
    }

    public class Validator : AbstractValidator<CreatePayerPayeeCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}