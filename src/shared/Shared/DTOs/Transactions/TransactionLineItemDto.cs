using System.Text.Json.Serialization;

namespace Habanerio.Xpnss.Shared.DTOs.Transactions;

public record TransactionLineItemDto
{
    public string Id { get; init; }

    public KeyValuePair<string, string>? Account { get; set; } = null;

    public KeyValuePair<string, string>? Category { get; set; } = null;

    public bool IsCredit { get; set; }

    public KeyValuePair<string, string>? PayerPayee { get; set; } = null;

    public KeyValuePair<string, string>? SubCategory { get; set; } = null;

    public decimal? CreditAmount { get; set; }

    public decimal? DebitAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string Title { get; set; }

    public DateTime TransactionDate { get; set; }

    public string TransactionType { get; set; }

    [JsonConstructor]
    public TransactionLineItemDto() { }

    public TransactionLineItemDto(
        string id,
        string accountId,
        string categoryId,
        bool isCredit,
        string payerPayeeId,
        string subCategoryId,
        string title,
        decimal totalAmount,
        DateTime transactionDate,
        string transactionType)
    {
        Id = id;
        IsCredit = isCredit;
        CreditAmount = isCredit == true ? totalAmount : null;
        DebitAmount = isCredit == false ? totalAmount : null;
        TotalAmount = totalAmount;
        Title = title;
        TransactionDate = transactionDate;
        TransactionType = transactionType;

        Account = !string.IsNullOrWhiteSpace(accountId)
            ? new KeyValuePair<string, string>(accountId, string.Empty)
            : default;

        Category = !string.IsNullOrWhiteSpace(categoryId)
            ? new KeyValuePair<string, string>(categoryId, string.Empty)
            : default;

        PayerPayee = !string.IsNullOrWhiteSpace(payerPayeeId)
            ? new KeyValuePair<string, string>(payerPayeeId, string.Empty)
            : default;

        SubCategory = !string.IsNullOrWhiteSpace(subCategoryId)
            ? new KeyValuePair<string, string>(subCategoryId, string.Empty)
            : default;
    }

    public void Deconstruct(
        out string id,
        out KeyValuePair<string, string>? account,
        out KeyValuePair<string, string>? category,
        out bool isCredit,
        out KeyValuePair<string, string>? payerPayee,
        out KeyValuePair<string, string>? subCategory,
        out decimal? creditAmount,
        out decimal? debitAmount,
        out decimal totalAmount,
        out string title,
        out DateTime transactionDate,
        out string transactionType)
    {
        id = Id;
        account = Account;
        category = Category;
        isCredit = IsCredit;
        payerPayee = PayerPayee;
        subCategory = SubCategory;
        creditAmount = CreditAmount;
        debitAmount = DebitAmount;
        totalAmount = TotalAmount;
        title = Title;
        transactionDate = TransactionDate;
        transactionType = TransactionType;
    }
}