using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayees;

public sealed record GetPayerPayeesQuery(string UserId) :
    IPayerPayeesQuery<Result<IEnumerable<PayerPayeeDto>>>;

public class GetPayerPayeesQueryHandler(IPayerPayeesRepository repository) :
    IRequestHandler<GetPayerPayeesQuery, Result<IEnumerable<PayerPayeeDto>>>
{
    private readonly IPayerPayeesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<PayerPayeeDto>>> Handle(
        GetPayerPayeesQuery query,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var payerPayeeResult = await _repository
            .ListAsync(query.UserId, cancellationToken);

        if (payerPayeeResult.IsFailed)
            return Result.Fail(payerPayeeResult.Errors);

        if (payerPayeeResult.ValueOrDefault is null)
            return Result.Ok(Enumerable.Empty<PayerPayeeDto>());

        var dtos = ApplicationMapper.Map(payerPayeeResult.Value);

        if (dtos is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map PayerPayee to PayerPayeeDto");

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetPayerPayeesQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}