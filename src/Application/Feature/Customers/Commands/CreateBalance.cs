public record CreateBalanceCommand(BalanceCreateDTO BalanceDTO) : IRequest<int>;

public class CreateBalanceHandler : IRequestHandler<CreateBalanceCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateBalanceHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateBalanceCommand request, CancellationToken ct)
    {
        var entity = new Balance
        {
            report_id = request.BalanceDTO.report_id,
            account_id = request.BalanceDTO.account_id,
            report_year = request.BalanceDTO.report_year,
            amount = request.BalanceDTO.amount
        };

        _context.Balances.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.balance_id;
    }
}