using Microsoft.AspNetCore.Mvc;

namespace WebServerApp.Services;

public class ProxyController
{
    private Controller Controller;
    private string Name;
    private IExternalWebService WebService;
    
    public ProxyController(Controller Controller, string Name, IExternalWebService WebService)
    {
        this.Controller = Controller;
        this.Name = Name;
        this.WebService = WebService;
    }

    public string Get(string Action)
    {
        string AbsolutePath = $"{Name}/{Action}{Controller.Request.QueryString}";
        return WebService.Get(AbsolutePath);
    }

    public async Task<string> Post(string Action)
    {
        string AbsolutePath = $"{Name}/{Action}";
        int length = (int)(Controller.Request.ContentLength);
        byte[] buffer = new byte[length];
        await Controller.Request.Body.ReadAsync(buffer, 0, length);
        return WebService.Post(AbsolutePath, buffer);
    }
}