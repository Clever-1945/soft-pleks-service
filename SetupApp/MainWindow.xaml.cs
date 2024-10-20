using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using Path = System.IO.Path;

namespace SetupApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        TextConnectionString.Text = "Data Source=DESKTOP-35GV7B5\\SQLEXPRESS;Database=TestDb;User Id=sa;Password=sql-555900;TrustServerCertificate=True;Encrypt=False;MultiSubnetFailover=False";
        TextPathServer.Text = "D:\\test\\server\\";
        TextPortServer.Text = "5000";
        
        TextPathService.Text = "D:\\test\\service\\";
        TextPortService.Text = "4000";

        TryDropTable("EventLog");
        TryDropTable("ProductVersion");
        TryDropTable("Product");
        
        if (Directory.Exists(TextPathServer.Text))
            Directory.Delete(TextPathServer.Text, true);
        
        if (Directory.Exists(TextPathService.Text))
            Directory.Delete(TextPathService.Text, true);
#endif
    }

    private void TryDropTable(string table)
    {
        try
        {
            SqlConnection connection = new SqlConnection(TextConnectionString.Text);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = $"DROP TABLE {table}";
            command.Connection = connection;
            command.ExecuteNonQuery();
        }
        catch
        {
            
        }
    }

    private void OnCheckConnect(object sender, RoutedEventArgs e)
    {
        try
        {
            SqlConnection connection = new SqlConnection(TextConnectionString.Text);
            connection.Open();
            TextLog.Text = "success";
        }
        catch (SqlException ex)
        {
            TextLog.Text = ex.Message;
        }
    }

    private void OnInstall(object sender, RoutedEventArgs e)
    {
        TextLog.Text = "";
        if (!int.TryParse(TextPortService.Text, out int PortService))
        {
            WriteLineLog("Укажите порт сервиса");
            return;
        }
        
        if (!int.TryParse(TextPortServer.Text, out int PortServer))
        {
            WriteLineLog("Укажите порт сервера");
            return;
        }
        
        if (!TryCreateDb())
            return;
        
        if(!TryExtractWebServerApp(PortServer, PortService))
            return;
        
        if(!TryExtractWebServiceApp(PortService))
            return;

        MessageBox.Show("Сервис и сервер успешно установлны!", null, MessageBoxButton.OK, MessageBoxImage.Information);
        
        // string messageBoxText = "Все установилось успешно.\r\nЗапустить сервер и сервис ?";
        // var result = MessageBox.Show(messageBoxText, null, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
        // if (result == MessageBoxResult.Yes)
        // {
        //     var exe = new DirectoryInfo(TextPathService.Text).GetFiles("*.exe").FirstOrDefault();
        //     if (exe != null)
        //         Process.Start(exe.FullName);
        //     
        //     exe = new DirectoryInfo(TextPathServer.Text).GetFiles("*.exe").FirstOrDefault();
        //     if (exe != null)
        //         Process.Start(exe.FullName);
        //     
        //     
        //     
        //     messageBoxText = "Открыть сайт сервера ?";
        //     result = MessageBox.Show(messageBoxText, null, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
        //
        //     if (result == MessageBoxResult.Yes)
        //     {
        //         var psi = new ProcessStartInfo
        //         {
        //             FileName = $"http://localhost:{PortServer}/",
        //             UseShellExecute = true
        //         };
        //         Process.Start (psi);
        //     }
        // }
    }


    private bool TryExtractWebServerApp(int port, int portService)
    {
        try
        {
            var directory = Directory.CreateDirectory(TextPathServer.Text);

            var assembly = this.GetType().Assembly;
            var streamName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("WebServerApp.zip"));
            WriteLineLog($"Извлекаю {streamName} в {directory.FullName}");
            
            using (var stream = assembly.GetManifestResourceStream(streamName))
            {
                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false, entryNameEncoding: null))
                {
                    zipArchive.ExtractToDirectory(directory.FullName);
                }
            }
            
            WriteLineLog("DONE!");
            
            var settingsFile = Path.Combine(directory.FullName, "settings.json");
            File.WriteAllText(settingsFile, JsonSerializer.Serialize(new 
            {
                ExternalServiceUrl = $"http://localhost:{portService}",
                Port = port
            }));

            return true;
        }
        catch (Exception ex)
        {
            WriteLineLog(ex);
            return false;
        }
    }
    
    private bool TryExtractWebServiceApp(int port)
    {
        try
        {
            var directory = Directory.CreateDirectory(TextPathService.Text);

            var assembly = this.GetType().Assembly;
            var streamName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("WebServiceApp.zip"));
            WriteLineLog($"Извлекаю {streamName} в {directory.FullName}");
            
            using (var stream = assembly.GetManifestResourceStream(streamName))
            {
                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false, entryNameEncoding: null))
                {
                    zipArchive.ExtractToDirectory(directory.FullName);
                }
            }

            var settingsFile = Path.Combine(directory.FullName, "settings.json");
            File.WriteAllText(settingsFile, JsonSerializer.Serialize(new 
            {
                ConnectionString = TextConnectionString.Text,
                Port = port
            }));
            
            WriteLineLog("DONE!");
            
            return true;
        }
        catch (Exception ex)
        {
            WriteLineLog(ex);
            return false;
        }
    }

    private void WriteLineLog(string text)
    {
        TextLog.Text += (text ?? "") + "\r\n";
    }
    
    private void WriteLineLog(Exception e)
    {
        TextLog.Text += $"{e.Message}\r\n{e.StackTrace}\r\n";
    }

    private bool TryCreateDb()
    {
        try
        {
            SqlConnection connection = new SqlConnection(TextConnectionString.Text);
            connection.Open();
            
            foreach (var script in GetSqlScripts())
            {
                WriteLineLog($"Выполняю: \r\n{script}");
                
                SqlCommand command = new SqlCommand();
                command.CommandText = script;
                command.Connection = connection;
                command.ExecuteNonQuery();
                
                WriteLineLog("DONE!");
            }

            return true;
        }
        catch (Exception e)
        {
            WriteLineLog(e);
            return false;
        }
    }

    private IEnumerable<string> GetSqlScripts()
    {
        var assembly = this.GetType().Assembly;
        foreach (var resourceName in this.GetType().Assembly.GetManifestResourceNames().OrderBy(x => x))
        {
            if (String.Equals(Path.GetExtension(resourceName), ".sql", StringComparison.OrdinalIgnoreCase))
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    byte[] buffer = new byte[(int)(stream.Length)];
                    stream.Read(buffer, 0, (int)stream.Length);
                    yield return Encoding.UTF8.GetString(buffer).Trim();
                }
            }
        }
    }
}