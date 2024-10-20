using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebServiceApp.Entities;

namespace WebServiceApp.Services;

public interface IEntityContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductVersion> ProductVersions { get; }
    
    DatabaseFacade Database { get; }

    int SaveChanges();

    EntityEntry Entry(object entity);

    EntityEntry Remove(object entity);
}