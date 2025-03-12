using System.IO;
using System.Net.NetworkInformation;
using BitcoinApp.Database.Wrappers.Interfaces;
using BitcoinApp.Database.Wrappers;
using BitcoinApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BitcoinApp.Services.Interfaces;

namespace BitcoinApp.App;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Create the host using the CreateHostBuilder method
        var host = CreateHostBuilder().Build();

        // Run the application
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(host.Services.GetRequiredService<MainForm>());
    }

    // Method to create and configure the host
    public static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Load configuration from appsettings.json
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Register HttpClient with IHttpClientFactory
                services.AddHttpClient();

                // Register the services, such as CoindeskService and CnbService
                services.AddSingleton<ICoindeskService, CoindeskService>();
                services.AddSingleton<ICoinGeckoService, CoinGeckoService>();
                services.AddSingleton<ICnbService, CnbService>();

                // Register BitcoinPriceCalculatorService for calculating Bitcoin price
                services.AddSingleton<BitcoinPriceCalculatorService>();

                // Register DatabaseService for database interaction
                services.AddTransient<IDatabaseConnectionFactory, SqlDatabaseConnectionFactory>();
                services.AddScoped<IDatabaseService, DatabaseService>();

                // Register the MainForm as a service
                services.AddScoped<MainForm>();
            })
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
            });
    }
}