using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.EntitiesConfigurations;
public class WatchlistMovieConfiguration : IEntityTypeConfiguration<WatchlistMovie>
{
    public void Configure(EntityTypeBuilder<WatchlistMovie> builder)
    {
        builder.HasKey(wm => new { wm.WatchlistId, wm.MovieId });

        builder.Property(wm => wm.AddedAt)
            .IsRequired();

        builder
            .HasOne(wm => wm.Watchlist)
            .WithMany(w => w.WatchlistMovies)
            .HasForeignKey(wm => wm.WatchlistId);

        builder
            .HasOne(wm => wm.Movie)
            .WithMany(m => m.WatchlistMovies)
            .HasForeignKey(wm => wm.MovieId);
    }
}
