using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    public partial class ChangeCropNameToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add temporary int column
            migrationBuilder.AddColumn<int>(
                name: "CropNameTemp",
                table: "Crop",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Map existing string values to enum integers
            migrationBuilder.Sql(
                @"UPDATE [Crop]
                  SET [CropNameTemp] = CASE
                    WHEN [CropName] = 'Wheat' THEN 1
                    WHEN [CropName] = 'Rice' THEN 2
                    WHEN [CropName] = 'Barley' THEN 3
                    WHEN [CropName] = 'Oats' THEN 4
                    WHEN [CropName] = 'Corn' THEN 5
                    WHEN [CropName] = 'Sorghum' THEN 6
                    WHEN [CropName] = 'Buckwheat' THEN 7
                    WHEN [CropName] = 'Chia' THEN 8
                    ELSE 0 END;"
            );

            // Drop old string column
            migrationBuilder.DropColumn(
                name: "CropName",
                table: "Crop");

            // Rename temp to final name
            migrationBuilder.RenameColumn(
                name: "CropNameTemp",
                table: "Crop",
                newName: "CropName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add temporary string column
            migrationBuilder.AddColumn<string>(
                name: "CropNameTemp",
                table: "Crop",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Map enum integers back to string values
            migrationBuilder.Sql(
                @"UPDATE [Crop]
                  SET [CropNameTemp] = CASE [CropName]
                    WHEN 0 THEN 'None'
                    WHEN 1 THEN 'Wheat'
                    WHEN 2 THEN 'Rice'
                    WHEN 3 THEN 'Barley'
                    WHEN 4 THEN 'Oats'
                    WHEN 5 THEN 'Corn'
                    WHEN 6 THEN 'Sorghum'
                    WHEN 7 THEN 'Buckwheat'
                    WHEN 8 THEN 'Chia'
                    ELSE 'None' END;"
            );

            // Drop integer column
            migrationBuilder.DropColumn(
                name: "CropName",
                table: "Crop");

            // Rename temp string column back to original
            migrationBuilder.RenameColumn(
                name: "CropNameTemp",
                table: "Crop",
                newName: "CropName");
        }
    }
}
