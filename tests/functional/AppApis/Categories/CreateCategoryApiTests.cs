using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class CreateCategoryApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    [Fact]
    public async Task CanCall_CreateCategory_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        var uniqueness = Guid.NewGuid();
        var newCategoryName = $"New Category {uniqueness.ToString()}";
        var newCategoryDescription = $"{newCategoryName} Description";

        // Arrange
        var request = new CreateCategoryCommand(
            userId.ToString(),
            newCategoryName,
            newCategoryDescription);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CATEGORIES_CREATE_CATEGORY
                .Replace("{userId}", userId.ToString()),
            request);

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<CategoryDto>(
            content,
            JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);

        var actualDto = Assert.IsType<CategoryDto>(apiResponse);
        Assert.NotNull(actualDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
        Assert.Equal(userId.ToString(), request.UserId);
        Assert.Contains(actualDto.Name, request.Name);
        Assert.Equal(request.Description, actualDto.Description);
        Assert.Empty(actualDto.SubCategories);

        // Can't guarantee the SortOrder will be the same,
        // as it is recalculated when the category is created so that
        // no two categories have the same SortOrder,
        // and to make sure that they are all sequential.
        //Assert.Equal(request.SortOrder, actualDto.SortOrder);
    }

    [Fact]
    public async Task CanCall_CreateSubCategory_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        var categoryDocs =
            (await CategoryDocumentsRepository
                .FindDocumentsAsync(c =>
                    c.UserId == userId))?.ToList() ?? [];

        if (categoryDocs.Count == 0)
            Assert.Fail("Need to add Categories before running the test");

        var firstCategoryDoc = categoryDocs[0];

        var uniqueness = Guid.NewGuid();
        var newCategoryName = $"{firstCategoryDoc.Name} {uniqueness.ToString()}";
        var newCategoryDescription = $"{newCategoryName} Description";

        // Arrange
        var request = new CreateCategoryCommand(
            userId.ToString(),
            newCategoryName,
            newCategoryDescription);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CATEGORIES_CREATE_CATEGORY
                .Replace("{userId}", userId.ToString()),
            request);

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<CategoryDto>(
            content,
            JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);

        var actualDto = Assert.IsType<CategoryDto>(apiResponse);
        Assert.NotNull(actualDto);

        Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
        Assert.Equal(userId.ToString(), request.UserId);
        Assert.Contains(actualDto.Name, request.Name);
        Assert.Equal(request.Description, actualDto.Description);
    }
}