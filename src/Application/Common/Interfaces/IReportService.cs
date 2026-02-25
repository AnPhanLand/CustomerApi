public interface IReportService
{
    Task<byte[]> GenerateReceiptPdfAsync(CancellationToken ct);
}