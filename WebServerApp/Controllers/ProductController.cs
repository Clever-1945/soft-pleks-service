using Microsoft.AspNetCore.Mvc;
using WebServerApp.Services;

namespace WebServerApp.Controllers;

[ApiController] 
[Route("[controller]")]
public class ProductController: Controller
{
    private readonly ProxyController _proxy;
    
    public ProductController(IExternalWebService externalWebService)
    {
        _proxy = new ProxyController(this, "Product", externalWebService);
    }
    
    [HttpGet]
    [Route(nameof(List))]
    public string List()
    {
        return _proxy.Get(nameof(List));
    }

    [HttpGet]
    [Route(nameof(Versions))]
    public string Versions()
    {
        return _proxy.Get(nameof(Versions));
    }

    [HttpPost]
    [Route(nameof(Add))]
    public Task<string> Add()
    {
        return _proxy.Post(nameof(Add));
    }
    
    [HttpPost]
    [Route(nameof(Update))]
    public Task<string> Update()
    {
        return _proxy.Post(nameof(Update));
    }

    [HttpPost]
    [Route(nameof(Delete))]
    public Task<string> Delete()
    {
        return _proxy.Post(nameof(Delete));
    }
}