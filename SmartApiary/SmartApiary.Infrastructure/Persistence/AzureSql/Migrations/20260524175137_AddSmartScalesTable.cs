using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSmartScalesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmartScales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HardwareId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartScales", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmartScales");
        }
    }
}
