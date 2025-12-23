using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class ReviewResponseConfiguration : IEntityTypeConfiguration<ReviewResponse>
{
    public void Configure(EntityTypeBuilder<ReviewResponse> builder)
    {
        builder.Property(r => r.Content)
            .HasMaxLength(1000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder
            .HasOne(r => r.User)
            .WithMany(r => r.ReviewResponses)
            .HasForeignKey(r => r.UserId);

        builder
            .HasOne(r => r.Review)
            .WithMany(r => r.ReviewResponses)
            .HasForeignKey(r => r.ReviewId);
    }
}
