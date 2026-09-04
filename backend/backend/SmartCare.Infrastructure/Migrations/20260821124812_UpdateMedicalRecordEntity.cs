using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedicalRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecordType",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MedicalRecords",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "RecordType",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MedicalRecords");
        }
    }
}
