using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    public partial class UpdateCropColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add flowering period columns with a safe default for existing rows
            migrationBuilder.AddColumn<DateTime>(
                name: "FloweringPeriodStart",
                table: "Crop",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "FloweringPeriodEnd",
                table: "Crop",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 2));

            // Drop removed columns if they exist
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Color') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Color];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Note') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Note];");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate dropped columns
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

            // Remove flowering period columns if they exist
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodStart') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodStart];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodEnd') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodEnd];");
        }
    }
}
