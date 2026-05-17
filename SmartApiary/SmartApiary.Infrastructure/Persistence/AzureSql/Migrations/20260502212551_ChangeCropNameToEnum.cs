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

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','FloweringPeriodEnd') IS NULL
    ALTER TABLE dbo.Crop ADD [FloweringPeriodEnd] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','FloweringPeriodStart') IS NULL
    ALTER TABLE dbo.Crop ADD [FloweringPeriodStart] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodEnd') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodEnd];");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Crop','FloweringPeriodStart') IS NOT NULL ALTER TABLE dbo.Crop DROP COLUMN [FloweringPeriodStart];");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','Color') IS NULL
    ALTER TABLE dbo.Crop ADD [Color] nvarchar(max) NOT NULL DEFAULT ''; ");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Crop','Note') IS NULL
    ALTER TABLE dbo.Crop ADD [Note] nvarchar(max) NULL;");
        }
    }
}