namespace CustomerApi.Domain.Entities;

public class Account
{
    public int account_id { get; set; }
    public string account_name { get; set; } = string.Empty;
    public string account_number { get; set; } = string.Empty;
    public int parent_account_id { get; set; }

    public virtual Account? ParentAccount { get; set; }
    public virtual ICollection<Account> ChildAccounts { get; set; } = new List<Account>();
}