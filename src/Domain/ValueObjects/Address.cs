namespace CustomerApp.Domain.ValueObjects;

// In modern C#, 'record' is perfect for Value Objects
public record Address(string Street, string City, string PostalCode);