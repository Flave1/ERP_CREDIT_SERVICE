using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class collateralupload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanApplicationId",
                table: "credit_collateralcustomer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanApplicationId",
                table: "credit_collateralcustomer");
        }
    }
}
