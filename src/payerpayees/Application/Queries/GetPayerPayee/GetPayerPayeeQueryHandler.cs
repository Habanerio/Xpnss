using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayee;

public class GetPayerPayeeQueryHandler(IPayerPayeesRepository repository) :
    IRequestHandler<GetPayerPayeeQuery, Result<PayerPayeeDto?>>
{
    private readonly IPayerPayeesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<PayerPayeeDto?>> Handle(GetPayerPayeeQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail<PayerPayeeDto?>(validationResult.Errors[0].ErrorMessage);

        var result = await _repository.GetAsync(request.UserId, request.PayerPayeeId, cancellationToken);

        if (result.IsFailed)
            return Result.Fail<PayerPayeeDto?>(result.Errors);

        if (result.ValueOrDefault is null)
            return Result.Ok<PayerPayeeDto?>(default);

        var payerPayeeDto = ApplicationMapper.Map(result.Value);

        if (payerPayeeDto is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map PayerPayeeDocument to PayerPayeeDto");

        return Result.Ok<PayerPayeeDto?>(payerPayeeDto);
    }

    public class Validator : AbstractValidator<GetPayerPayeeQuery>
    {
        public Validator()
        {
            RuleFor(x => x.PayerPayeeId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}