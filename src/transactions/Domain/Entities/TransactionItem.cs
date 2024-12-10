using Habanerio.Xpnss.Shared.Entities;
using Habanerio.Xpnss.Shared.ValueObjects;

namespace Habanerio.Xpnss.Transactions.Domain.Entities;

public sealed class TransactionItem : Entity
{
    public Money Amount { get; set; }

    public CategoryId CategoryId { get; set; }

    public CategoryId SubCategoryId { get; set; }

    public string Description { get; init; }

    private TransactionItem(Money amount, CategoryId categoryId, SubCategoryId subCategoryId, string description) :
        this(TransactionItemId.New, amount, categoryId, subCategoryId, description)
    {
        IsTransient = true;
    }

    private TransactionItem
        (TransactionItemId id,
            Money amount,
            CategoryId categoryId,
            SubCategoryId subCategoryId,
            string description) :
        base(id)
    {
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
    }

    public static TransactionItem Load(
        TransactionItemId id,
        Money amount,
        CategoryId categoryId,
        SubCategoryId subCategoryId,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException($"{nameof(id)} cannot be null or whitespace.", nameof(id));

        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        return new TransactionItem(id, amount, categoryId, subCategoryId, description);
    }

    public static TransactionItem New(Money amount, CategoryId categoryId, SubCategoryId subCategoryId, string description = "")
    {
        if (amount < 0)
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0.", nameof(amount));

        return new TransactionItem(amount, categoryId, subCategoryId, description);
    }
}