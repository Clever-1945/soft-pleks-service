using Microsoft.EntityFrameworkCore;
using WebServiceApp.Entities;

namespace WebServiceApp.Services.Implementations;

public class EntityContext: DbContext, IEntityContext
{
    private readonly ISettingsService _settingsService;
    
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVersion> ProductVersions { get; set; }
    
    
    
    public EntityContext(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Products = this.Set<Product>();
        ProductVersions = this.Set<ProductVersion>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_settingsService.GetSettings().ConnectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .ToTable(tb => tb.HasTrigger("SomeTrigger"));
        
        modelBuilder.Entity<ProductVersion>()
            .ToTable(tb => tb.HasTrigger("SomeTrigger"));
    }
}