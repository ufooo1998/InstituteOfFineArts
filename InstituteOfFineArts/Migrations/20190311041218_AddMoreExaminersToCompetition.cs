using Microsoft.EntityFrameworkCore.Migrations;

namespace InstituteOfFineArts.Migrations
{
    public partial class AddMoreExaminersToCompetition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID2",
                table: "Competition",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserID3",
                table: "Competition",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID2",
                table: "Competition");

            migrationBuilder.DropColumn(
                name: "UserID3",
                table: "Competition");
        }
    }
}
