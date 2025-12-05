using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class UserWatchlistConfiguration : IEntityTypeConfiguration<UserWatchlist>
{
    public void Configure(EntityTypeBuilder<UserWatchlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasOne(w => w.User)
            .WithMany(u => u.Watchlists)
            .HasForeignKey(w => w.UserId);
    }
}
