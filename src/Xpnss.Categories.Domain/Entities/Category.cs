using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Categories.Domain.Entities;

public class Category : AggregateRoot<CategoryId>
{
    private readonly List<Category> _subCategories;

    public UserId UserId { get; private set; }

    public CategoryName Name { get; private set; }

    public string Description { get; private set; }

    public int SortOrder { get; set; }

    public CategoryId ParentId { get; private set; }

    public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

    private Category(
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        CategoryId parentId,
        IEnumerable<Category> subCategories
    ) : this(
            parentId,
            userId,
            name,
            description,
            sortOrder,
            parentId,
            subCategories,
            DateTime.UtcNow)
    {
        IsTransient = true;

        // Add `CategoryCreated` Domain Event
    }

    private Category(
        CategoryId id,
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        CategoryId parentId,
        IEnumerable<Category> subCategories,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
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
            userId,
            name,
            description,
            sortOrder,
            parentId,
            []);
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

    public void Delete()
    {
        if (!IsDeleted)
            DateDeleted = DateTime.UtcNow;
    }

    public void Update(CategoryName name, string description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(sortOrder), $"{nameof(sortOrder)} cannot be less than 0.");

        Name = name;
        Description = description;
        SortOrder = sortOrder;
        DateUpdated = DateTime.UtcNow;
    }
}