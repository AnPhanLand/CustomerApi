using Microsoft.EntityFrameworkCore;
namespace CustomerApi.Infrastructure.Persistence.Postgres;
public class CustomerDb : DbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
}