namespace Habanerio.Core.DBs.EFCore.Interfaces;

//TODO: Should be move to base Habanerio.Core.Data.Common
public interface IDbEntity<out TId> where TId : IComparable<TId>, IEquatable<TId>
{
    TId Id { get; }


    //TId CreatedById { get; set; }

    //DateTimeOffset DateCreated { get; set; }


    //TId UpdatedById { get; set; }

    //DateTimeOffset? DateUpdated { get; set; }
}