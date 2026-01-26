using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerApi.Domain.Entities;

namespace CustomerApi.Infrastructure.Persistence.Postgres.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // 1. Table Name
        builder.ToTable("Customers");

        // 2. Mapping Value Objects (Fixes the migration errors)
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value).HasColumnName("Email").IsRequired();
        });

        builder.OwnsOne(c => c.ContactNumber, phone =>
        {
            phone.Property(p => p.CountryCode).HasColumnName("CountryCode");
            phone.Property(p => p.Number).HasColumnName("PhoneNumber");
        });
    }
}