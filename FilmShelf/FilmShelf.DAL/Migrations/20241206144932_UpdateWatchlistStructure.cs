using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWatchlistStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_Movies_MovieId",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_MovieId",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId_MovieId",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Watchlists");

            migrationBuilder.CreateTable(
                name: "WatchlistMovies",
                columns: table => new
                {
                    WatchlistId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchlistMovies", x => new { x.WatchlistId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_WatchlistMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchlistMovies_Watchlists_WatchlistId",
                        column: x => x.WatchlistId,
                        principalTable: "Watchlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistMovies_MovieId",
                table: "WatchlistMovies",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchlistMovies");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Watchlists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_MovieId",
                table: "Watchlists",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId_MovieId",
                table: "Watchlists",
                columns: new[] { "UserId", "MovieId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_Movies_MovieId",
                table: "Watchlists",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
