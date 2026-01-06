namespace FilmShelf.DAL.Entities;

public class ReviewNotification : Notification
{
    public int ReviewResponseId { get; set; }

    public ReviewResponse ReviewResponse { get; set; } = null!;
}
