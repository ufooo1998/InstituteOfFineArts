using Microsoft.EntityFrameworkCore.Migrations;

namespace InstituteOfFineArts.Migrations
{
    public partial class addFeatureTableCompetitionPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffSubmit",
                table: "CompetitionPost",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentSubmit",
                table: "CompetitionPost",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffSubmit",
                table: "CompetitionPost");

            migrationBuilder.DropColumn(
                name: "StudentSubmit",
                table: "CompetitionPost");
        }
    }
}
