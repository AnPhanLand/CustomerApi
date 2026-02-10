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
    
    // This allows your Handlers to save changes without seeing the DB implementation
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}