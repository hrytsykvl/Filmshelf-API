namespace FilmShelf.BAL.Exceptions;

public class InvalidLoginException : Exception
{
    public InvalidLoginException() : base("Invalid email or password") { }
}
