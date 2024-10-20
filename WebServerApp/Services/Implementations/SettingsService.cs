using System.Text.Json;
using WebServerApp.Definitions;

namespace WebServerApp.Services.Implementations;

public class SettingsService: ISettingsService
{
    public string ApplicationPath { private set; get; }
    
    private SettingsApp _settings = null;
    
    public SettingsService()
    {
        var location = Path.GetDirectoryName(this.GetType().Assembly.Location);
        #if DEBUG
        ApplicationPath = new DirectoryInfo(Path.Combine(location, "..", "..", "..")).FullName;
        #else
        ApplicationPath = location;
        #endif
    }
    
    /// <summary>
    /// Получить настройки приложения
    /// </summary>
    /// <returns></returns>
    public SettingsApp GetSettings() => _settings ?? (_settings = GetSettingsPrivate());
    
    private SettingsApp GetSettingsPrivate()
    {
        string fileName = "settings.json";
        string content = File.ReadAllText(Path.Combine(ApplicationPath, fileName));
        return JsonSerializer.Deserialize<SettingsApp>(content);
    }
}