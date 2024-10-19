using FluentResults;
using Habanerio.Xpnss.Modules.Categories.CQRS.Commands;
using Habanerio.Xpnss.Modules.Categories.Data;
using Habanerio.Xpnss.Modules.Categories.Interfaces;
using MongoDB.Bson;
using Moq;

namespace Habanerio.Xpnss.Tests.Unit.Modules.Categories.CQRS;

public class CreateCategoryTests
{
    private readonly Mock<ICategoriesRepository> _repository;

    private readonly CreateCategory.Handler _handler;

    public CreateCategoryTests()
    {
        _repository = new Mock<ICategoriesRepository>();

        _handler = new CreateCategory.Handler(_repository.Object);
    }

    [Fact]
    public void Can_Instantiate_Handler()
    {
        var handler = new CreateCategory.Handler(_repository.Object);

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
    public async Task CanCall_CreateCategory()
    {
        // Arrange
        var expectedUserId = "test-user-id";
        var expectedName = "Category Name";
        var expectedDescription = "Some description";
        var expectedSortOrder = 51;

        var expectedDocument = CategoryDocument.New(
            expectedUserId,
            expectedName,
            expectedDescription,
            expectedSortOrder
        );

        var command = new CreateCategory.Command(
            expectedUserId,
            expectedName,
            expectedDescription,
            expectedSortOrder);

        _repository.Setup(x =>
                x.AddAsync(It.IsAny<CategoryDocument>(),
                    CancellationToken.None))
            .ReturnsAsync(Result.Ok(expectedDocument));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(ObjectId.Parse(result.Value.Id) != ObjectId.Empty);
    }

    [Fact]
    public async Task CreateCategory_WithEmptyUserId_ReturnsError()
    {
        // Arrange
        var expectedUserId = "";
        var expectedName = "Category Name";
        var expectedDescription = "Some description";
        var expectedSortOrder = 51;

        var command = new CreateCategory.Command(
            expectedUserId,
            expectedName,
            expectedDescription,
            expectedSortOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("'User Id' must not be empty.", result.Errors[0].Message);
    }

    [Fact]
    public async Task CreateCategory_WithEmptyName_ReturnsError()
    {
        // Arrange
        var expectedUserId = "test-user-id";
        var expectedName = "";
        var expectedDescription = "Some description";
        var expectedSortOrder = 51;

        var command = new CreateCategory.Command(
            expectedUserId,
            expectedName,
            expectedDescription,
            expectedSortOrder);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("'Name' must not be empty.", result.Errors[0].Message);
    }
}