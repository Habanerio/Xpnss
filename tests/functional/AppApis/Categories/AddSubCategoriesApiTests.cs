using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class AddSubCategoriesApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact(Skip = "")]
    public async Task CanCall_AddSubCategories_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        var categoryDocs =
            (await CategoryDocumentsRepository
                .FindDocumentsAsync(c =>
                    c.UserId == userId))?.ToList() ?? [];

        if (categoryDocs.Count == 0)
            Assert.Fail("Need to add Categories before running the test");

        var addSubCategoryCommands = new List<AddSubCategoriesCommand>();

        foreach (var categoryDoc in categoryDocs)
        {
            addSubCategoryCommands.Add(
                new AddSubCategoriesCommand(
                    userId.ToString(),
                    categoryDoc.Id.ToString(),
                    new List<SubCategoryDto>
                    {
                        new SubCategoryDto
                        {
                            Name = $"{categoryDoc.Name} Test Sub Category",
                            Description = $"{categoryDoc.Name} Test Sub Category Description",
                            SortOrder = 1
                        }
                    }));
        }


        foreach (var command in addSubCategoryCommands)
        {
            var response = await HttpClient.PostAsJsonAsync(
                ENDPOINTS_CATEGORIES_ADD_SUBCATEGORIES
                    .Replace("{userId}", userId.ToString()
                    .Replace("{categoryId}", command.ParentCategoryId)),
                command);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                var errors = JsonSerializer.Deserialize<List<string>>(errorContent, JsonSerializationOptions);

                Assert.NotNull(errors);

                Assert.Fail(string.Join(Environment.NewLine, errors));
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(
                content,
                JsonSerializationOptions);

            // Assert
            Assert.NotNull(apiResponse);
            Assert.True(apiResponse.IsSuccess);

            var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);
            Assert.NotNull(actualDto);

            Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
            Assert.Equal(userId.ToString(), command.UserId);
            Assert.Equal(command.ParentCategoryId, actualDto.Id);

            foreach (var subCategoryCommand in command.SubCategories)
            {
                var actualSubCategoryDto = actualDto
                    .SubCategories
                    .Find(s => s.Name.Equals(subCategoryCommand.Name));

                Assert.NotNull(actualSubCategoryDto);
                Assert.Equal(subCategoryCommand.Description, actualSubCategoryDto.Description);
            }
        }
    }
}