using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartApiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RecreateCropTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF OBJECT_ID(N'dbo.Crop', N'U') IS NOT NULL
            DROP TABLE dbo.Crop;

        CREATE TABLE dbo.Crop
        (
            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            FieldId UNIQUEIDENTIFIER NOT NULL,
            CropName INT NOT NULL,
            FloweringPeriodStart DATETIME2 NOT NULL,
            FloweringPeriodEnd DATETIME2 NOT NULL,
            Description NVARCHAR(512) NULL,
            IsDeleted BIT NOT NULL
        );

        ALTER TABLE dbo.Crop
            ADD CONSTRAINT FK_Crop_Fields_FieldId FOREIGN KEY (FieldId)
            REFERENCES dbo.Fields (Id) ON DELETE CASCADE;

        CREATE INDEX IX_Crop_FieldId ON dbo.Crop (FieldId);
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF OBJECT_ID(N'dbo.Crop', N'U') IS NOT NULL DROP TABLE dbo.Crop;");
        }
    }
}
