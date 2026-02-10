public record CreatePaymentCommand(PaymentCreateDTO PaymentDTO) : IRequest<Guid>;

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreatePaymentHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var entity = new Payment
        {
            Description = request.PaymentDTO.Description,
            Price = request.PaymentDTO.Price,
            InterestRate = request.PaymentDTO.InterestRate,
            StudentId = request.PaymentDTO.StudentId
        };

        _context.Payments.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}