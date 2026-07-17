using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultTV.Migrations
{
    /// <inheritdoc />
    public partial class AddActorTopRank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopRank",
                table: "Actors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopRank",
                table: "Actors");
        }
    }
}
