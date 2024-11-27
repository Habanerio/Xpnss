using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Domain.ValueObjects;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Application.Commands.CreatePayerPayee;

public class CreatePayerPayeeHandler(IPayerPayeesRepository repository) :
    IRequestHandler<CreatePayerPayeeCommand, Result<PayerPayeeDto>>
{
    private readonly IPayerPayeesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<PayerPayeeDto>> Handle(CreatePayerPayeeCommand command, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var payerPayee = PayerPayee.New(new UserId(command.UserId), new PayerPayeeName(command.Name), command.Description, command.Location);

        var result = await _repository.AddAsync(payerPayee, cancellationToken);

        if (result.IsFailed || result.ValueOrDefault is null)
            return Result.Fail(result.Errors[0].Message ?? "Could not save the PayerPayee");

        var payerPayeeDto = ApplicationMapper.Map(payerPayee);

        if (payerPayeeDto is null)
            return Result.Fail("Failed to map PayerPayeeDocument to PayerPayeeDto");

        return Result.Ok(payerPayeeDto);
    }

    public class Validator : AbstractValidator<CreatePayerPayeeCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}