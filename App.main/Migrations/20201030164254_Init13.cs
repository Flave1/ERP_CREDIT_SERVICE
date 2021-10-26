using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class Init13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName_",
                table: "credit_offerletter");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "credit_offerletter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "credit_offerletter");

            migrationBuilder.AddColumn<string>(
                name: "FileName_",
                table: "credit_offerletter",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
