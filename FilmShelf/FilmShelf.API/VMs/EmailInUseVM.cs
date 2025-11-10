namespace FilmShelf.API.VMs;

public class EmailInUseVM
{
    public string Email { get; set; } = null!;
    public bool IsInUse { get; set; }
}
