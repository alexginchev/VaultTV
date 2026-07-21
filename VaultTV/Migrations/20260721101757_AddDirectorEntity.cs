using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultTV.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Director",
                table: "Media");

            migrationBuilder.AddColumn<int>(
                name: "DirectorId",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Born = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_DirectorId",
                table: "Media",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_Name",
                table: "Directors",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Directors_DirectorId",
                table: "Media",
                column: "DirectorId",
                principalTable: "Directors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Directors_DirectorId",
                table: "Media");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropIndex(
                name: "IX_Media_DirectorId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "DirectorId",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
