using Habanerio.Xpnss.Domain.Entities;
using Habanerio.Xpnss.Domain.ValueObjects;

namespace Habanerio.Xpnss.Categories.Domain.Entities;

public class Category : AggregateRoot<CategoryId>
{
    private readonly List<SubCategory> _subCategories;

    public UserId UserId { get; private set; }

    public CategoryName Name { get; private set; }

    public string Description { get; private set; }

    public int SortOrder { get; set; }

    public IReadOnlyCollection<SubCategory> SubCategories => _subCategories
        .OrderBy(c => c.SortOrder).ToList().AsReadOnly();


    private Category(
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        IEnumerable<SubCategory> subCategories
    ) : this(
            CategoryId.New,
            userId,
            name,
            description,
            sortOrder,
            subCategories,
            DateTime.UtcNow)
    {
        IsTransient = true;

        ResortSubCategories();
        // Add `CategoryCreated` Domain Event
    }

    private Category(
        CategoryId id,
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        IEnumerable<SubCategory> subCategories,
        DateTime dateCreated,
        DateTime? dateUpdated = null,
        DateTime? dateDeleted = null) : base(id)
    {
        if (id is null)
            throw new ArgumentNullException(nameof(id), $"{nameof(id)} cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(id) || id.Equals(CategoryId.Empty))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (userId is null || id.Equals(UserId.Empty))
            throw new ArgumentNullException(nameof(userId), $"{nameof(userId)} cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));

        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        SortOrder = sortOrder < 0 ? 99 : sortOrder;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;

        _subCategories = [.. subCategories];

        ResortSubCategories();
    }

    public static Category Load(
        CategoryId id,
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder,
        IEnumerable<SubCategory> subCategories,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new Category(
            id,
            userId,
            name,
            description,
            sortOrder,
            subCategories,
            dateCreated,
            dateUpdated,
            dateDeleted);
    }

    public static Category New(
        UserId userId,
        CategoryName name,
        string description,
        int sortOrder = 99)
    {
        return new Category(
            userId,
            name,
            description,
            sortOrder,
            []);
    }

    public void Delete()
    {
        if (!IsDeleted)
        {
            DateDeleted = DateTime.UtcNow;

            //TODO: Add `CategoryDeleted` Domain Event
        }
    }

    public void Update(CategoryName name, string description, int sortOrder)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted Category.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} cannot be null or whitespace.", nameof(name));

        sortOrder = sortOrder < 1 ? 99 : sortOrder;

        Name = name;
        Description = description;
        SortOrder = sortOrder;
        DateUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a new Sub Category to this Parent Category
    /// This is the only way to create a new Sub Category
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddSubCategory(CategoryName subCategoryName, string subCategoryDescription, int sortOrder)
    {
        var existingSubCategory = _subCategories.Find(c =>
            c.Name.Equals(subCategoryName));

        // If the SubCategory already exists, then maybe it has a new description?
        if (existingSubCategory is not null)
        {
            existingSubCategory.Description = subCategoryDescription;
            existingSubCategory.SortOrder = sortOrder;

            return;
        }

        var newSubCategory = new SubCategory(
            Id,
            subCategoryName,
            subCategoryDescription,
            sortOrder);

        _subCategories.Add(newSubCategory);

        ResortSubCategories();

        // Add `SubCategoryCreated` Domain Event ?
    }

    public void RemoveSubCategory(SubCategoryId subCategoryId)
    {
        var subCategory = _subCategories.Find(c => c.Id == subCategoryId);

        if (subCategory is null)
            return;

        subCategory.Delete();

        // Do we really want to remove it from the list, or just mark it as deleted?
        // Could permanently remove it after so many days?
        //_subCategories.Remove(subCategory);

        var idx = 1;

        foreach (var subCategory2 in _subCategories
                     .OrderBy(c => c.SortOrder)
                     .ThenBy(c => c.Name.Value))
        {
            subCategory2.SortOrder = idx;

            idx++;
        }

        // Add `SubCategoryDeleted` Domain Event
    }

    private void ResortSubCategories()
    {
        // Reorder the SubCategories
        var idx = 1;

        var sortedCategories = _subCategories.OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name.Value)
            .ToList();

        foreach (var subCategory in sortedCategories)
        {
            subCategory.SortOrder = idx;

            idx++;
        }
    }
}

public class SubCategory : Entity<SubCategoryId>
{
    public CategoryName Name { get; internal set; }

    public string Description { get; internal set; }

    public CategoryId ParentId { get; }

    public int SortOrder { get; internal set; }

    // New SubCategories
    internal SubCategory(CategoryId parentId, CategoryName name, string description, int sortOrder) :
        base(SubCategoryId.New)
    {
        Name = name;
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder < 1 ? 99 : sortOrder;
    }

    // Existing SubCategories
    private SubCategory(
        SubCategoryId id,
        CategoryId parentId,
        CategoryName name,
        string description,
        int sortOrder,
        DateTime dateCreated, DateTime? dateUpdated, DateTime? dateDeleted) :
        base(id)
    {
        Name = name;
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder < 1 ? 99 : sortOrder;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        DateDeleted = dateDeleted;
    }

    public static SubCategory Load(
        SubCategoryId id,
        CategoryId parentId,
        CategoryName name,
        string description,
        int sortOrder,
        DateTime dateCreated,
        DateTime? dateUpdated,
        DateTime? dateDeleted)
    {
        return new SubCategory(
            id,
            parentId,
            name,
            description,
            sortOrder,
            dateCreated, dateUpdated, dateDeleted);
    }

    internal void Delete()
    {
        if (!IsDeleted)
        {
            DateDeleted = DateTime.UtcNow;
        }
    }
}