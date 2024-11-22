using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Habanerio.Xpnss.Categories.Application.Commands.CreateCategory;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class CreateCategoryApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_CREATE_CATEGORY = "/api/v1/users/{userId}/categories";

    [Fact]
    public async Task CanCall_CreateCategory_WithValidRequest_ReturnsOk()
    {
        var uniqueness = Guid.NewGuid();
        var newCategoryName = $"NewId Category {uniqueness.ToString()}";
        var newCategoryDescription = $"{newCategoryName} Description";

        // Arrange
        var request = new CreateCategoryCommand(
            USER_ID,
            newCategoryName,
            null,
            newCategoryDescription,
            9);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_CATEGORY
                .Replace("{userId}", USER_ID),
            request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);
        Assert.NotNull(actualDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
        Assert.Equal(USER_ID, request.UserId);
        Assert.Contains(actualDto.Name, request.Name);
        Assert.Equal(request.Description, actualDto.Description);
        // Would rather have it null
        Assert.True(actualDto.ParentId is null || actualDto.ParentId.Equals(ObjectId.Empty.ToString()));
        Assert.Empty(actualDto.SubCategories);

        // Can't guarantee the SortOrder will be the same,
        // as it is recalculated when the category is created so that no two categories have the same SortOrder,
        // and to make sure that they are all sequential.
        //Assert.Equal(request.SortOrder, actualDto.SortOrder);
    }

    [Fact]
    public async Task CanCall_CreateSubCategory_WithValidRequest_ReturnsOk()
    {
        var categoryDocs = await CategoryDocumentsRepository.FindDocumentsAsync(c => c.UserId == USER_ID);

        var firstCategoryDoc = categoryDocs.First();

        var uniqueness = Guid.NewGuid();
        var newCategoryName = $"{firstCategoryDoc.Name} {uniqueness.ToString()}";
        var newCategoryDescription = $"{newCategoryName} Description";

        // Arrange
        var request = new CreateCategoryCommand(
            USER_ID,
            newCategoryName,
            firstCategoryDoc.Id.ToString(),
            newCategoryDescription,
            9);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_CATEGORY
                .Replace("{userId}", USER_ID),
            request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);
        Assert.NotNull(actualDto);

        Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
        Assert.Equal(USER_ID, request.UserId);
        Assert.Contains(actualDto.Name, request.Name);
        Assert.Equal(request.Description, actualDto.Description);
        Assert.NotNull(actualDto.ParentId);
        Assert.Equal(request.ParentId, actualDto.ParentId);
        //Assert.Equal(request.SortOrder, actualDto.SortOrder);
    }
}