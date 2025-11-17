using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrivilegeId1",
                table: "RolePrivileges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_PrivilegeId1",
                table: "RolePrivileges",
                column: "PrivilegeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId1",
                table: "RolePrivileges",
                column: "PrivilegeId1",
                principalTable: "Privileges",
                principalColumn: "PrivilegeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId1",
                table: "RolePrivileges");

            migrationBuilder.DropIndex(
                name: "IX_RolePrivileges_PrivilegeId1",
                table: "RolePrivileges");

            migrationBuilder.DropColumn(
                name: "PrivilegeId1",
                table: "RolePrivileges");
        }
    }
}
