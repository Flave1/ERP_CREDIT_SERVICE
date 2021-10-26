using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class pd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Likelihood",
                table: "ifrs_forecasted_pd",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "ifrs_forecasted_pd",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Scenario",
                table: "ifrs_forecasted_pd",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likelihood",
                table: "ifrs_forecasted_pd");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "ifrs_forecasted_pd");

            migrationBuilder.DropColumn(
                name: "Scenario",
                table: "ifrs_forecasted_pd");
        }
    }
}
