using System.IO;
using BitcoinApp.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;

namespace BitcoinApp.App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Vytvoření konfiguračního objektu
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            // Načtení connection stringu z konfigurace
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            // Create instances of CoindeskService and CnbService
            var coindeskService = new CoindeskService();
            var cnbService = new CnbService();

            // Create an instance of BitcoinPriceCalculatorService
            var priceCalculatorService = new BitcoinPriceCalculatorService(coindeskService, cnbService);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(connectionString, configuration, priceCalculatorService));
        }
    }
}