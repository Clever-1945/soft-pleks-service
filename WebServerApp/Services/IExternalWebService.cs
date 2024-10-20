namespace WebServerApp.Services;

public interface IExternalWebService
{
    /// <summary>
    /// Адрес внешнего сервиса
    /// </summary>
    string ExternalServiceUrl { get; }

    /// <summary>
    /// GET запрос на внешний сервис
    /// </summary>
    /// <param name="AbsolutePath"></param>
    /// <returns></returns>
    string Get(string AbsolutePath);

    /// <summary>
    /// GET запрос на внешний сервис
    /// </summary>
    string Get(string AbsolutePath, Dictionary<string, string> Parameters);

    /// <summary>
    /// POST запрос на внешний сервис
    /// </summary>
    /// <param name="AbsolutePath"></param>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    string Post<T>(string AbsolutePath, T data = null) where T : class;

    /// <summary>
    /// POST запрос на внешний сервис
    /// </summary>
    /// <param name="AbsolutePath"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    string Post(string AbsolutePath, byte[] buffer = null);
}