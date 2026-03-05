namespace CustomerApi.Infrastructure.Persistence.Postgres;
public class CustomerDb : DbContext, IApplicationDbContext
{
    public CustomerDb(DbContextOptions<CustomerDb> options)
        : base(options) { }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PhieuThu> PhieuThus { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<PhuHuynh> PhuHuynhs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BienLai> BienLais { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Balance> Balances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDb).Assembly);
    }
}