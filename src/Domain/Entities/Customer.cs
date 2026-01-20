namespace CustomerApi.Domain.Entities;
public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    // Using the Enum here
    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;
    public MembershipLevel MembershipLevel { get; set; } = MembershipLevel.Standard;
    public string Region { get; set; } = CustomerRules.DefaultRegion;
// Using Value Objects instead of strings
    public EmailAddress Email { get; private set; } 
    public PhoneNumber ContactNumber { get; private set; }
    // Constructor ensures these are set immediately
    public Customer(EmailAddress email, PhoneNumber contactNumber)
    {
        Email = email;
        ContactNumber = contactNumber;
    }
    // Required for EF Core to recreate the entity from the database
    private Customer() { }

    // The "n" part of the relationship: A list of orders
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public void PlaceOrder(decimal amount)
    {
        // Business logic: perhaps check if customer status is Active first
        var order = new Order(this.Id, amount);
        _orders.Add(order);
        
        // You could also raise a 'OrderPlacedEvent' here
    }
    
    public void UpgradeMembership(MembershipLevel newLevel)
    {
        if (this.MembershipLevel != newLevel)
        {
            var oldLevel = this.MembershipLevel;
            this.MembershipLevel = newLevel;

            // Add the event to the entity's internal list
            this.AddDomainEvent(new MembershipLevelUpgradedEvent(this.Id, oldLevel, newLevel));
        }
    }

    public void UpdateEmail(string newEmail)
    {
        var emailObject = EmailAddress.Create(newEmail); // Validation happens here!
        if (this.Email != emailObject)
        {
            this.Email = emailObject;
        }
    }

    public void UpdatePhoneNumber(string countryCode, string number)
    {
        // The Value Object logic validates the input
        var newPhoneNumber = PhoneNumber.Create(countryCode, number);

        // Only update if the value actually changed
        if (this.ContactNumber != newPhoneNumber)
        {
            this.ContactNumber = newPhoneNumber;
        }
    }
}

