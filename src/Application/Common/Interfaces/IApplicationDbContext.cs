using CustomerApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // List every DbSet that your Application logic needs to access
    DbSet<Customer> Customers { get; }
    
    // This allows your Handlers to save changes without seeing the DB implementation
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}