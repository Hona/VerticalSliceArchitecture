using Microsoft.EntityFrameworkCore;

namespace VerticalSliceArchitectureTemplate.Common.EfCore;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.RegisterAllInEfCoreConverters(); // Vogen <--> EF Core converters
    }
}
