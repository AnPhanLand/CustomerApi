using System.Text.RegularExpressions;
using CustomerApi.Domain.Common;

namespace CustomerApi.Domain.ValueObjects;

public record EmailAddress
{
    public string Value { get; init; }

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