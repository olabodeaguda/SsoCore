using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsoCore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserRoleUpdateMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "AspNetUserRoles",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles",
                column: "ClientId",
                principalTable: "OpenIddictApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "AspNetUserRoles",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_OpenIddictApplications_ClientId",
                table: "AspNetUserRoles",
                column: "ClientId",
                principalTable: "OpenIddictApplications",
                principalColumn: "Id");
        }
    }
}
