using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebServiceApp.Services;
using WebServiceApp.Services.Implementations;

namespace WebServiceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<IEntityContext, EntityContext>();
            

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.WebHost.UseUrls($"http://*:{new SettingsService().GetSettings().Port}");
            var app = builder.Build();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllers();

            app.Run();   
        }
    }
}

