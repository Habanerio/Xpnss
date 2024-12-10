namespace Habanerio.Xpnss.Shared.Types;

public static class EntityEnums
{
    public enum Keys
    {
        ACCOUNT,
        CATEGORY,
        SUBCATEGORY,
        PAYER_PAYEE,
        TRANSACTION,
        USER
    }

    public static string GetKey(Keys key) =>
        key switch
        {
            Keys.ACCOUNT => "Account",
            Keys.CATEGORY => "Category",
            Keys.SUBCATEGORY => "Subcategory",
            Keys.PAYER_PAYEE => "PayerPayee",
            Keys.TRANSACTION => "Transaction",
            Keys.USER => "User",
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

    public static Keys GetKey(string key)
    {
        return key.ToLower() switch
        {
            "account" => Keys.ACCOUNT,
            "category" => Keys.CATEGORY,
            "subcategory" => Keys.SUBCATEGORY,
            "payerpayee" => Keys.PAYER_PAYEE,
            "transaction" => Keys.TRANSACTION,
            "user" => Keys.USER,
            _ => throw new ArgumentException("Invalid key", nameof(key))
        };
    }

    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<Keys>()
            .ToDictionary(k => (int)k, v => v.ToString());
    }
}