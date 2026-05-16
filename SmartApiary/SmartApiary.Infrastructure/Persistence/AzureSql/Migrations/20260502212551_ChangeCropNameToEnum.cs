using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCropNameToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Color') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Color];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Note') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Note];");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Crop",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FloweringPeriodEnd",
                table: "Crop",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FloweringPeriodStart",
                table: "Crop",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FloweringPeriodEnd",
                table: "Crop");

            migrationBuilder.DropColumn(
                name: "FloweringPeriodStart",
                table: "Crop");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
