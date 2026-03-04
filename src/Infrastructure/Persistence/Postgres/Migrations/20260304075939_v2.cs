using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BienLais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    HoTen = table.Column<string>(type: "text", nullable: false),
                    TonAn = table.Column<int>(type: "integer", nullable: false),
                    TienAn = table.Column<int>(type: "integer", nullable: false),
                    ChamSocBanTru = table.Column<int>(type: "integer", nullable: false),
                    Dien = table.Column<int>(type: "integer", nullable: false),
                    NuocUongTK = table.Column<int>(type: "integer", nullable: false),
                    HocHe = table.Column<int>(type: "integer", nullable: false),
                    TrangBiPVBanTru = table.Column<int>(type: "integer", nullable: false),
                    BaoHiemTT = table.Column<int>(type: "integer", nullable: false),
                    NangKhieu = table.Column<int>(type: "integer", nullable: false),
                    Mua = table.Column<int>(type: "integer", nullable: false),
                    Ve = table.Column<int>(type: "integer", nullable: false),
                    TiengAnh = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienLais", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BienLais");
        }
    }
}
