using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GDSwithREST.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateStores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AuthorityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateStores", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ApplicationType = table.Column<int>(type: "int", nullable: false),
                    ProductUri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ServerCapabilities = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Certificate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    HttpsCertificate = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TrustListId = table.Column<int>(type: "int", nullable: true),
                    HttpsTrustListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Applications_HttpsTrustListId",
                        column: x => x.HttpsTrustListId,
                        principalTable: "CertificateStores",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Applications_TrustListId",
                        column: x => x.TrustListId,
                        principalTable: "CertificateStores",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationNames",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationNames", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ApplicationNames_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CertificateGroupId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateTypeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateSigningRequest = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SubjectName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DomainNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrivateKeyFormat = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    PrivateKeyPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AuthorityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CertificateRequests_Applications",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerEndpoints",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DiscoveryUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerEndpoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServerEndpoints_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_ApplicationNames_ApplicationId",
                table: "ApplicationNames",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Applications_HttpsTrustListId",
                table: "Applications",
                column: "HttpsTrustListId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Applications_TrustListId",
                table: "Applications",
                column: "TrustListId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_CertificateRequests_Applications",
                table: "CertificateRequests",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_ServerEndpoints_ApplicationId",
                table: "ServerEndpoints",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationNames");

            migrationBuilder.DropTable(
                name: "CertificateRequests");

            migrationBuilder.DropTable(
                name: "ServerEndpoints");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "CertificateStores");
        }
    }
}
