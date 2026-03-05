public record CreateReportCommand(ReportCreateDTO ReportDTO) : IRequest<int>;

public class CreateReportHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IApplicationDbContext _context;
    public CreateReportHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateReportCommand request, CancellationToken ct)
    {
        var entity = new Report
        {
            report_name = request.ReportDTO.report_name,
            currency = request.ReportDTO.currency,
            unit = request.ReportDTO.unit
        };

        _context.Reports.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.report_id;
    }
}