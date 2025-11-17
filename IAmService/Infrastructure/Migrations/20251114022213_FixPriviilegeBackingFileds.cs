using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPriviilegeBackingFileds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId1",
                table: "RolePrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivileges_Users_UserId",
                table: "UserPrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_RolePrivileges_PrivilegeId1",
                table: "RolePrivileges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PrivilegeId1",
                table: "RolePrivileges");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_User_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivileges_User_UserId",
                table: "UserPrivileges",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_User_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPrivileges_User_UserId",
                table: "UserPrivileges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "PrivilegeId1",
                table: "RolePrivileges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePrivileges_PrivilegeId1",
                table: "RolePrivileges",
                column: "PrivilegeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePrivileges_Privileges_PrivilegeId1",
                table: "RolePrivileges",
                column: "PrivilegeId1",
                principalTable: "Privileges",
                principalColumn: "PrivilegeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrivileges_Users_UserId",
                table: "UserPrivileges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
