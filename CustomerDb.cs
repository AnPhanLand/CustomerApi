using Microsoft.EntityFrameworkCore;
namespace CustomerApp;
class CustomerDb : DbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
}