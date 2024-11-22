using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class GetCategoriesApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_GET_CATEGORIES = "/api/v1/users/{userId}/categories";

    [Fact]
    public async Task CanCall_GetCategories_WithValidRequest_ReturnsOk()
    {
        // Act
        var response = await HttpClient.GetAsync(ENDPOINTS_GET_CATEGORIES
                .Replace("{userId}", USER_ID));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<CategoryDto>>>(content, JsonSerializationOptions);

        // Assert
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.IsSuccess);

        var actualDtos = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(apiResponse.Data);

        var actualListDtos = actualDtos.ToList();

        Assert.NotNull(actualListDtos);
        Assert.NotEmpty(actualListDtos);

        var lastSortOrder = 0;

        foreach (var actualListDto in actualListDtos)
        {
            Assert.NotNull(actualListDto);
            Assert.NotEqual(ObjectId.Empty.ToString(), actualListDto.Id);
            Assert.Equal(USER_ID, actualListDto.UserId);
            Assert.NotEmpty(actualListDto.Name);

            Assert.True(actualListDto.ParentId is null || actualListDto.ParentId.Equals(ObjectId.Empty.ToString()));

            Assert.True(actualListDto.SortOrder > lastSortOrder);
            lastSortOrder = actualListDto.SortOrder;
        }
    }
}