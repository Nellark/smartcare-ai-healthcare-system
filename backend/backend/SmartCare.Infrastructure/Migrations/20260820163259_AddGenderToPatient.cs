using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patients");
        }
    }
}
