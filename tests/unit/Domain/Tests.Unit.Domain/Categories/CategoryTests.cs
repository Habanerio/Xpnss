//using AutoFixture;
//using AutoFixture.AutoMoq;
//using Habanerio.Xpnss.Categories.Domain.Entities;
//using Habanerio.Xpnss.Domain.ValueObjects;

//namespace Habanerio.Xpnss.Tests.Unit.Domain.Categories;

//public class CategoryTests
//{
//    private readonly Category _testClass;
//    private IFixture _fixture;

//    public CategoryTests()
//    {
//        _fixture = new Fixture().Customize(new AutoMoqCustomization());
//        _testClass = _fixture.Create<Category>();
//    }

//    [Fact]
//    public void CanCall_Load()
//    {
//        // Arrange
//        var id = _fixture.Create<CategoryId>();
//        var userId = _fixture.Create<UserId>();
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var sortOrder = _fixture.Create<int>();
//        var parentId = _fixture.Create<CategoryId>();
//        var subCategories = _fixture.Create<IEnumerable<Category>>();
//        var dateCreated = _fixture.Create<DateTime>();
//        var dateUpdated = _fixture.Create<DateTime?>();
//        var dateDeleted = _fixture.Create<DateTime?>();

//        // Act
//        var result = Category.Load(id, userId, name, description, sortOrder, parentId, subCategories, dateCreated, dateUpdated, dateDeleted);

//        // Assert
//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CannotCall_Load_WithNull_Id()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.Load(default(CategoryId), _fixture.Create<UserId>(), _fixture.Create<CategoryName>(), _fixture.Create<string>(), _fixture.Create<int>(), _fixture.Create<CategoryId>(), _fixture.Create<IEnumerable<Category>>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime?>(), _fixture.Create<DateTime?>()));
//    }

//    [Fact]
//    public void CannotCall_Load_WithNull_UserId()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.Load(_fixture.Create<CategoryId>(), default(UserId), _fixture.Create<CategoryName>(), _fixture.Create<string>(), _fixture.Create<int>(), _fixture.Create<CategoryId>(), _fixture.Create<IEnumerable<Category>>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime?>(), _fixture.Create<DateTime?>()));
//    }

//    [Fact]
//    public void CannotCall_Load_WithNull_ParentId()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.Load(_fixture.Create<CategoryId>(), _fixture.Create<UserId>(), _fixture.Create<CategoryName>(), _fixture.Create<string>(), _fixture.Create<int>(), default(CategoryId), _fixture.Create<IEnumerable<Category>>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime?>(), _fixture.Create<DateTime?>()));
//    }

//    [Fact]
//    public void CannotCall_Load_WithNull_SubCategories()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.Load(_fixture.Create<CategoryId>(), _fixture.Create<UserId>(), _fixture.Create<CategoryName>(), _fixture.Create<string>(), _fixture.Create<int>(), _fixture.Create<CategoryId>(), default(IEnumerable<Category>), _fixture.Create<DateTime>(), _fixture.Create<DateTime?>(), _fixture.Create<DateTime?>()));
//    }

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    [InlineData("   ")]
//    public void CannotCall_Load_WithInvalid_Description(string value)
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.Load(_fixture.Create<CategoryId>(), _fixture.Create<UserId>(), _fixture.Create<CategoryName>(), value, _fixture.Create<int>(), _fixture.Create<CategoryId>(), _fixture.Create<IEnumerable<Category>>(), _fixture.Create<DateTime>(), _fixture.Create<DateTime?>(), _fixture.Create<DateTime?>()));
//    }

//    [Fact]
//    public void Load_PerformsMapping()
//    {
//        // Arrange
//        var id = _fixture.Create<CategoryId>();
//        var userId = _fixture.Create<UserId>();
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var sortOrder = _fixture.Create<int>();
//        var parentId = _fixture.Create<CategoryId>();
//        var subCategories = _fixture.Create<IEnumerable<Category>>();
//        var dateCreated = _fixture.Create<DateTime>();
//        var dateUpdated = _fixture.Create<DateTime?>();
//        var dateDeleted = _fixture.Create<DateTime?>();

//        // Act
//        var result = Category.Load(id, userId, name, description, sortOrder, parentId, subCategories, dateCreated, dateUpdated, dateDeleted);

//        // Assert
//        Assert.Same(userId, result.UserId);
//        Assert.Equal(name, result.Name);
//        Assert.Same(description, result.Description);
//        Assert.Equal(sortOrder, result.SortOrder);
//        Assert.Same(parentId, result.ParentId);
//        Assert.Same(subCategories, result.SubCategories);
//    }

//    [Fact]
//    public void CanCall_New()
//    {
//        // Arrange
//        var userId = _fixture.Create<UserId>();
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var parentId = _fixture.Create<CategoryId>();
//        var sortOrder = _fixture.Create<int>();

//        // Act
//        var result = Category.New(userId, name, description, parentId, sortOrder);

//        // Assert
//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CannotCall_New_WithNull_UserId()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.New(default(UserId), _fixture.Create<CategoryName>(), _fixture.Create<string>(), _fixture.Create<CategoryId>(), _fixture.Create<int>()));
//    }

//    [Fact]
//    public void CannotCall_New_WithNull_ParentId()
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.New(_fixture.Create<UserId>(), _fixture.Create<CategoryName>(), _fixture.Create<string>(), default(CategoryId), _fixture.Create<int>()));
//    }

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    [InlineData("   ")]
//    public void CannotCall_New_WithInvalid_Description(string value)
//    {
//        Assert.Throws<ArgumentNullException>(() =>
//            Category.New(_fixture.Create<UserId>(), _fixture.Create<CategoryName>(), value, _fixture.Create<CategoryId>(), _fixture.Create<int>()));
//    }

//    [Fact]
//    public void New_PerformsMapping()
//    {
//        // Arrange
//        var userId = _fixture.Create<UserId>();
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var parentId = _fixture.Create<CategoryId>();
//        var sortOrder = _fixture.Create<int>();

//        // Act
//        var result = Category.New(userId, name, description, parentId, sortOrder);

//        // Assert
//        Assert.Same(userId, result.UserId);
//        Assert.Equal(name, result.Name);
//        Assert.Same(description, result.Description);
//        Assert.Same(parentId, result.ParentId);
//        Assert.Equal(sortOrder, result.SortOrder);
//    }

//    [Fact]
//    public void CanCall_AddSubCategory()
//    {
//        // Arrange
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var sortOrder = _fixture.Create<int>();

//        // Act
//        var result = _testClass.AddSubCategory(name, description, sortOrder);

//        // Assert
//        throw new NotImplementedException("Create or modify test");
//    }

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    [InlineData("   ")]
//    public void CannotCall_AddSubCategory_WithInvalid_Description(string value)
//    {
//        Assert.Throws<ArgumentNullException>(() => _testClass.AddSubCategory(_fixture.Create<CategoryName>(), value, _fixture.Create<int>()));
//    }

//    [Fact]
//    public void AddSubCategory_PerformsMapping()
//    {
//        // Arrange
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var sortOrder = _fixture.Create<int>();

//        // Act
//        var result = _testClass.AddSubCategory(name, description, sortOrder);

//        // Assert
//        Assert.Equal(name, result.Name);
//        Assert.Same(description, result.Description);
//        Assert.Equal(sortOrder, result.SortOrder);
//    }

//    [Fact]
//    public void CanCall_Delete()
//    {
//        // Act
//        _testClass.Delete();

//        // Assert
//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CanCall_Update()
//    {
//        // Arrange
//        var name = _fixture.Create<CategoryName>();
//        var description = _fixture.Create<string>();
//        var sortOrder = _fixture.Create<int>();

//        // Act
//        _testClass.Update(name, description, sortOrder);

//        // Assert
//        throw new NotImplementedException("Create or modify test");
//    }

//    [Theory]
//    [InlineData(null)]
//    [InlineData("")]
//    [InlineData("   ")]
//    public void CannotCall_Update_WithInvalid_Description(string value)
//    {
//        Assert.Throws<ArgumentNullException>(() => _testClass.Update(_fixture.Create<CategoryName>(), value, _fixture.Create<int>()));
//    }

//    [Fact]
//    public void CanGet_UserId()
//    {
//        // Assert
//        Assert.IsType<UserId>(_testClass.UserId);

//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CanGet_Name()
//    {
//        // Assert
//        Assert.IsType<CategoryName>(_testClass.Name);

//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CanGet_Description()
//    {
//        // Assert
//        Assert.IsType<string>(_testClass.Description);

//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CanSet_And_Get_SortOrder()
//    {
//        // Arrange
//        var testValue = _fixture.Create<int>();

//        // Act
//        _testClass.SortOrder = testValue;

//        // Assert
//        Assert.Equal(testValue, _testClass.SortOrder);
//    }

//    [Fact]
//    public void CanGet_ParentId()
//    {
//        // Assert
//        Assert.IsType<CategoryId>(_testClass.ParentId);

//        throw new NotImplementedException("Create or modify test");
//    }

//    [Fact]
//    public void CanGet_SubCategories()
//    {
//        // Assert
//        Assert.IsType<IReadOnlyCollection<Category>>(_testClass.SubCategories);

//        throw new NotImplementedException("Create or modify test");
//    }
//}