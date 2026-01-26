using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerApi.Infrastructure.Persistence.Postgres;

public class CustomerDbFactory : IDesignTimeDbContextFactory<CustomerDb>
{
    public CustomerDb CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerDb>();
        
        // Use the same connection string you have in DependencyInjection
        var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword";
        
        optionsBuilder.UseNpgsql(connectionString, x => 
            x.MigrationsAssembly(typeof(CustomerDb).Assembly.FullName));

        return new CustomerDb(optionsBuilder.Options);
    }
}