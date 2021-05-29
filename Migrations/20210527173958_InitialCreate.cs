using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GatewayApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gateways",
                columns: table => new
                {
                    USN = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gateways", x => x.USN);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    UID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gatewayUSN = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Vendor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Online = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.UID);
                    table.ForeignKey(
                        name: "FK_devices_gateways_gatewayUSN",
                        column: x => x.gatewayUSN,
                        principalTable: "gateways",
                        principalColumn: "USN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_gatewayUSN",
                table: "devices",
                column: "gatewayUSN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "gateways");
        }
    }
}
