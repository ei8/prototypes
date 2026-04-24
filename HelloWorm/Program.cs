
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    services.AddSingleton<ISelectionService, SelectionService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddTransient<frmMain>();
                    services.AddTransient<frmDish>();
                    services.AddSingleton<frmToolbox>();
                    services.AddSingleton<frmProperties>();
                });
        }
    }
}