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
    [Fact]
    public async Task CanCall_GetCategories_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        // Act
        var response = await HttpClient.GetAsync(
        ENDPOINTS_CATEGORIES_GET_CATEGORIES
                .Replace("{userId}", userId.ToString()));

        //response.EnsureSuccessStatusCode();
        //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<CategoryDto>>>(
            content,
            JsonSerializationOptions);

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
            Assert.Equal(userId.ToString(), actualListDto.UserId);
            Assert.NotEmpty(actualListDto.Name);

            Assert.True(actualListDto.SortOrder > lastSortOrder);
            lastSortOrder = actualListDto.SortOrder;
        }
    }
}