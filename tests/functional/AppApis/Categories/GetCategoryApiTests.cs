using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class GetCategoryApiTests(WebApplicationFactory<Apis.App.AppApis.Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Apis.App.AppApis.Program>>
{
    private const string ENDPOINTS_GET_CATEGORY = "/api/v1/users/{userId}/categories/{categoryId}";

    [Fact]
    public async Task CanCall_GetCategory_WithValidRequest_ReturnsOk()
    {
        var categoryDocs =
            (await CategoryDocumentsRepository.FindDocumentsAsync(c => c.UserId.Equals(USER_ID)))?
            .ToList() ??
            [];

        if (!categoryDocs.Any())
        {
            foreach (var categoryDoc in categoryDocs)
            {
                // Act
                var response = await HttpClient.GetAsync(ENDPOINTS_GET_CATEGORY
                        .Replace("{userId}", USER_ID)
                        .Replace("{categoryId}", categoryDoc.Id.ToString()));

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(content, JsonSerializationOptions);

                // Assert
                Assert.NotNull(apiResponse);
                Assert.True(apiResponse.IsSuccess);

                var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);

                Assert.NotNull(actualDto);
                Assert.NotEqual(categoryDoc.Id.ToString(), actualDto.Id);
                Assert.Equal(USER_ID, actualDto.UserId);
                Assert.NotEmpty(actualDto.Name);

                Assert.True(actualDto.ParentId is null || actualDto.ParentId.Equals(ObjectId.Empty.ToString()));
                Assert.True(actualDto.SortOrder > 0);
            }
        }
    }
}