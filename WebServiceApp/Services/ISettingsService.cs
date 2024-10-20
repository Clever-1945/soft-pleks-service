using WebServiceApp.Definitions;

namespace WebServiceApp.Services;

/// <summary>
/// Сервис настройки приложения
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Получить настройки приложения
    /// </summary>
    /// <returns></returns>
    SettingsApp GetSettings();
}