namespace Habanerio.Xpnss.Shared.Types;

public static class CategoryGroupEnums
{
    public enum CategoryKeys
    {
        /// <summary>
        /// Anything earned
        /// </summary>
        INCOME,
        /// <summary>
        /// Anything saved ... could be investments too?
        /// </summary>
        SAVINGS,
        /// <summary>
        /// Anything owed ... credit cards, loans, mortgages, etc.
        /// </summary>
        DEBT,
        /// <summary>
        /// Anything spent
        /// </summary>
        EXPENSE,
    }

    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<CategoryKeys>().ToDictionary(k => (int)k, v => v.ToString());
    }
}