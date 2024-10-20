using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServiceApp.Entities;
using WebServiceApp.Extensions;
using WebServiceApp.Models;
using WebServiceApp.Services;

namespace WebServiceApp.Controllers;

[ApiController] 
[Route("[controller]")]
public class ProductController: Controller
{
    private readonly IEntityContext _entityContext;
    
    public ProductController(IEntityContext entityContext)
    {
        _entityContext = entityContext;
    }

    [HttpGet]
    [Route(nameof(Index))]
    public string Index()
    {
        return DateTime.Now.ToString();
    }

    [HttpGet]
    [Route(nameof(List))]
    public object List(string? Name, int? Start, int? Limit)
    {
        IQueryable<Product> products = _entityContext.Products;
        if (!String.IsNullOrWhiteSpace(Name))
        {
            products = products.Where(x => x.Name.Contains(Name));
        }

        var TotalCount = products.Count();
        if (Start != null)
            products = products.Skip(Start.Value);
        if (Limit != null)
            products = products.Take(Limit.Value);

        return new
        {
            ListProduct = products.ToArray(),
            TotalCount = TotalCount
        };
    }

    [HttpGet]
    [Route(nameof(Versions))]
    public ProductVersion[] Versions(Guid? ProductId)
    {
        return _entityContext.ProductVersions.Where(x => x.ProductId == ProductId.Value).ToArray();
    }
    
    [HttpPost]
    [Route(nameof(Add))]
    public Guid Add([FromBody] ProductModel request)
    {
        using (var t = _entityContext.Database.BeginTransaction())
        {
            var product = new Product()
            {
                Name = request.Name,
                Description = request.Description
            };
            _entityContext.Products.Add(product);
            
            try
            {
                foreach (var version in request.Versions)
                {
                    _entityContext.ProductVersions.Add(new ProductVersion()
                    {
                        Description = version.Description,
                        Height = version.Height.Value,
                        Length = version.Length.Value,
                        ProductId = product.Id,
                        Wdith = version.Wdith.Value,
                        Name = version.Name,
                        CreatingDate = DateTime.Now
                    });
                }
                
                _entityContext.SaveChanges();
                t.Commit();

                return product.Id;
            }
            catch
            {
                t.Rollback();
                throw;
            }
        }
    }
    
    [HttpPost]
    [Route(nameof(Update))]
    public Guid Update([FromBody] ProductModel request)
    {
        using (var t = _entityContext.Database.BeginTransaction())
        {
            try
            {
                var product = _entityContext.Products.FirstOrDefault(x => x.Id == request.Id);
                product.Name = request.Name;
                product.Description = request.Description;
                _entityContext.Products.Update(product);

                ProductVersion[] currentVersions =_entityContext.ProductVersions.Where(x => x.ProductId == product.Id).ToArray();

                var dictionaryCurrentVersions = currentVersions.ToDictionary(x => x.Id, x => x);
                HashSet<Guid> processedVersions = new HashSet<Guid>();
                foreach (var version in request.Versions)
                {
                    var guid = version.Id.ToGuidNull();
                    var currentVersion = guid != null
                        ? dictionaryCurrentVersions.GetValueOrDefault(guid.Value)
                        : null;
                    if (currentVersion == null)
                    {
                        currentVersion = new ProductVersion();
                        currentVersion.CreatingDate = DateTime.Now;
                        _entityContext.ProductVersions.Add(currentVersion);
                    }

                    currentVersion.ProductId = product.Id;
                    currentVersion.Description = version.Description;
                    currentVersion.Name = version.Name;
    
                    currentVersion.Wdith = version.Wdith.Value;
                    currentVersion.Height = version.Height.Value;
                    currentVersion.Length = version.Length.Value;
                    processedVersions.Add(currentVersion.Id);
                }

                foreach (var versions in currentVersions.Where(x => !processedVersions.Contains(x.Id)))
                {
                    _entityContext.ProductVersions.Remove(versions);
                }

                _entityContext.SaveChanges();
                t.Commit();
                return product.Id;
            }
            catch
            {
                t.Rollback();
                throw;
            }
            
        }
    }

    [HttpPost]
    [Route(nameof(Delete))]
    public bool Delete([FromBody] ProductModel request)
    {
        var product = _entityContext.Products.FirstOrDefault(x => x.Id == request.Id);
        _entityContext.Products.Remove(product);
        _entityContext.SaveChanges();
        return true;
    }
}