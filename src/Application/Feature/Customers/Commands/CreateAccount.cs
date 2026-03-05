public record CreateAccountCommand(AccountCreateDTO AccountDTO) : IRequest<int>;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateAccountHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var entity = new Account
        {
            account_name = request.AccountDTO.account_name,
            account_number = request.AccountDTO.account_number,
            parent_account_id = request.AccountDTO.parent_account_id
        };

        _context.Accounts.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.account_id;
    }
}