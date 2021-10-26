using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class taxsetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxGl",
                table: "inf_product",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "inf_product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxGl",
                table: "inf_product");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "inf_product");
        }
    }
}
