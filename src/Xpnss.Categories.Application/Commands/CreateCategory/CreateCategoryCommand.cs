using FluentResults;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Domain.Interfaces;

namespace Habanerio.Xpnss.Categories.Application.Commands.CreateCategory;

public record CreateCategoryCommand(
        string UserId,
        string Name,
        string? ParentId = null,
        string Description = "",
        int SortOrder = 99) : ICategoriesCommand<Result<CategoryDto>>;