using FluentResults;
using FluentValidation;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Domain.Interfaces;
using MediatR;

namespace Habanerio.Xpnss.Categories.Application.Queries.GetCategory;

public sealed record GetCategoryQuery(string UserId, string CategoryId) :
    ICategoriesQuery<Result<CategoryDto?>>;


public class GetCategoryHandler(ICategoriesRepository repository) :
    IRequestHandler<GetCategoryQuery, Result<CategoryDto?>>
{
    private readonly ICategoriesRepository _repository = repository ??
          throw new ArgumentNullException(nameof(repository));

    public async Task<Result<CategoryDto?>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var validator = new Validator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors[0].ErrorMessage);

        var categoryResult = await _repository.GetAsync(request.UserId, request.CategoryId, cancellationToken);

        if (categoryResult.IsFailed)
            return Result.Fail(categoryResult.Errors);

        var categoryDocument = categoryResult.ValueOrDefault;

        if (categoryDocument is null)
            return Result.Ok<CategoryDto?>(null);

        var categoryDto = Mappers.ApplicationMapper.Map(categoryDocument);

        if (categoryDto is null)
            return Result.Fail("Failed to map CategoryDocument to CategoryDto");

        return Result.Ok<CategoryDto?>(categoryDto);
    }
}

public class Validator : AbstractValidator<GetCategoryQuery>
{
    public Validator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
