using Habanerio.Xpnss.Modules.Categories.CQRS.Queries;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Categories.CQRS.Queries;

[Collection(nameof(CategoriesMongoCollection))]
public class GetCategoryHandlerTests : IClassFixture<CategoriesTestDbContextFixture>
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly ICategoriesRepository _categoriesRepository;

    private readonly GetCategory.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    private readonly List<(string UserId, CategoryDocument Category)> _actualCategories;

    public GetCategoryHandlerTests(CategoriesTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _categoriesRepository = dbContextFixture.CategoriesRepository;

        _testHandler = new GetCategory.Handler(_categoriesRepository);

        _actualCategories = dbContextFixture.ActualCategories;
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new GetCategory.Handler(_categoriesRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ICategoriesRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new GetCategory.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_Handle_GetCategoryByUserId()
    {
        foreach (var _actualCategory in _actualCategories)
        {
            var userId = _actualCategory.UserId;
            var expectedCategory = _actualCategory.Category;

            var query = new GetCategory.Query(userId, expectedCategory.Id.ToString());

            var result = await _testHandler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.ValueOrDefault);

            var actualCategory = result.ValueOrDefault;
            Assert.NotNull(actualCategory);
            Assert.Equal(expectedCategory.Id.ToString(), actualCategory.Id);
            Assert.Equal(userId, actualCategory.UserId);
            Assert.Equal(expectedCategory.Name, actualCategory.Name);
            Assert.Equal(expectedCategory.Description, actualCategory.Description);
            Assert.Equal(expectedCategory.SortOrder, actualCategory.SortOrder);
        }
    }
}