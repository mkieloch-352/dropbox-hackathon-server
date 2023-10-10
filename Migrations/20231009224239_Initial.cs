using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contract");
        }
    }
}
