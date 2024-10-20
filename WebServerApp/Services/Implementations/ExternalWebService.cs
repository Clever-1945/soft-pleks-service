using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace WebServerApp.Services.Implementations;

public class ExternalWebService: IExternalWebService
{
    private readonly ISettingsService _settingsService;
    
    public string ExternalServiceUrl { private set; get; }
    
    public ExternalWebService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        ExternalServiceUrl = _settingsService.GetSettings().ExternalServiceUrl;
    }

    public string Get(string AbsolutePath)
    {
        return Get(AbsolutePath, null);
    }
    
    public string Get(string AbsolutePath, Dictionary<string, string> Parameters)
    {
        List<string> urlParameters = new List<string>();
        if (Parameters != null)
        {
            foreach (var parameter in Parameters)
            {
                var name = HttpUtility.UrlEncode(parameter.Key);
                var value = HttpUtility.UrlEncode(parameter.Value ?? "");
                if (!String.IsNullOrWhiteSpace(name))
                {
                    urlParameters.Add($"{name}={value}");
                }
            }
        }

        string url = urlParameters.Count < 1
            ? $"{ExternalServiceUrl}/{AbsolutePath}"
            : $"{ExternalServiceUrl}/{AbsolutePath}?{String.Join("&", urlParameters)}";

        using (var wc = new WebClient())
        {
            return wc.DownloadString(url);
        }
    }

    public string Post<T>(string AbsolutePath, T data = null) where T : class
    {
        var buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        return Post(AbsolutePath, buffer);
    }

    public string Post(string AbsolutePath, byte[] buffer = null)
    {
        using (var wc = new WebClient())
        {
            string url = $"{ExternalServiceUrl}/{AbsolutePath}";
            
            wc.Headers[HttpRequestHeader.ContentType] = "application/json";
            var result = wc.UploadData(url, "POST", buffer ?? new byte[] { });
            return Encoding.UTF8.GetString(result);
        }
    }
}