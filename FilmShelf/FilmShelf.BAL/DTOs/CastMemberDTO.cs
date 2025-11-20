namespace FilmShelf.BAL.DTOs;

public class CastMemberDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Character { get; set; } = null!;
    public string ProfilePath { get; set; } = null!;
}
