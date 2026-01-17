using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class MoviePageConfiguration : IEntityTypeConfiguration<MoviePage>
{
    public void Configure(EntityTypeBuilder<MoviePage> builder)
    {
        builder.Property(p => p.PageNumber)
            .IsRequired();

        builder.Property(p => p.MoviesJson)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasDefaultValue(MoviePageType.Regular);
    }
}
