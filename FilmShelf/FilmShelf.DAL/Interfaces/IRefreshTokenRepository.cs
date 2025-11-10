using FilmShelf.DAL.Identity;

namespace FilmShelf.DAL.Interfaces;
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task RemoveRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, string newToken, DateTime newExpirationDate);
}
