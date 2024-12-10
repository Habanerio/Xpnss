using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Totals.Application.Mappers;
using Habanerio.Xpnss.Totals.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Totals.Application.Queries;

/// <summary>
/// Get the Year-To-Date totals for a given EntityType and EntityId.
/// </summary>
public sealed record GetYTDTotalsByEntityQuery(
    string UserId,
    EntityEnums.Keys EntityType,
    string EntityId) :
    IMonthlyTotalsQuery<Result<IEnumerable<MonthlyTotalDto>>>;

/// <summary>
/// Handler for the GetYTDTotalsByEntityQuery.
/// </summary>
public class GetYTDTotalsByEntityQueryHandler(IMonthlyTotalsRepository repository) :
    IRequestHandler<GetYTDTotalsByEntityQuery, Result<IEnumerable<MonthlyTotalDto>>>
{
    private readonly IMonthlyTotalsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<MonthlyTotalDto>>> Handle(
        GetYTDTotalsByEntityQuery query,
        CancellationToken cancellationToken)
    {
        var validator = new Validator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors?
                .Select(e => new Error(e.ErrorMessage)));

        var results =
            await _repository.ListAsync(
                query.UserId,
                query.EntityId,
                query.EntityType,
                DateTime.Now.Year,
                cancellationToken);

        if (results.IsFailed)
            return Result.Fail(results.Errors);

        if (results.ValueOrDefault is null || !results.ValueOrDefault.Any())
            return Result.Ok(Enumerable.Empty<MonthlyTotalDto>());

        var dtos = ApplicationMapper.Map(results.Value);

        if (dtos is null)
            throw new InvalidCastException(
                $"{nameof(GetType)}: Failed to map MonthlyTotal to MonthlyTotalDto");

        dtos = dtos.OrderByDescending(t => t.Year)
            .ThenBy(x => x.Month).ToList();

        return Result.Ok(dtos);
    }

    public class Validator : AbstractValidator<GetYTDTotalsByEntityQuery>
    {
        public Validator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.EntityId).NotEmpty();
        }
    }
}