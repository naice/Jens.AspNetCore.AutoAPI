using Jens.AspNetCore.AutoAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Server;

public class InMemoryDbContext : DbContext
{
    private readonly string _contextName;

    public DbSet<Models.Movie> Movies { get; set; } = default!;
    public DbSet<Models.Cast> Casts { get; set; } = default!;
    public DbSet<Models.Actor> Actors { get; set; } = default!;

    public InMemoryDbContext() : this(nameof(InMemoryDbContext))
    {
        
    }

    public InMemoryDbContext(string contextName)
    {
        _contextName = contextName;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {   
        optionsBuilder.UseInMemoryDatabase(_contextName);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Models.Cast>().ConfigureKeys();
    }
}
