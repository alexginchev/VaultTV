using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultTV.Migrations
{
    /// <inheritdoc />
    public partial class AddActorIsIncomplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIncomplete",
                table: "Actors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIncomplete",
                table: "Actors");
        }
    }
}
