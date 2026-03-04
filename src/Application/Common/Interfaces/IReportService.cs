public interface IReportService
{
    Task<byte[]> GenerateReceiptPdfAsync(string statusFilter, CancellationToken ct);
}