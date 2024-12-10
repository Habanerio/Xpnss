using System.Collections.ObjectModel;
using AutoFixture;
using Habanerio.Xpnss.Categories.Domain.Entities;
using Habanerio.Xpnss.Shared.Types;
using Habanerio.Xpnss.Shared.ValueObjects;
using MongoDB.Bson;

namespace Habanerio.Xpnss.Tests.Unit.Domain.Categories;

public class CategoryTests : TestsBase
{
    private readonly Category _testClass;

    public CategoryTests()
    {
        var id = AutoFixture.Create<CategoryId>();
        var userId = AutoFixture.Create<UserId>();
        var name = AutoFixture.Create<CategoryName>();
        var categoryType = AutoFixture.Create<CategoryGroupEnums.CategoryKeys>();
        var description = AutoFixture.Create<string>();
        var sortOrder = AutoFixture.Create<int>();
        var subCategories = new List<SubCategory>
        {
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>()
        };
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateUpdated = AutoFixture.Create<DateTime?>();
        var dateDeleted = AutoFixture.Create<DateTime?>();

        _testClass = Category.Load(
            id,
            userId,
            name,
            categoryType,
            description,
            sortOrder,
            subCategories,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    [Fact]
    public void CanCall_Load()
    {
        // Arrange
        var id = AutoFixture.Create<CategoryId>();
        var userId = AutoFixture.Create<UserId>();
        var name = AutoFixture.Create<CategoryName>();
        var categoryType = AutoFixture.Create<CategoryGroupEnums.CategoryKeys>();
        var description = AutoFixture.Create<string>();
        var sortOrder = AutoFixture.Create<int>();
        var subCategories = new List<SubCategory>
        {
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>()
        };
        var dateCreated = AutoFixture.Create<DateTime>();
        var dateUpdated = AutoFixture.Create<DateTime?>();
        var dateDeleted = AutoFixture.Create<DateTime?>();

        // Act
        var result = Category.Load(
            id,
            userId,
            name,
            categoryType,
            description,
            sortOrder,
            subCategories,
            dateCreated,
            dateUpdated,
            dateDeleted);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Category>(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(name, result.Name);
        Assert.Equal(description, result.Description);
        Assert.Equal(sortOrder, result.SortOrder);
        Assert.Equal(subCategories, result.SubCategories);
        Assert.Equal(dateCreated, result.DateCreated);
        Assert.Equal(dateUpdated, result.DateUpdated);
        Assert.Equal(dateDeleted, result.DateDeleted);
    }

    [Fact]
    public void CannotCall_Load_WithNull_Id()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.Load(
                null,
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));

        Assert.Throws<ArgumentNullException>(() =>
            Category.Load(
                default(CategoryId),
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));

        Assert.Throws<ArgumentException>(() =>
            Category.Load(
                new CategoryId(ObjectId.Empty),
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));

        Assert.Throws<ArgumentException>(() =>
            Category.Load(
                new CategoryId(string.Empty),
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.Load(
                AutoFixture.Create<CategoryId>(),
                default(UserId),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Fact]
    public void CannotCall_Load_WithNull_SubCategories()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.Load(
                AutoFixture.Create<CategoryId>(),
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>(),
                default(IEnumerable<SubCategory>),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_Load_WithInvalid_Description(string value)
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.Load(
                AutoFixture.Create<CategoryId>(),
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                value,
                AutoFixture.Create<int>(),
                AutoFixture.Create<IEnumerable<SubCategory>>(),
                AutoFixture.Create<DateTime>(),
                AutoFixture.Create<DateTime?>(),
                AutoFixture.Create<DateTime?>()));
    }


    [Fact]
    public void CanCall_New()
    {
        // Arrange
        var userId = AutoFixture.Create<UserId>();
        var name = AutoFixture.Create<CategoryName>();
        var categoryType = AutoFixture.Create<CategoryGroupEnums.CategoryKeys>();
        var description = AutoFixture.Create<string>();
        var sortOrder = AutoFixture.Create<int>();

        // Act
        var result = Category.New(userId, name, categoryType, description, sortOrder);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Category>(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(name, result.Name);
        Assert.Equal(description, result.Description);
        Assert.Empty(result.SubCategories);
    }

    [Fact]
    public void CannotCall_New_WithNull_UserId()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.New(
                default(UserId),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<int>()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CannotCall_New_WithInvalid_Description(string value)
    {
        Assert.Throws<ArgumentNullException>(() =>
            Category.New(
                AutoFixture.Create<UserId>(),
                AutoFixture.Create<CategoryName>(),
                AutoFixture.Create<CategoryGroupEnums.CategoryKeys>(),
                value,
                AutoFixture.Create<int>()));
    }


    [Fact]
    public void CanCall_AddSubCategory()
    {
        // Arrange
        var subCatName = AutoFixture.Create<CategoryName>();
        var description = AutoFixture.Create<string>();
        var sortOrder = AutoFixture.Create<int>();

        // Act
        _testClass.AddSubCategory(subCatName, description, sortOrder);

        // Assert
        var actual = _testClass.SubCategories.FirstOrDefault(c => c.Name.Equals(subCatName));
    }

    [Fact]
    public void CanCall_Delete()
    {
        // Act
        _testClass.Delete();

        // Assert
        Assert.True(_testClass.IsDeleted);
        Assert.NotNull(_testClass.DateDeleted);
    }

    [Fact]
    public void CanCall_Update()
    {
        // Arrange
        var id = AutoFixture.Create<CategoryId>();
        var userId = AutoFixture.Create<UserId>();
        var name = AutoFixture.Create<CategoryName>();
        var categoryType = AutoFixture.Create<CategoryGroupEnums.CategoryKeys>();
        var description = AutoFixture.Create<string>();
        var sortOrder = AutoFixture.Create<int>();
        var subCategories = new List<SubCategory>
        {
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
            AutoFixture.Create<SubCategory>(),
        };

        var dateCreated = AutoFixture.Create<DateTime>();
        DateTime? dateUpdated = null;
        DateTime? dateDeleted = null;

        // Act
        var actualClass = Category.Load(
            id,
            userId,
            name,
            categoryType,
            description,
            sortOrder,
            subCategories,
            dateCreated,
            dateUpdated,
            dateDeleted);

        // Arrange
        var expectedName = AutoFixture.Create<CategoryName>();
        var expectedDescription = AutoFixture.Create<string>();
        var updatedSortOrder = 3534;

        // Act
        actualClass.Update(expectedName, expectedDescription, updatedSortOrder);

        // Assert
        Assert.Equal(expectedName, actualClass.Name);
        Assert.Equal(expectedDescription, actualClass.Description);
        Assert.Equal(updatedSortOrder, actualClass.SortOrder);
    }

    [Fact]
    public void CanGet_UserId()
    {
        // Assert
        Assert.IsType<UserId>(_testClass.UserId);

        Assert.False(string.IsNullOrWhiteSpace(_testClass.UserId));
    }

    [Fact]
    public void CanGet_Name()
    {
        // Assert
        Assert.IsType<CategoryName>(_testClass.Name);

        Assert.False(string.IsNullOrWhiteSpace(_testClass.Name));
    }

    [Fact]
    public void CanGet_Description()
    {
        // Assert
        Assert.IsType<string>(_testClass.Description);

        Assert.False(string.IsNullOrWhiteSpace(_testClass.Description));
    }

    [Fact]
    public void CanSet_And_Get_SortOrder()
    {
        // Assert
        Assert.True(_testClass.SortOrder > 0);
    }

    [Fact]
    public void CanGet_SubCategories()
    {
        // Assert
        Assert.IsType<ReadOnlyCollection<SubCategory>>(_testClass.SubCategories);

        Assert.Equal(5, _testClass.SubCategories.Count);

        var expectedSortOrder = 1;

        foreach (var subCategory in _testClass.SubCategories)
        {
            Assert.Equal(expectedSortOrder, subCategory.SortOrder);

            expectedSortOrder++;
        }
    }
}