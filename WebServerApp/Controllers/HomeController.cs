using Microsoft.AspNetCore.Mvc;
using WebServerApp.Services;

namespace WebServerApp.Controllers;

public class HomeController: Controller
{
    private readonly IExternalWebService _webService;
    
    public HomeController(IExternalWebService webService)
    {
        _webService = webService;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public string Get()
    {
        return _webService.Get("Product/List");
    }
}