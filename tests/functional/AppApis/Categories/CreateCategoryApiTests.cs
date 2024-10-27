using System.Net.Http.Json;
using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Endpoints.Categories;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Modules.Categories.DTOs;
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
        // Arrange
        var request = new CreateCategoryEndpoint.CreateCategoryRequest
        {
            UserId = USER_ID,
            Name = "Home",
            Description = "Home Category Description",
            SortOrder = 9
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            ENDPOINTS_CREATE_CATEGORY
                .Replace("{userId}", USER_ID),
            request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);
        Assert.NotNull(actualDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), actualDto.Id);
        Assert.Equal(USER_ID, request.UserId);
        Assert.Equal(request.Name, actualDto.Name);
        Assert.Equal(request.Description, actualDto.Description);
        Assert.Equal(request.SortOrder, actualDto.SortOrder);
    }
}