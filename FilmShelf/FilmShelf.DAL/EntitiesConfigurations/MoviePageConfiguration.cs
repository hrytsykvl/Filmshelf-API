using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class MoviePageConfiguration : IEntityTypeConfiguration<MoviePage>
{
    public void Configure(EntityTypeBuilder<MoviePage> builder)
    {
        builder.Property(p => p.PageNumber)
            .IsRequired();

        builder.HasIndex(p => p.PageNumber).IsUnique();

        builder.Property(p => p.MoviesJson)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();
    }
}
