using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSprayLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID(N'dbo.FieldLogEntry', N'U') IS NOT NULL DROP TABLE dbo.FieldLogEntry;");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewType",
                table: "HiveLogEntries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "HiveLogEntries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BottomBoardColor",
                table: "HiveLogEntries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CropName",
                table: "Crop",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.SprayLogs', N'U') IS NULL
BEGIN
    CREATE TABLE [SprayLogs] (
        [Id] uniqueidentifier NOT NULL,
        [SprayScheduleId] uniqueidentifier NOT NULL,
        [FieldId] uniqueidentifier NOT NULL,
        [FieldName] nvarchar(100) NOT NULL,
        [CropName] nvarchar(100) NULL,
        [StartDateTime] datetime2 NOT NULL,
        [EndDateTime] datetime2 NOT NULL,
        [PesticideType] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [WindSpeed] float NULL,
        [Temperature] float NULL,
        [WeatherDescription] nvarchar(256) NULL,
        CONSTRAINT [PK_SprayLogs] PRIMARY KEY ([Id])
    );
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID(N'dbo.SprayLogs', N'U') IS NOT NULL DROP TABLE dbo.SprayLogs;");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewType",
                table: "HiveLogEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "HiveLogEntries",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BottomBoardColor",
                table: "HiveLogEntries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CropName",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Recreating FieldLogEntry is omitted to avoid errors if the table structure has changed or no longer needed.
        }
    }
}
