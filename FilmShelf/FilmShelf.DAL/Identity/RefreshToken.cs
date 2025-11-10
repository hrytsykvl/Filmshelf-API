namespace FilmShelf.DAL.Identity;
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
