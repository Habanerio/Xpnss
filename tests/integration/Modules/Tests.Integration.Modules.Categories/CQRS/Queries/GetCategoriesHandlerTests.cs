using Habanerio.Xpnss.Modules.Categories.CQRS.Queries;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Categories.CQRS.Queries;

[Collection(nameof(CategoriesMongoCollection))]
public class GetCategoriesHandlerTests : IClassFixture<CategoriesTestDbContextFixture>
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly ICategoriesRepository _categoriesRepository;

    private readonly GetCategories.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    private readonly List<(string UserId, CategoryDocument Category)> _actualCategories;

    public GetCategoriesHandlerTests(CategoriesTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _categoriesRepository = dbContextFixture.CategoriesRepository;

        _testHandler = new GetCategories.Handler(_categoriesRepository);

        _actualCategories = dbContextFixture.ActualCategories;
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new GetCategories.Handler(_categoriesRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ICategoriesRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new GetCategories.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_Handle_GetCategoriesByUserId()
    {
        Dictionary<string, int> expectedCategories = _actualCategories.Where(x => x.UserId == _userId)
            .GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => x.Count());

        var query = new GetCategories.Query(_userId);

        var result = await _testHandler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.ValueOrDefault);

        var actualCategories = result.ValueOrDefault.ToList();
        Assert.NotEmpty(actualCategories);
        Assert.Equal(expectedCategories.Sum(k => k.Value), actualCategories.Count);

        foreach (var expectedCategory in expectedCategories)
        {
            var actualCategory = actualCategories.Find(x => x.UserId == expectedCategory.Key);
            Assert.NotNull(actualCategory);
        }
    }
}