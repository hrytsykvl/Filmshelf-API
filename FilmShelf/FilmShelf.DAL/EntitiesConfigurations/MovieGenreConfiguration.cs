using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> builder)
    {
        builder
            .HasKey(me => new { me.MovieId, me.GenreId });

        builder
            .HasOne(me => me.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(me => me.MovieId);

        builder
            .HasOne(me => me.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(me => me.GenreId);
    }
}
