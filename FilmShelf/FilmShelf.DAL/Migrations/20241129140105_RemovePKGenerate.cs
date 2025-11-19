using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmShelf.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovePKGenerate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Directors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Genres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Actors SET TempId = Id");
            migrationBuilder.Sql("UPDATE Directors SET TempId = Id");
            migrationBuilder.Sql("UPDATE Genres SET TempId = Id");
            migrationBuilder.Sql("UPDATE Movies SET TempId = Id");

            migrationBuilder.DropForeignKey(name: "FK_MovieActors_Actors_ActorId", table: "MovieActors");
            migrationBuilder.DropForeignKey(name: "FK_MovieActors_Movies_MovieId", table: "MovieActors");
            migrationBuilder.DropForeignKey(name: "FK_MovieGenres_Genres_GenreId", table: "MovieGenres");
            migrationBuilder.DropForeignKey(name: "FK_MovieGenres_Movies_MovieId", table: "MovieGenres");
            migrationBuilder.DropForeignKey(name: "FK_Movies_Directors_DirectorId", table: "Movies");
            migrationBuilder.DropForeignKey(name: "FK_Reviews_Movies_MovieId", table: "Reviews");
            migrationBuilder.DropForeignKey(name: "FK_Watchlists_Movies_MovieId", table: "Watchlists");

            migrationBuilder.DropPrimaryKey(name: "PK_Actors", table: "Actors");
            migrationBuilder.DropPrimaryKey(name: "PK_Directors", table: "Directors");
            migrationBuilder.DropPrimaryKey(name: "PK_Genres", table: "Genres");
            migrationBuilder.DropPrimaryKey(name: "PK_Movies", table: "Movies");

            migrationBuilder.DropColumn(name: "Id", table: "Actors");
            migrationBuilder.DropColumn(name: "Id", table: "Directors");
            migrationBuilder.DropColumn(name: "Id", table: "Genres");
            migrationBuilder.DropColumn(name: "Id", table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Directors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Genres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Actors SET Id = TempId");
            migrationBuilder.Sql("UPDATE Directors SET Id = TempId");
            migrationBuilder.Sql("UPDATE Genres SET Id = TempId");
            migrationBuilder.Sql("UPDATE Movies SET Id = TempId");

            migrationBuilder.DropColumn(name: "TempId", table: "Actors");
            migrationBuilder.DropColumn(name: "TempId", table: "Directors");
            migrationBuilder.DropColumn(name: "TempId", table: "Genres");
            migrationBuilder.DropColumn(name: "TempId", table: "Movies");

            migrationBuilder.AddPrimaryKey(name: "PK_Actors", table: "Actors", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Directors", table: "Directors", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Genres", table: "Genres", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Movies", table: "Movies", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Actors_ActorId",
                table: "MovieActors",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Movies_MovieId",
                table: "MovieActors",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                table: "Movies",
                column: "DirectorId",
                principalTable: "Directors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Movies_MovieId",
                table: "Reviews",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Directors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Genres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Actors SET TempId = Id");
            migrationBuilder.Sql("UPDATE Directors SET TempId = Id");
            migrationBuilder.Sql("UPDATE Genres SET TempId = Id");
            migrationBuilder.Sql("UPDATE Movies SET TempId = Id");

            migrationBuilder.DropForeignKey(name: "FK_MovieActors_Actors_ActorId", table: "MovieActors");
            migrationBuilder.DropForeignKey(name: "FK_MovieActors_Movies_MovieId", table: "MovieActors");
            migrationBuilder.DropForeignKey(name: "FK_MovieGenres_Genres_GenreId", table: "MovieGenres");
            migrationBuilder.DropForeignKey(name: "FK_MovieGenres_Movies_MovieId", table: "MovieGenres");
            migrationBuilder.DropForeignKey(name: "FK_Movies_Directors_DirectorId", table: "Movies");
            migrationBuilder.DropForeignKey(name: "FK_Reviews_Movies_MovieId", table: "Reviews");
            migrationBuilder.DropForeignKey(name: "FK_Watchlists_Movies_MovieId", table: "Watchlists");

            migrationBuilder.DropPrimaryKey(name: "PK_Actors", table: "Actors");
            migrationBuilder.DropPrimaryKey(name: "PK_Directors", table: "Directors");
            migrationBuilder.DropPrimaryKey(name: "PK_Genres", table: "Genres");
            migrationBuilder.DropPrimaryKey(name: "PK_Movies", table: "Movies");

            migrationBuilder.DropColumn(name: "Id", table: "Actors");
            migrationBuilder.DropColumn(name: "Id", table: "Directors");
            migrationBuilder.DropColumn(name: "Id", table: "Genres");
            migrationBuilder.DropColumn(name: "Id", table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Directors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Genres",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.Sql("UPDATE Actors SET Id = TempId");
            migrationBuilder.Sql("UPDATE Directors SET Id = TempId");
            migrationBuilder.Sql("UPDATE Genres SET Id = TempId");
            migrationBuilder.Sql("UPDATE Movies SET Id = TempId");

            migrationBuilder.DropColumn(name: "TempId", table: "Actors");
            migrationBuilder.DropColumn(name: "TempId", table: "Directors");
            migrationBuilder.DropColumn(name: "TempId", table: "Genres");
            migrationBuilder.DropColumn(name: "TempId", table: "Movies");

            migrationBuilder.AddPrimaryKey(name: "PK_Actors", table: "Actors", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Directors", table: "Directors", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Genres", table: "Genres", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Movies", table: "Movies", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Actors_ActorId",
                table: "MovieActors",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Movies_MovieId",
                table: "MovieActors",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Genres_GenreId",
                table: "MovieGenres",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_Movies_MovieId",
                table: "MovieGenres",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                table: "Movies",
                column: "DirectorId",
                principalTable: "Directors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Movies_MovieId",
                table: "Reviews",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
