using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultTV.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
       table: "Users",
       columns: new[] { "Username", "Email", "PasswordHash", "Role", "CreatedAt" },
       values: new object[] { "admin", "admin@vaulttv.local", "$2a$11$79SPMczqIysh9ig6lOx25.FXnqWqClc9WxlBi3xPouY7YmtPilhou", "Admin", DateTime.UtcNow }
   );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
       table: "Users",
       keyColumn: "Username",
       keyValue: "admin"
   );
        }
    }

}
