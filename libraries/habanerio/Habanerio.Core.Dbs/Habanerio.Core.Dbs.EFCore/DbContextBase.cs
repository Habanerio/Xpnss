using Microsoft.EntityFrameworkCore;

namespace Habanerio.Core.DBs.EFCore;

public abstract class DbContextBase : DbContext
{
    protected DbContextBase() { }

    protected DbContextBase(DbContextOptions options) : base(options)
    {

    }
}