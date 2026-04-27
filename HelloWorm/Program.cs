
using ei8.Cortex.Library.Client.Out;
using ei8.Prototypes.HelloWorm.Spiker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using neurUL.Common.Http;
using NLog.Extensions.Logging;
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
                    services.AddTransient<IRequestProvider>((sp) =>
                        {
                            var rp = new RequestProvider();
                            rp.SetHttpClientHandler(new HttpClientHandler());
                            return rp;
                        }
                    );
                    services.AddTransient<INeuronQueryClient, HttpNeuronQueryClient>();
                    services.AddSingleton<ISelectionService, SelectionService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<frmMain>();
                    services.AddTransient<frmDish>();
                    services.AddSingleton<frmToolbox>();
                    services.AddSingleton<frmProperties>();
                    services.AddSingleton<frmProjectExplorer>();
                    services.AddSingleton<frmOutput>();
                    services.AddTransient<ISpikeService, SpikeService>();
                    services.AddTransient<Worm>();
                    services.AddTransient<Food>();
                    services.AddTransient<Dish>();
                });
        }
    }
}