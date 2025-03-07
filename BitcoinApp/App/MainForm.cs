using BitcoinApp.Models;
using BitcoinApp.Services;
using Microsoft.Extensions.Configuration;
using System.Windows.Forms;
using System.Threading;

namespace BitcoinApp
{
    public partial class MainForm : Form
    {
        private readonly DatabaseService _databaseService;
        private readonly BitcoinPriceCalculatorService _priceCalculatorService;

        // Declare variables to hold Bitcoin prices
        private decimal bitcoinPriceEUR;
        private decimal bitcoinPriceCZK;
        private System.Threading.Timer? _timer; // Declare _timer as nullable
        private readonly int _fetchInterval = 30000; // 30 seconds

        // Konstruktor formuláře
        public MainForm(string connectionString, IConfiguration configuration, BitcoinPriceCalculatorService priceCalculatorService)
        {
            _priceCalculatorService = priceCalculatorService;
            InitializeComponent();

            _fetchInterval = configuration.GetValue<int>("AppSettings:DataFetchInterval");

            // Inicializace DatabaseService s connection stringem
            _databaseService = new DatabaseService(connectionString, configuration);

            // Vytvoření databáze, pokud neexistuje – pouze pro vývojové účely. 
            // Tento přístup není doporučen pro produkční nasazení.
            //_databaseService.CreateDatabaseIfNotExists();

            _databaseService.CreateDatabaseSchema();

            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new System.Threading.Timer(OnTimerElapsed, null, 0, _fetchInterval);
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Získání ceny Bitcoinu v CZK
                (bitcoinPriceEUR, bitcoinPriceCZK) = await _priceCalculatorService.GetBitcoinPriceAsync();

                // Přidání dat do DataGridView
                dataGridView1.Rows.Add(DateTime.Now, bitcoinPriceEUR, bitcoinPriceCZK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při získávání dat: {ex.Message}");
            }
        }

        private void LoadBitcoinData()
        {
            // Předpokládáme, že máš třídu DatabaseService pro práci s databází
            var bitcoinData = _databaseService.GetSavedBitcoinData();  // Tato metoda vrátí seznam BitcoinData

            // Nastavujeme data jako DataSource pro DataGridView
            dataGridView2.DataSource = bitcoinData;

            // Můžeš také přidat možnosti formátování nebo další úpravy
            dataGridView2.Columns["Id"].Visible = false;  // Skrytí sloupce Id, pokud nechceš, aby byl zobrazen
        }

        // Tato metoda bude zavolána při každém tiku timeru
        private async void OnTimerElapsed(object? state)
        {
            try
            {
                // Získání ceny Bitcoinu v CZK
                (bitcoinPriceEUR, bitcoinPriceCZK) = await _priceCalculatorService.GetBitcoinPriceAsync();

                // Přidání nových dat do DataGridView
                dataGridView1.Invoke((Action)(() =>
                {
                    dataGridView1.Rows.Add(DateTime.Now, bitcoinPriceEUR, bitcoinPriceCZK);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při načítání dat: {ex.Message}");
            }
        }

        // Zastavení timeru při zavření formuláře
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Dispose(); // Uvolnění prostředků timeru
            base.OnFormClosing(e);
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;  // Ignoruje prázdný řádek
                    decimal priceEUR = Convert.ToDecimal(row.Cells["PriceEUR"].Value);
                    decimal priceCZK = Convert.ToDecimal(row.Cells["PriceCZK"].Value);
                    DateTime timestamp = Convert.ToDateTime(row.Cells["Timestamp"].Value);

                    // Aktualizace poznámky v databázi
                    _databaseService.SaveBitcoinData(priceEUR, priceCZK, timestamp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při ukládání změn: {ex.Message}");
            }

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.SelectedRows)
            {
                var id = (int)row.Cells["Id"].Value;

                _databaseService.DeleteBitcoinData(id);
                dataGridView2.Rows.Remove(row);
            }

            MessageBox.Show("Vybrané řádky byly smazány.");
        }

        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;

                var id = (int)row.Cells["Id"].Value;
                var note = row.Cells["Note"].Value?.ToString() ?? string.Empty;

                _databaseService.UpdateBitcoinData(new BitcoinData { Id = id, Note = note });
            }

            MessageBox.Show("Změny byly úspěšně uloženy.");
        }
    }
}
