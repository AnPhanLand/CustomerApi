namespace CustomerApi.Infrastructure.Persistence.Postgres;
public class CustomerDb : DbContext, IApplicationDbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
}