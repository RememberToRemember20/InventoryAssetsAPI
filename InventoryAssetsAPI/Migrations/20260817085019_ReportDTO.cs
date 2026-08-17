using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryAssetsAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReportDTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditReportItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditSessionId = table.Column<int>(type: "int", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<long>(type: "bigint", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomNameAtAudit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNameAtAudit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditReportItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditReportItems_AuditSessions_AuditSessionId",
                        column: x => x.AuditSessionId,
                        principalTable: "AuditSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditSessions_RoomId",
                table: "AuditSessions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditReportItems_AuditSessionId",
                table: "AuditReportItems",
                column: "AuditSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditSessions_Rooms_RoomId",
                table: "AuditSessions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditSessions_Rooms_RoomId",
                table: "AuditSessions");

            migrationBuilder.DropTable(
                name: "AuditReportItems");

            migrationBuilder.DropIndex(
                name: "IX_AuditSessions_RoomId",
                table: "AuditSessions");
        }
    }
}
