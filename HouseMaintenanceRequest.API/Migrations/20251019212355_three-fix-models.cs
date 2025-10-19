using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseMaintenanceRequest.API.Migrations
{
    /// <inheritdoc />
    public partial class threefixmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Landlords_LandlordId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MaintenanceCompanies_MaintenanceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LandlordId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MaintenanceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LandlordId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaintenanceCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Tenants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MaintenanceCompanies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Landlords",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ApplicationUserId",
                table: "Tenants",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCompanies_ApplicationUserId",
                table: "MaintenanceCompanies",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Landlords_ApplicationUserId",
                table: "Landlords",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Landlords_AspNetUsers_ApplicationUserId",
                table: "Landlords",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceCompanies_AspNetUsers_ApplicationUserId",
                table: "MaintenanceCompanies",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_ApplicationUserId",
                table: "Tenants",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Landlords_AspNetUsers_ApplicationUserId",
                table: "Landlords");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceCompanies_AspNetUsers_ApplicationUserId",
                table: "MaintenanceCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_ApplicationUserId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_ApplicationUserId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceCompanies_ApplicationUserId",
                table: "MaintenanceCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Landlords_ApplicationUserId",
                table: "Landlords");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MaintenanceCompanies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Landlords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "LandlordId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceCompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LandlordId",
                table: "AspNetUsers",
                column: "LandlordId",
                unique: true,
                filter: "[LandlordId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MaintenanceCompanyId",
                table: "AspNetUsers",
                column: "MaintenanceCompanyId",
                unique: true,
                filter: "[MaintenanceCompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Landlords_LandlordId",
                table: "AspNetUsers",
                column: "LandlordId",
                principalTable: "Landlords",
                principalColumn: "LandlordId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MaintenanceCompanies_MaintenanceCompanyId",
                table: "AspNetUsers",
                column: "MaintenanceCompanyId",
                principalTable: "MaintenanceCompanies",
                principalColumn: "MaintenanceCompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Tenants_TenantId",
                table: "AspNetUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
