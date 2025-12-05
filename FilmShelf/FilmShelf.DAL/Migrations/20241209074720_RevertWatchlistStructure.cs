using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RevertWatchlistStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchlistMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Watchlists",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId",
                table: "Watchlists");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Watchlists",
                newName: "MovieId");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Watchlists");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Watchlists",
                type: "int",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Watchlists",
                table: "Watchlists",
                columns: new[] { "UserId", "MovieId" });

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_MovieId",
                table: "Watchlists",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_Movies_MovieId",
                table: "Watchlists",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_Movies_MovieId",
                table: "Watchlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Watchlists",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_MovieId",
                table: "Watchlists");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "Watchlists",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Watchlists",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Watchlists",
                table: "Watchlists",
                column: "Id");

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
                name: "IX_Watchlists_UserId",
                table: "Watchlists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistMovies_MovieId",
                table: "WatchlistMovies",
                column: "MovieId");
        }
    }
}
