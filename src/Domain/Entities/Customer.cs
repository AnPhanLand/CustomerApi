namespace CustomerApi.Domain.Entities;

// Inheriting from BaseEntity gives Customer a unique Guid Id and the ability 
// to track Domain Events (messages for other parts of the system).
public class Customer : BaseEntity
{
    // Primitive properties for basic data.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // A computed property that isn't saved in the database but used in the UI.
    public string FullName => $"{FirstName} {LastName}";

    // Enums ensure the status and level can only be specific, valid values.
    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;
    public MembershipLevel MembershipLevel { get; set; } = MembershipLevel.Standard;
    
    // Constant or default business rules often live in a 'Rules' class.
    public string Region { get; set; } = CustomerRules.DefaultRegion;

    // Value Objects: These handle their own validation (e.g., checking for an '@' symbol)
    // and are immutable. 'private set' ensures they can only be changed via methods.
    public EmailAddress Email { get; private set; } 
    public PhoneNumber ContactNumber { get; private set; }

    // Public Constructor: Forces any new Customer to have an Email and Phone.
    public Customer(EmailAddress email, PhoneNumber contactNumber)
    {
        Email = email;
        ContactNumber = contactNumber;
    }

    // Private Constructor: Needed by EF Core to load data from PostgreSQL.
    private Customer() { }

    // One-to-Many Relationship: Encapsulates the list of orders so they 
    // can't be modified from outside this class without a method.
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    // Business Method: Handles logic for creating a new order for this customer.
    public void PlaceOrder(decimal amount)
    {
        var order = new Order(this.Id, amount);
        _orders.Add(order);
    }
    
    // Business Method: Changes state and announces it to the system via an Event.
    public void UpgradeMembership(MembershipLevel newLevel)
    {
        if (this.MembershipLevel != newLevel)
        {
            var oldLevel = this.MembershipLevel;
            this.MembershipLevel = newLevel;

            // This 'shouts' that an upgrade happened so handlers can send emails.
            this.AddDomainEvent(new MembershipLevelUpgradedEvent(this.Id, oldLevel, newLevel));
        }
    }

    // Specialized Update: Uses Value Object validation before assignment.
    public void UpdateEmail(string newEmail)
    {
        var emailObject = EmailAddress.Create(newEmail); 
        if (this.Email != emailObject) // Only update if data actually changed.
        {
            this.Email = emailObject;
        }
    }

    public void UpdatePhoneNumber(string countryCode, string number)
    {
        var newPhoneNumber = PhoneNumber.Create(countryCode, number);
        if (this.ContactNumber != newPhoneNumber)
        {
            this.ContactNumber = newPhoneNumber;
        }
    }
}