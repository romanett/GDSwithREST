using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GDSwithREST.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class constraintCertificateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrustLists_CertificateType",
                table: "TrustLists",
                column: "CertificateType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrustLists_CertificateType",
                table: "TrustLists");
        }
    }
}
