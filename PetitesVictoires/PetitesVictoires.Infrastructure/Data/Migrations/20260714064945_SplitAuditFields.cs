using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetitesVictoires.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Likes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Likes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Likes",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
