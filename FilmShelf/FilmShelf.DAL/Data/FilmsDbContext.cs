using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Data;

public class FilmsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public FilmsDbContext(DbContextOptions<FilmsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilmsDbContext).Assembly);
    }
}
