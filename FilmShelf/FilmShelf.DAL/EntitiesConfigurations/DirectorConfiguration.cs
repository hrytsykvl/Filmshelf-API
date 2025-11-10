using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.BirthDate)
            .IsRequired();

        builder.Property(d => d.Bio)
            .HasMaxLength(500);
    }
}
