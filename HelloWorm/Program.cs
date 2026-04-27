
using ei8.Prototypes.HelloWorm.Spiker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;

namespace ei8.Prototypes.HelloWorm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var host = CreateHostBuilder().Build();
            Application.Run(host.Services.GetRequiredService<frmMain>());
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(logging => logging.AddConsole());
                    services.AddSingleton<ISelectionService, SelectionService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<frmMain>();
                    services.AddTransient<frmDish>();
                    services.AddSingleton<frmToolbox>();
                    services.AddSingleton<frmProperties>();
                    services.AddSingleton<frmOutput>();
                    services.AddSingleton<ISpikeService, SpikeService>();
                    services.AddTransient<Worm>();
                    services.AddTransient<Food>();
                    services.AddTransient<Dish>();
                });
        }
    }
}