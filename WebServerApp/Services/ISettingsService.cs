using WebServerApp.Definitions;

namespace WebServerApp.Services;

public interface ISettingsService
{
    SettingsApp GetSettings();
}