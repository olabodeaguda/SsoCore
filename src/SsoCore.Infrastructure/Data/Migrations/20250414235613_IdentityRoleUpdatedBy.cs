using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsoCore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityRoleUpdatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetRoles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "AspNetRoles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "AspNetRoles",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "AspNetRoles");
        }
    }
}
