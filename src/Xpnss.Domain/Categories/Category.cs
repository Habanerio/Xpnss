using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Domain.Categories;

public class Category : AggregateRoot<CategoryId>
{
    private readonly List<Category> _subCategories;

    public UserId UserId { get; }

    public CategoryName Name { get; }

    public string Description { get; }

    public int SortOrder { get; set; }

    public CategoryId ParentId { get; }

    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

    public bool IsDeleted => DateDeleted.HasValue;

    public DateTime DateCreated { get; }

    public DateTime? DateUpdated { get; }

    public DateTime? DateDeleted { get; }

    private Category(
        CategoryId id,
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        CategoryId parentId,
        IEnumerable<Category> subCategories,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted
    ) : base(id)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        SortOrder = sortOrder;
        ParentId = parentId;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _subCategories = [.. subCategories];
    }

    public static Category Load(
        CategoryId id,
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        CategoryId parentId,
        IEnumerable<Category> subCategories,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder), $"{nameof(sortOrder)} cannot be less than 0.");

        return new Category(
            id,
            userId,
            name,
            description,
            sortOrder,
            parentId,
            subCategories,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static Category New(
        UserId userId,
        CategoryName name,
        string description,
        CategoryId parentId,
        int sortOrder = 99)
    {
        return new Category(
            CategoryId.New,
            userId,
            name,
            description,
            sortOrder,
            parentId,
            [],
            DateTime.UtcNow,
            null,
            null);
    }

    public Category AddSubCategory(CategoryName name, string description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));

        var subCategory = New(UserId, name, description, Id, sortOrder);

        _subCategories.Add(subCategory);

        var idx = 1;
        foreach (var _subCategory in _subCategories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name.Value))
        {
            _subCategory.SortOrder = idx;
            idx++;
        }

        return subCategory;
    }
}