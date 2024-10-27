using Habanerio.Xpnss.Modules.Categories.CQRS.Commands;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.DTOs;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MongoDB.Bson;
using Tests.Integration.Common;
using Xunit.Abstractions;

namespace Tests.Integration.Modules.Categories.CQRS.Commands;

[Collection(nameof(CategoriesMongoCollection))]
public class AddSubCategoriesHandlerTests : IClassFixture<CategoriesTestDbContextFixture>
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly ICategoriesRepository _categoriesRepository;

    private readonly List<(string UserId, CategoryDocument Category)> _actualCategories = [];

    private readonly AddSubCategories.Handler _testHandler;

    private readonly string _userId = "test-user-id";

    public AddSubCategoriesHandlerTests(CategoriesTestDbContextFixture dbContextFixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;

        _categoriesRepository = dbContextFixture.CategoriesRepository;

        _actualCategories = dbContextFixture.ActualCategories;

        _testHandler = new AddSubCategories.Handler(_categoriesRepository);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new AddSubCategories.Handler(_categoriesRepository);

        Assert.NotNull(handler);
    }

    [Fact]
    public void Cannot_Instantiate_Handler_WithNull_Repository_ThrowsException()
    {
        ICategoriesRepository repository = null;

        var error = Assert.Throws<ArgumentNullException>(() =>
            new AddSubCategories.Handler(repository));

        Assert.Equal("Value cannot be null. (Parameter 'repository')", error.Message);
    }

    [Fact]
    public async Task CanCall_Handle_CreateCategory()
    {
        // Arrange
        var expectedSubCategoryDto = new CategoryDto()
        {
            UserId = _userId,
            Name = $"Test Sub Category 1 {ObjectId.GenerateNewId().ToString()}",
            Description = "Test Sub Category 1 Description",
            SortOrder = 2
        };

        // Act
        var parentCategory = _actualCategories[0];
        var actualCategory = new AddSubCategories.Command(
            _userId,
            parentCategory.Category.Id.ToString(),
            new List<CategoryDto> { expectedSubCategoryDto });

        var result = await _testHandler.Handle(actualCategory, default);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var parentCategoryDto = Assert.IsType<CategoryDto>(result.Value);
        Assert.NotNull(parentCategoryDto);
        Assert.NotEmpty(parentCategoryDto.SubCategories);

        var subCategoryDto = parentCategoryDto.SubCategories
            .First(c => c.Name.Equals(expectedSubCategoryDto.Name));

        Assert.NotNull(subCategoryDto);
        Assert.NotEqual(ObjectId.Empty.ToString(), subCategoryDto.Id);
        Assert.Equal(_userId, subCategoryDto.UserId);
        Assert.Equal(expectedSubCategoryDto.Name, subCategoryDto.Name);
        Assert.Equal(expectedSubCategoryDto.Description, subCategoryDto.Description);
        //Assert.Equal(expectedSubCategoryDto.SortOrder, subCategoryDto.SortOrder);
    }

    [Fact]
    public async Task CannotCall_Handle_AddSubCategories_WithNullCommand_ThrowsException()
    {
        AddSubCategories.Command command = null;

        var error = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _testHandler.Handle(command, default));

        Assert.Equal("Cannot pass null model to Validate. (Parameter 'instanceToValidate')", error.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CannotCall_Handle_AddSubCategories_WithInvalidUserId_ReturnsError(string value)
    {
        var expectedCategory = new AddSubCategories.Command(value, ObjectId.GenerateNewId().ToString(), new List<CategoryDto>());

        var reult = await _testHandler.Handle(expectedCategory, default);

        Assert.False(reult.IsSuccess);
        Assert.Equal("'User Id' must not be empty.", reult.Errors[0].Message);
    }
}