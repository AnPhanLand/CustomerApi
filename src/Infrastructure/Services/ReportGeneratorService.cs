using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;

namespace CustomerApi.Infrastructure.Services;

public class ReportGeneratorService : IReportService
{
        private const string GenerateReportPath = "C:\\Users\\phant\\Documents\\Intern\\BackEndIntership\\CustomerApi\\src\\API\\newReport.repx";
         private const string ViewReportPath = "C:\\Users\\phant\\Documents\\Intern\\BackEndIntership\\CustomerApi\\src\\API\\ViewReport.repx";
        // var report = XtraReport.FromFile(reportPath, true);

        private const string ConnectionString = "XpoProvider=Postgres;Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword";

    
    public async Task<byte[]> GenerateReceiptPdfAsync(CancellationToken ct)
    {
        var report = XtraReport.FromFile(ViewReportPath, true);
        var sqlDataSource = CreateDataSource();

        report.DataSource = sqlDataSource;
        report.DataMember = "PhieuThus";

        using var ms = new MemoryStream();
        await report.ExportToPdfAsync(ms, null, ct);
        return ms.ToArray();
    }

    // --- NEW METHOD TO UPDATE THE SCHEMA ---
    public void UpdateReportSchemaWithSql()
    {
        // 1. Start with a fresh report or load existing
        XtraReport report = new XtraReport(); 
        
        // 2. Create the data source
        var sqlDataSource = CreateDataSource();

        // 3. IMPORTANT: Tell the report to store this data source inside the XML
        report.DataSource = sqlDataSource;
        report.ComponentStorage.Add(sqlDataSource); 
        report.DataMember = "";

        // 4. Save back to the .repx file
        report.SaveLayoutToXml(GenerateReportPath);
        
        Console.WriteLine("Successfully updated .repx schema with SQL connection and queries.");
    }

    // Helper to avoid repeating the SQL logic
    private SqlDataSource CreateDataSource()
    {
        // Use "User ID" instead of "Username" for DevExpress Postgres provider
        var connParams = new CustomStringConnectionParameters(ConnectionString);
        var sqlDataSource = new SqlDataSource(connParams);

        var query = new CustomSqlQuery("PhieuThus", 
            @"SELECT pt.*, 
                s.""LastName"" || ' ' || s.""FirstName"" AS ""StudentName"",
                s.""ClassName"",
                p.""LastName"" || ' ' || p.""FirstName"" AS ""ParentName"",
                p.""PhoneNumber"" AS ""ParentPhone""
              FROM ""PhieuThus"" pt 
              INNER JOIN ""PhuHuynhs"" p ON pt.""PhuHuynhId"" = p.""Id""
              INNER JOIN ""Students"" s ON pt.""StudentId"" = s.""Id""");

        var query2 = new CustomSqlQuery("Payments", 
            @"SELECT p.*,
                s.""LastName"" || ' ' || s.""FirstName"" AS ""StudentName"",
                s.""ClassName""
                FROM ""Payments"" p 
              INNER JOIN ""Students"" s ON p.""StudentId"" = s.""Id""");

        var query3 = new CustomSqlQuery("Students", 
            @"SELECT s.* FROM ""Students"" s");
        
        var query4 = new CustomSqlQuery("PhuHuynhs", 
            @"SELECT p.* FROM ""PhuHuynhs"" p");

        sqlDataSource.Queries.Add(query);
        sqlDataSource.Queries.Add(query2);
        sqlDataSource.Queries.Add(query3);
        sqlDataSource.Queries.Add(query4);

        var relation = new MasterDetailInfo("PhieuThus", "Payments", "StudentId", "StudentId");
        var relation2 = new MasterDetailInfo("PhieuThus", "Payments", "Id", "PhieuThuId");
        var relation3 = new MasterDetailInfo("PhuHuynhs", "Students", "Id", "PhuHuynhId");

        sqlDataSource.Relations.Add(relation);
        sqlDataSource.Relations.Add(relation2);
        sqlDataSource.Relations.Add(relation3);

        // This fetches the column metadata
        sqlDataSource.RebuildResultSchema();

        return sqlDataSource;
    }
}