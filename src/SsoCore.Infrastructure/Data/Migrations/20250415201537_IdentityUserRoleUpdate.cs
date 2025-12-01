using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsoCore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserRoleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUserRoles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUserRoles",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUserRoles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "AspNetUserRoles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "AspNetUserRoles",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ClientId",
                table: "AspNetUserRoles",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId_RoleId_ClientId",
                table: "AspNetUserRoles",
                columns: ["UserId", "RoleId", "ClientId"],
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles",
                column: "ClientId",
                principalTable: "OpenIddictApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId_RoleId_ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AspNetUserRoles");
        }
    }
}
