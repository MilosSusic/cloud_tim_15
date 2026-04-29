using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSprayScheduleToSpraySchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpraySchedule_Fields_FieldId",
                table: "SpraySchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpraySchedule",
                table: "SpraySchedule");

            migrationBuilder.RenameTable(
                name: "SpraySchedule",
                newName: "SpraySchedules");

            migrationBuilder.RenameIndex(
                name: "IX_SpraySchedule_FieldId",
                table: "SpraySchedules",
                newName: "IX_SpraySchedules_FieldId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SpraySchedules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PesticideType",
                table: "SpraySchedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpraySchedules",
                table: "SpraySchedules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpraySchedules_Fields_FieldId",
                table: "SpraySchedules",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpraySchedules_Fields_FieldId",
                table: "SpraySchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpraySchedules",
                table: "SpraySchedules");

            migrationBuilder.RenameTable(
                name: "SpraySchedules",
                newName: "SpraySchedule");

            migrationBuilder.RenameIndex(
                name: "IX_SpraySchedules_FieldId",
                table: "SpraySchedule",
                newName: "IX_SpraySchedule_FieldId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SpraySchedule",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PesticideType",
                table: "SpraySchedule",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpraySchedule",
                table: "SpraySchedule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpraySchedule_Fields_FieldId",
                table: "SpraySchedule",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
