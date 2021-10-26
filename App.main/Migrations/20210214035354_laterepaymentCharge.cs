using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class laterepaymentCharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LateRepaymentCharge",
                table: "credit_loan_past_due",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "LateRepaymentCharge",
                table: "credit_loan",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LateRepaymentCharge",
                table: "credit_loan_past_due");

            migrationBuilder.DropColumn(
                name: "LateRepaymentCharge",
                table: "credit_loan");
        }
    }
}
