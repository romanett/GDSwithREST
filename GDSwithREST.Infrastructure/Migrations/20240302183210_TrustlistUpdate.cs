using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GDSwithREST.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TrustlistUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_HttpsTrustListId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_TrustListId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_CertificateRequests_Applications",
                table: "CertificateRequests");

            migrationBuilder.DropTable(
                name: "CertificateStores");

            migrationBuilder.DropIndex(
                name: "IX_FK_Applications_HttpsTrustListId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_FK_Applications_TrustListId",
                table: "Applications");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "CertificateRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TrustListId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HttpsTrustListId",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateRequests_Applications",
                table: "CertificateRequests",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CertificateRequests_Applications",
                table: "CertificateRequests");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "CertificateRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrustListId",
                table: "Applications",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HttpsTrustListId",
                table: "Applications",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CertificateStores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateStores", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_Applications_HttpsTrustListId",
                table: "Applications",
                column: "HttpsTrustListId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Applications_TrustListId",
                table: "Applications",
                column: "TrustListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_HttpsTrustListId",
                table: "Applications",
                column: "HttpsTrustListId",
                principalTable: "CertificateStores",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_TrustListId",
                table: "Applications",
                column: "TrustListId",
                principalTable: "CertificateStores",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateRequests_Applications",
                table: "CertificateRequests",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
