using FilmShelf.DAL.Data;
using FilmShelf.DAL.Identity;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly FilmsDbContext _context;

    public RefreshTokenRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(
        RefreshToken refreshToken,
        string newToken,
        DateTime newExpirationDate)
    {
        refreshToken.Token = newToken;
        refreshToken.ExpirationDate = newExpirationDate;
        await _context.SaveChangesAsync();
    }
}
