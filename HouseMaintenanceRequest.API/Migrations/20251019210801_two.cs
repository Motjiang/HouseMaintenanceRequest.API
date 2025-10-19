using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseMaintenanceRequest.API.Migrations
{
    /// <inheritdoc />
    public partial class two : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Occupation",
                table: "Tenants",
                newName: "ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "MaintenanceCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Landlords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "MaintenanceCompanies");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Landlords");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Tenants",
                newName: "Occupation");
        }
    }
}
