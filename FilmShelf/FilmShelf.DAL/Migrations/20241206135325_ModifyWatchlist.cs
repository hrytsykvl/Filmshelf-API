using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyWatchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId",
                table: "Watchlists");

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId",
                table: "Watchlists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId_MovieId",
                table: "Watchlists",
                columns: new[] { "UserId", "MovieId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId_MovieId",
                table: "Watchlists");

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId",
                table: "Watchlists",
                column: "UserId");
        }
    }
}
