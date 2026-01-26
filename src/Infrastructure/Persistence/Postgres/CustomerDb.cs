namespace CustomerApi.Infrastructure.Persistence.Postgres;
public class CustomerDb : DbContext, IApplicationDbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Customer>(entity =>
        // {
        //     // Tells EF that EmailAddress is owned by Customer
        //     entity.OwnsOne(c => c.Email, email =>
        //     {
        //         // Maps the 'Value' property of EmailAddress to a column named 'Email'
        //         email.Property(e => e.Value).HasColumnName("Email"); 
        //     });

        //     // Tells EF that PhoneNumber is owned by Customer
        //     entity.OwnsOne(c => c.ContactNumber, phone =>
        //     {
        //         phone.Property(p => p.CountryCode).HasColumnName("CountryCode");
        //         phone.Property(p => p.Number).HasColumnName("PhoneNumber");
        //     });
        // });

        // base.OnModelCreating(modelBuilder);

        // This looks for every class in this project that implements IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDb).Assembly);
    }
}