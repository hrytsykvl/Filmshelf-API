namespace FilmShelf.BAL.Exceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException() : base("Email address not found") { }
}
