using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // List every DbSet that your Application logic needs to access
    DbSet<Customer> Customers { get; set; }
    public DbSet<PhieuThu> PhieuThus { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<PhuHuynh> PhuHuynhs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BienLai> BienLais { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Balance> Balances { get; set; }

    // This allows your Handlers to save changes without seeing the DB implementation
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);

            // This creates the "thuộc về (tài khoản cha)" loop from your ERD
            entity.HasOne(d => d.ParentAccount)
                .WithMany(p => p.ChildAccounts)
                .HasForeignKey(d => d.parent_account_id)
                // Use Restrict or NoAction to avoid circular delete issues in some SQL versions
                .OnDelete(DeleteBehavior.Restrict); 
        });
    }

}