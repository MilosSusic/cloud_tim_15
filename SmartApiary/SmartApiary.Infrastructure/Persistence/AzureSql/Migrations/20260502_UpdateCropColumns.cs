using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    public partial class UpdateCropColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','FloweringPeriodStart') IS NULL
    ALTER TABLE dbo.Crop ADD [FloweringPeriodStart] datetime2 NOT NULL DEFAULT '2000-01-01T00:00:00.0000000';");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','FloweringPeriodEnd') IS NULL
    ALTER TABLE dbo.Crop ADD [FloweringPeriodEnd] datetime2 NOT NULL DEFAULT '2000-01-02T00:00:00.0000000';");

            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Color') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Color];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','Note') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [Note];");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','Color') IS NULL
    ALTER TABLE dbo.Crop ADD [Color] nvarchar(max) NOT NULL DEFAULT ''; ");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','Note') IS NULL
    ALTER TABLE dbo.Crop ADD [Note] nvarchar(max) NULL;");

            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodStart') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodStart];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodEnd') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodEnd];");
        }
    }
}