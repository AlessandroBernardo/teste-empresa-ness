using Microsoft.EntityFrameworkCore.Migrations;

namespace TesteEmpresaNess.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Users",
                type: "decimal(11, 8)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Users",
                type: "decimal(10, 8)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Users",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11, 8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Users",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10, 8)");
        }
    }
}
