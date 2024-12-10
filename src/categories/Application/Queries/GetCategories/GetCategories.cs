using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Shared.DTOs;
using Habanerio.Xpnss.Categories.Application.Mappers;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Queries.GetCategories;

public sealed record GetCategoriesQuery(string UserId) : ICategoriesQuery<Result<IEnumerable<CategoryDto>>>;

public sealed class GetCategoriesHandler(ICategoriesRepository repository) : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    private readonly ICategoriesRepository _repository = repository ??
         throw new ArgumentNullException(nameof(repository));

    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        try
        {
            var docsResult = await _repository.ListAsync(request.UserId, cancellationToken);

            if (docsResult.IsFailed)
                return Result.Fail(docsResult.Errors);

            if (!docsResult.Value.Any())
                return Result.Ok(Enumerable.Empty<CategoryDto>());

            var dtos = ApplicationMapper.Map(docsResult.Value);

            return Result.Ok(dtos);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public class Validator : AbstractValidator<GetCategoriesQuery>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}