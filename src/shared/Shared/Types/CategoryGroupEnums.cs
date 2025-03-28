namespace Habanerio.Xpnss.Shared.Types;

public static class CategoryGroupEnums
{
    public enum CategoryKeys
    {
        NA = -1,

        /// <summary>
        /// Anything earned from work or services provided
        /// </summary>
        REVENUE,

        /// <summary>
        /// Anything earned through investments
        /// </summary>
        INVESTMENTS,

        /// <summary>
        /// Anything paid towards debts, such as interest.
        /// </summary>
        DEBTS,

        /// <summary>
        /// Anything spent
        /// </summary>
        EXPENSES,


    }

    public static Dictionary<int, string> ToDictionary()
    {
        return Enum.GetValues<CategoryKeys>().ToDictionary(k => (int)k, v => v.ToString());
    }
}