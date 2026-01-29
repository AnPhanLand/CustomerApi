namespace CustomerApi.Application.Customers.Commands;

public record ImportCSVCommand(Stream FileStream) : IRequest<int>;

public class ImportCSVCommandHandler : IRequestHandler<ImportCSVCommand, int>
{
    private readonly ICsvService _csvService;
    private readonly IApplicationDbContext _context; 

    public ImportCSVCommandHandler(ICsvService csvService, IApplicationDbContext context)
    {
        _csvService = csvService;
        _context = context;
    }

    public async Task<int> Handle(ImportCSVCommand request, CancellationToken cancellationToken)
    {
        // 1. Use CsvHelper (via our service) to parse the file into DTOs
        var customerDtos = _csvService.ImportFromCsv<CustomerCreateDTO>(request.FileStream).ToList();

        if (!customerDtos.Any()) return 0;

        // 2. Map DTOs to Domain Entities (handling Enums manually)
        var entities = customerDtos.Select(dto => new Customer(
            EmailAddress.Create(dto.Email), 
            PhoneNumber.Create(dto.CountryCode, dto.PhoneNumber)) 
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        });

        // 3. Save to the Database
        _context.Customers.AddRange(entities);
        await _context.SaveChangesAsync(cancellationToken);

        return customerDtos.Count;
    }
}