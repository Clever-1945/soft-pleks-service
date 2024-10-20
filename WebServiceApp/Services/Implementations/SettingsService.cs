using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebServiceApp.Definitions;

namespace WebServiceApp.Services.Implementations;

/// <summary>
/// Сервис настройки приложения
/// </summary>
public class SettingsService: ISettingsService
{
    public string ApplicationFileName { private set; get; }
    public string ApplicationPath { private set; get; }

    private SettingsApp _settings = null;

    public SettingsService()
    {
        ApplicationFileName = this.GetType().Assembly.Location;
        ApplicationPath = Path.GetDirectoryName(ApplicationFileName);
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

