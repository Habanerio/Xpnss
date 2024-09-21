using Habanerio.Core.DBs.EFCore.Interfaces;

namespace Habanerio.Core.DBs.EFCore;

public class DbEntity<TId> : IDbEntity<TId> where TId : IComparable<TId>, IEquatable<TId>
{
    public TId Id { get; set; } = default!;
}