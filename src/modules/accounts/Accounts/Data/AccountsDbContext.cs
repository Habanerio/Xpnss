using Habanerio.Core.DBs.MongoDB.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Habanerio.Xpnss.Modules.Accounts.Data;

public class AccountsDbContext : MongoDbContext //<AccountsDbContext>
{
    protected DbSet<AccountDocument> MoneyAccounts { get; set; }

    public AccountsDbContext(IOptions<MongoDbSettings> options) :
        base(GetDbContextOptions(options))
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AccountDocument>(e =>
        {
            e.ToCollection("user_money_accounts");

            e.HasKey(x => new { x.UserId, x.Id });

            //e.HasKey(x => x.AccountId);

            e.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_UserId");

            e.Property(p => p.UserId)
                .HasMaxLength(36)
                .IsRequired();

            e.Property(p => p.AccountType)
                .HasAnnotation("BsonElement", "account_type")
                .HasMaxLength(36)
                .IsRequired();

            e.Property(p => p.Name)
                .HasAnnotation("BsonElement", "account_name")
                .HasMaxLength(36)
                .IsRequired();

            e.Property(p => p.Balance)
                .IsRequired();

            e.Property(p => p.Description)
                .HasAnnotation("BsonElement", "description")
                .HasMaxLength(255);

            e.Property(p => p.DisplayColor)
                .HasAnnotation("BsonElement", "display_color")
                .HasMaxLength(7);

            e.Property(p => p.IsCredit)
                .HasAnnotation("BsonElement", "is_credit");

            e.Property(p => p.IsDefault)
                .HasAnnotation("BsonElement", "is_default");

            e.Property(p => p.IsDeleted)
                .HasAnnotation("BsonElement", "is_deleted");

            e.Property(p => p.DateCreated)
                .HasAnnotation("BsonElement", "date_created")
                //.HasDefaultValue(DateTime.UtcNow)
                .IsRequired();

            e.Property(p => p.DateUpdated)
                .HasAnnotation("BsonElement", "date_updated");

            e.Property(p => p.DateDeleted)
                .HasAnnotation("BsonElement", "date_deleted");

            e.Property(p => p.ExtendedProps)
                .HasAnnotation("BsonElement", "extended_props");

            //e.Property(p => p.ChangeHistory)
            //    .HasAnnotation("BsonElement", "change_history");

            e.OwnsMany(c => c.ChangeHistory, t =>
            {
                t.Property(i => i.UserId);
                t.Property(t => t.AccountId);
                t.Property(i => i.NewValues);
                t.Property(i => i.OldValues);
                t.Property(i => i.Property);
                t.Property(i => i.Reason);
                t.Property(i => i.DateChanged);
            });

            e.OwnsMany(c => c.MonthlyTotals, t =>
            {
                t.Property(i => i.Month);
                t.Property(i => i.Year);
                t.Property(i => i.Total);
                t.Property(i => i.TransactionCount);
            });

            /* Not supported in EF for MongoDB
            e.HasDiscriminator<string>("AccountType")
                .HasValue<CheckingAccountDocument>(AccountType.CHECKING_ACCOUNT)
                .HasValue<SavingsAccountDocument>(AccountType.SAVINGS_ACCOUNT)
                .HasValue<CreditCardAccountDocument>(AccountType.CREDIT_CARD_ACCOUNT)
                .HasValue<LineOfCreditAccountDocument>(AccountType.LINE_OF_CREDIT_ACCOUNT);
            */
        });
    }
}