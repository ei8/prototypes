using ei8.Cortex.Coding.Spiker;
using ei8.Cortex.Library.Client.Out;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neurUL.Common.Http;
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
            
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // https://www.experts-exchange.com/questions/27527568/Add-Error-Handler-to-Windows-Form-App.html
            // Add handler to handle the exception raised by main threads
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Add handler to handle the exception raised by additional threads
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.Run(host.Services.GetRequiredService<frmMain>());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) =>
            ShowExceptionDetails(e.Exception);

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionDetails((Exception)e.ExceptionObject);
        }

        static void ShowExceptionDetails(Exception ex)
        {
            // Do logging of exception details
            MessageBox.Show(
                $"An error occurred while executing '{ex.TargetSite?.ToString()}': " +
                $"{string.Concat(Enumerable.Repeat(Environment.NewLine, 2))}" +
                $"{ex.Message}", 
                "Error",
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
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
                    services.AddTransient<frmGraph>();
                    services.AddTransient<frmTree>();
                    services.AddTransient<ISpikeService, SpikeService>();
                    services.AddTransient<Worm>();
                    services.AddTransient<Worksheet>();
                    services.AddTransient<Food>();
                    services.AddTransient<Dish>();
                    services.AddSingleton<IProjectService, ProjectService>();
                    services.AddTransient<Project>();
                });
        }
    }
}