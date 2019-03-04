using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InstituteOfFineArts.Migrations
{
    public partial class AddColumnSubmitdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Post");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitDate",
                table: "CompetitionPost",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmitDate",
                table: "CompetitionPost");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Post",
                nullable: false,
                defaultValue: 0);
        }
    }
}
