namespace CustomerApi.Application.Customers.Commands;

public record ImportCustomersCommand(Stream FileStream) : IRequest<int>;

public class ImportCustomersCommandHandler : IRequestHandler<ImportCustomersCommand, int>
{
    private readonly IExcelService _excelService;
    private readonly IApplicationDbContext _context; 

    public ImportCustomersCommandHandler(IExcelService excelService, IApplicationDbContext context)
    {
        _excelService = excelService;
        _context = context;
    }

    public async Task<int> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
    {
        // 1. Use the service to turn the Excel stream into DTOs
        var customerDtos = _excelService.ImportCustomers(request.FileStream);

        if (customerDtos == null || !customerDtos.Any()) return 0;

        // 2. Convert DTOs to Domain Entities
        var entities = customerDtos.Select(dto => new Customer(
            EmailAddress.Create(dto.Email), 
            PhoneNumber.Create(dto.CountryCode, dto.PhoneNumber)) 
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            // ... map other properties
        });

        // 3. Save to PostgreSQL
        _context.Customers.AddRange(entities);
        await _context.SaveChangesAsync(cancellationToken);

        return customerDtos.Count;
    }
}