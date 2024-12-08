using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.PayerPayees.Application.Mappers;
using Habanerio.Xpnss.PayerPayees.Domain.Interfaces;
using MediatR;
using MongoDB.Bson;

namespace Habanerio.Xpnss.PayerPayees.Application.Queries.GetPayerPayeesByIds;

public sealed record GetPayerPayeesByIdsQuery(string UserId, string[] PayerPayeesIds) :
    IPayerPayeesQuery<Result<IEnumerable<PayerPayeeDto>>>;

public class GetPayerPayeesByIdsQueryHandler(IPayerPayeesRepository repository) :
    IRequestHandler<GetPayerPayeesByIdsQuery, Result<IEnumerable<PayerPayeeDto>>>
{
    private readonly IPayerPayeesRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<PayerPayeeDto>>> Handle(
        GetPayerPayeesByIdsQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var payerPayeeResult = await _repository
            .GetAsync(
                request.UserId,
                request.PayerPayeesIds
                    .Select(ObjectId.Parse),
                cancellationToken);

        if (payerPayeeResult.IsFailed)
            return Result.Fail(payerPayeeResult.Errors);

        if (payerPayeeResult.ValueOrDefault is null)
            return Result.Ok(Enumerable.Empty<PayerPayeeDto>());

        var dtos = ApplicationMapper.Map(payerPayeeResult.Value);

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetPayerPayeesByIdsQuery>
    {
        public Validator()
        {
            RuleFor(x => x.PayerPayeesIds).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}