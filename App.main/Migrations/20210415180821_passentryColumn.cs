using Microsoft.EntityFrameworkCore.Migrations;

namespace Banking.Migrations
{
    public partial class passentryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PassEntry",
                table: "inf_investorfund_website",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassEntry",
                table: "inf_investorfund_website");
        }
    }
}
