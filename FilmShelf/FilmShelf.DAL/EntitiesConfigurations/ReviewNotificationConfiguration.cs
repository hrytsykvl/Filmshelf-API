using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmShelf.DAL.EntitiesConfigurations;

public class ReviewNotificationConfiguration : IEntityTypeConfiguration<ReviewNotification>
{
    public void Configure(EntityTypeBuilder<ReviewNotification> builder)
    {
        builder
            .HasOne(un => un.ReviewResponse)
            .WithMany(r => r.Notifications)
            .HasForeignKey(un => un.ReviewResponseId);
    }
}
