using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class flutterrefcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedPayment",
                table: "inf_investorfund_website",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlutterwaveRef",
                table: "inf_investorfund_website",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedPayment",
                table: "inf_investorfund",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlutterwaveRef",
                table: "inf_investorfund",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Scenario",
                table: "ifrs_forecasted_pd",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedPayment",
                table: "credit_loanscheduleperiodic",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlutterwaveRef",
                table: "credit_loanscheduleperiodic",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedPayment",
                table: "inf_investorfund_website");

            migrationBuilder.DropColumn(
                name: "FlutterwaveRef",
                table: "inf_investorfund_website");

            migrationBuilder.DropColumn(
                name: "ConfirmedPayment",
                table: "inf_investorfund");

            migrationBuilder.DropColumn(
                name: "FlutterwaveRef",
                table: "inf_investorfund");

            migrationBuilder.DropColumn(
                name: "ConfirmedPayment",
                table: "credit_loanscheduleperiodic");

            migrationBuilder.DropColumn(
                name: "FlutterwaveRef",
                table: "credit_loanscheduleperiodic");

            migrationBuilder.AlterColumn<decimal>(
                name: "Scenario",
                table: "ifrs_forecasted_pd",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
