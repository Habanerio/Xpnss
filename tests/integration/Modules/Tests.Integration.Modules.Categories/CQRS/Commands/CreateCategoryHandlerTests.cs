using Habanerio.Xpnss.Modules.Categories.CQRS.Commands;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Categories.CQRS.Commands;

[Collection(nameof(CategoriesMongoCollection))]
public class CreateCategoryHandlerTests : IClassFixture<CategoriesTestDbContextFixture>
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly ICategoriesRepository _categoriesRepository;

    private readonly TestCategoriesRepository _verifyRepository;

    private readonly CreateCategory.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    public CreateCategoryHandlerTests(CategoriesTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _categoriesRepository = dbContextFixture.CategoriesRepository;

        _verifyRepository = dbContextFixture.VerifyCategoriesRepository;

        _testHandler = new CreateCategory.Handler(_categoriesRepository);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new CreateCategory.Handler(_categoriesRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ICategoriesRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new CreateCategory.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_Handle_CreateCategory()
    {
        // Arrange
        var expectedCategory = new CreateCategory.Command(_userId, "Test Category 1", "Test Category 1 Description", 54);

        // Act
        var result = await _testHandler.Handle(expectedCategory, default);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var categoryDto = Assert.IsType<CategoryDto>(result.Value);
        Assert.NotEqual(ObjectId.Empty.ToString(), categoryDto.Id);
        Assert.Equal(_userId, categoryDto.UserId);
        Assert.Equal(expectedCategory.Name, categoryDto.Name);
        Assert.Equal(expectedCategory.Description, categoryDto.Description);
        Assert.Equal(expectedCategory.SortOrder, categoryDto.SortOrder);
    }

    [Fact]
    public async Task CannotCall_Handle_CreateCategory_WithNullCommand_ThrowsException()
    {
        CreateCategory.Command command = null;

        var error = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _testHandler.Handle(command, default));

        Assert.Equal("Cannot pass null model to Validate. (Parameter 'instanceToValidate')", error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_CreateCategory_WithInvalidUserId_ReturnsError(string value)
    {
        var expectedCategory = new CreateCategory.Command(value, "Test Category 1", "Test Category 1 Description", 54);

        var reult = await _testHandler.Handle(expectedCategory, default);

        Assert.False(reult.IsSuccess);
        Assert.Equal("'User Id' must not be empty.", reult.Errors[0].Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_CreateCategory_WithInvalidName_ReturnsError(string value)
    {
        var expectedCategory = new CreateCategory.Command(_userId, value, "Test Category 1 Description", 54);

        var reult = await _testHandler.Handle(expectedCategory, default);

        Assert.False(reult.IsSuccess);
        Assert.Equal("'Name' must not be empty.", reult.Errors[0].Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task CannotCall_Handle_CreateCategory_WithSortOrder_ReturnsError(int value)
    {
        var expectedCategory = new CreateCategory.Command(_userId, "Test Category 1", "Test Category 1 Description", value);

        var reult = await _testHandler.Handle(expectedCategory, default);

        Assert.False(reult.IsSuccess);
        Assert.Equal("'Sort Order' must be greater than '0'.", reult.Errors[0].Message);
    }
}