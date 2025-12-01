using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SsoCore.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClientUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_ClientUsers_ClientId",
                table: "ClientUsers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUsers_UserId",
                table: "ClientUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUsers_AspNetUsers_UserId",
                table: "ClientUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUsers_OpenIddictApplications_ClientId",
                table: "ClientUsers",
                column: "ClientId",
                principalTable: "OpenIddictApplications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_AspNetUsers_UserId",
                table: "ClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_OpenIddictApplications_ClientId",
                table: "ClientUsers");

            migrationBuilder.DropIndex(
                name: "IX_ClientUsers_ClientId",
                table: "ClientUsers");

            migrationBuilder.DropIndex(
                name: "IX_ClientUsers_UserId",
                table: "ClientUsers");
        }
    }
}
