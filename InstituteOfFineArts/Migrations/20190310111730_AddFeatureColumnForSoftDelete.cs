using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InstituteOfFineArts.Migrations
{
    public partial class AddFeatureColumnForSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Post");

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Post",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Post",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "CompetitionPost",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CompetitionPost",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Competition",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Competition",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "CompetitionPost");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CompetitionPost");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "Competition");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Competition");

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Post",
                nullable: false,
                defaultValue: 0);
        }
    }
}
