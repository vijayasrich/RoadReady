using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadReady.Migrations
{
    public partial class AddPasswordResetColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "PasswordReset",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "PasswordReset",
                newName: "reset_token");

            migrationBuilder.RenameColumn(
                name: "is_used",
                table: "PasswordReset",
                newName: "IsUsed");

            migrationBuilder.RenameColumn(
                name: "reset_id",
                table: "PasswordReset",
                newName: "ResetId");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordReset_user_id",
                table: "PasswordReset",
                newName: "IX_PasswordReset_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "reset_token",
                table: "PasswordReset",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "PasswordReset",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reset_token",
                table: "PasswordReset",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PasswordReset",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "PasswordReset",
                newName: "is_used");

            migrationBuilder.RenameColumn(
                name: "ResetId",
                table: "PasswordReset",
                newName: "reset_id");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordReset_UserId",
                table: "PasswordReset",
                newName: "IX_PasswordReset_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "token",
                table: "PasswordReset",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "is_used",
                table: "PasswordReset",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
