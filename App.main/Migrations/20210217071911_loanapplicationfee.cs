using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class loanapplicationfee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credit_loanapplication_fee_log",
                columns: table => new
                {
                    LoanApplicationFeeLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    LoanApplicationId = table.Column<int>(nullable: false),
                    ApprovedProductId = table.Column<int>(nullable: false),
                    FeeId = table.Column<int>(nullable: false),
                    FeeTypeId = table.Column<int>(nullable: false),
                    StaffName = table.Column<string>(nullable: true),
                    ApprovedProductAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_loanapplication_fee_log", x => x.LoanApplicationFeeLogId);
                });

            migrationBuilder.CreateTable(
                name: "credit_loanapplicationfee",
                columns: table => new
                {
                    LoanApplicationFeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    FeeId = table.Column<int>(nullable: false),
                    ProductPaymentType = table.Column<int>(nullable: false),
                    ProductFeeType = table.Column<int>(nullable: false),
                    ProductAmount = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    LoanApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_loanapplicationfee", x => x.LoanApplicationFeeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit_loanapplication_fee_log");

            migrationBuilder.DropTable(
                name: "credit_loanapplicationfee");
        }
    }
}
