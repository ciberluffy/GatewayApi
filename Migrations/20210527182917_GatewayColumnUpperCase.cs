using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayApi.Migrations
{
    public partial class GatewayColumnUpperCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_devices_gateways_gatewayUSN",
                table: "devices");

            migrationBuilder.RenameColumn(
                name: "gatewayUSN",
                table: "devices",
                newName: "GatewayUSN");

            migrationBuilder.RenameIndex(
                name: "IX_devices_gatewayUSN",
                table: "devices",
                newName: "IX_devices_GatewayUSN");

            migrationBuilder.AddForeignKey(
                name: "FK_devices_gateways_GatewayUSN",
                table: "devices",
                column: "GatewayUSN",
                principalTable: "gateways",
                principalColumn: "USN",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_devices_gateways_GatewayUSN",
                table: "devices");

            migrationBuilder.RenameColumn(
                name: "GatewayUSN",
                table: "devices",
                newName: "gatewayUSN");

            migrationBuilder.RenameIndex(
                name: "IX_devices_GatewayUSN",
                table: "devices",
                newName: "IX_devices_gatewayUSN");

            migrationBuilder.AddForeignKey(
                name: "FK_devices_gateways_gatewayUSN",
                table: "devices",
                column: "gatewayUSN",
                principalTable: "gateways",
                principalColumn: "USN",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
