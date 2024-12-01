using System.Text.Json;
using Habanerio.Xpnss.Apis.App.AppApis;
using Habanerio.Xpnss.Apis.App.AppApis.Models;
using Habanerio.Xpnss.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Habanerio.Xpnss.Tests.Functional.AppApis.Categories;

public class GetCategoryApiTests(WebApplicationFactory<Program> factory) :
    BaseFunctionalApisTests(factory),
    IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CanCall_GetCategory_WithValidRequest_ReturnsOk()
    {
        var userId = await GetTestUserObjectIdAsync();

        var categoryDocs =
            (await CategoryDocumentsRepository.FindDocumentsAsync(c =>
                c.UserId.Equals(userId)))?
            .ToList() ??
            [];

        if (categoryDocs.Any())
        {
            foreach (var categoryDoc in categoryDocs)
            {
                // Act
                var response = await HttpClient.GetAsync(
                    ENDPOINTS_CATEGORIES_GET_CATEGORY
                        .Replace("{userId}", userId.ToString())
                        .Replace("{categoryId}", categoryDoc.Id.ToString()));

                //response.EnsureSuccessStatusCode();
                //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(response.IsSuccessStatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(
                    content,
                    JsonSerializationOptions);

                // Assert
                Assert.NotNull(apiResponse);
                Assert.True(apiResponse.IsSuccess);

                var actualDto = Assert.IsType<CategoryDto>(apiResponse.Data);

                Assert.NotNull(actualDto);
                Assert.Equal(categoryDoc.Id.ToString(), actualDto.Id);
                Assert.Equal(userId.ToString(), actualDto.UserId);
                Assert.NotEmpty(actualDto.Name);
                Assert.True(actualDto.SortOrder > 0);
            }
        }
    }
}