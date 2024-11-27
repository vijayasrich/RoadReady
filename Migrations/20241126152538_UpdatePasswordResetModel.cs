using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadReady.Migrations
{
    public partial class UpdatePasswordResetModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__PasswordR__user___6754599E",
                table: "PasswordResets");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Password__40FB0520CEEC07F9",
                table: "PasswordResets");

            migrationBuilder.DropColumn(
                name: "request_time",
                table: "PasswordResets");

            migrationBuilder.DropColumn(
                name: "reset_time",
                table: "PasswordResets");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                newName: "PasswordReset");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResets_user_id",
                table: "PasswordReset",
                newName: "IX_PasswordReset_user_id");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "PasswordReset",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "PasswordReset",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "PasswordReset",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_used",
                table: "PasswordReset",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordReset",
                table: "PasswordReset",
                column: "reset_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordReset_User",
                table: "PasswordReset",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordReset_User",
                table: "PasswordReset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordReset",
                table: "PasswordReset");

            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "PasswordReset");

            migrationBuilder.DropColumn(
                name: "is_used",
                table: "PasswordReset");

            migrationBuilder.RenameTable(
                name: "PasswordReset",
                newName: "PasswordResets");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordReset_user_id",
                table: "PasswordResets",
                newName: "IX_PasswordResets_user_id");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "PasswordResets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "PasswordResets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "request_time",
                table: "PasswordResets",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(sysdatetime())");

            migrationBuilder.AddColumn<DateTime>(
                name: "reset_time",
                table: "PasswordResets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Password__40FB0520CEEC07F9",
                table: "PasswordResets",
                column: "reset_id");

            migrationBuilder.AddForeignKey(
                name: "FK__PasswordR__user___6754599E",
                table: "PasswordResets",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
