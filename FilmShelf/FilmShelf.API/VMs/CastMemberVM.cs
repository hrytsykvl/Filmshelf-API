namespace FilmShelf.API.VMs;

public class CastMemberVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Character { get; set; } = null!;
    public string ProfilePath { get; set; } = null!;
}
