using FluentResults;
using Habanerio.Xpnss.Application.Categories.DTOs;
using Habanerio.Xpnss.Domain.Categories.Interfaces;

namespace Habanerio.Xpnss.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
        string UserId,
        string Name,
        string? ParentId = null,
        string Description = "",
        int SortOrder = 99) : ICategoriesCommand<Result<CategoryDto>>;