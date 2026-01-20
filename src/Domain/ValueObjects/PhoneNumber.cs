namespace CustomerApi.Domain.ValueObjects;

public record PhoneNumber
{
    public string CountryCode { get; init; }
    public string Number { get; init; }

    private PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    public static PhoneNumber Create(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length < 7)
            throw new ArgumentException("Invalid phone number length.");

        // Logic here ensures that the Domain remains "pure" and valid
        return new PhoneNumber(countryCode, number.Trim());
    }

    public string FullNumber => $"{CountryCode}{Number}";
}