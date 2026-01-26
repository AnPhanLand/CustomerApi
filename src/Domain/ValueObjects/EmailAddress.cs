using System.Text.Json.Serialization;

namespace CustomerApi.Domain.ValueObjects;

public record EmailAddress
{
    public string Value { get; init; }

    // 1. Make this public so the JSON tools can reach it
    // 2. Add the attribute to tell .NET to use this specific constructor
    [JsonConstructor]
    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        // Basic Regex validation to ensure business rules are met at the core
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email format.");

        return new EmailAddress(email.ToLower().Trim());
    }
}