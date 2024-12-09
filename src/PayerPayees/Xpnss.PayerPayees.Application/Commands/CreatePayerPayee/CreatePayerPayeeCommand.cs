using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain.Entities;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;

public sealed record CreatePayerPayeeCommand(
    string UserId,
    string Name) :
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

        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail("Id or Name must be provided");

        PayerPayee? payerPayee;

        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            var getResult =
                await _repository.GetByNameAsync(command.UserId, command.Name, cancellationToken);

            payerPayee = getResult.ValueOrDefault;

            if (getResult is { IsSuccess: true, ValueOrDefault: not null })
            {
                return Result.Ok(ApplicationMapper.Map(payerPayee));
            }
        }

        payerPayee = PayerPayee.New(
            new UserId(command.UserId),
            new PayerPayeeName(command.Name));

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