namespace FilmShelf.BAL.Exceptions;

public class InvalidResetTokenException : Exception
{
    public InvalidResetTokenException(string message) : base(message) { } 
}
