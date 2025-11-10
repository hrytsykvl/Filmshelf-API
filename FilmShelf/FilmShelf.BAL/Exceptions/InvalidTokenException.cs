namespace FilmShelf.BAL.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("Invalid JWT token") { }
}
