using Microsoft.EntityFrameworkCore;

namespace Server;

public class InMemoryDbContext : DbContext
{
    public DbSet<Models.Movie> Movies { get; set; } = default!;
    public DbSet<Models.Cast> Casts { get; set; } = default!;
    public DbSet<Models.Actor> Actors { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {   
        optionsBuilder.UseInMemoryDatabase(nameof(InMemoryDbContext));
    }
}