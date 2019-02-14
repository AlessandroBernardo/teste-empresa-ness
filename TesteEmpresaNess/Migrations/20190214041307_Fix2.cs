using Microsoft.EntityFrameworkCore.Migrations;

namespace TesteEmpresaNess.Migrations
{
    public partial class Fix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Longitude",
                table: "Users",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 8)");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "Users",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Users",
                type: "decimal(11, 8)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Users",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
