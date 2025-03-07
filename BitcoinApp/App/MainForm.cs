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
            
            InitialSettings();

            StartTimer();
        }

        private void InitialSettings()
        {
            button1.BackColor = Color.Blue;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;  // Moderní vzhled bez okrajů
            button1.FlatAppearance.BorderSize = 0;  // Bez okrajů
            button1.MouseEnter += (s, e) => { button1.BackColor = Color.DarkBlue; };
            button1.MouseLeave += (s, e) => { button1.BackColor = Color.Blue; };

            button2.BackColor = Color.Blue;
            button2.ForeColor = Color.White;
            button2.FlatStyle = FlatStyle.Flat;  // Moderní vzhled bez okrajů
            button2.FlatAppearance.BorderSize = 0;  // Bez okrajů
            button2.MouseEnter += (s, e) => { button1.BackColor = Color.DarkBlue; };
            button2.MouseLeave += (s, e) => { button1.BackColor = Color.Blue; };

            button3.BackColor = Color.Blue;
            button3.ForeColor = Color.White;
            button3.FlatStyle = FlatStyle.Flat;  // Moderní vzhled bez okrajů
            button3.FlatAppearance.BorderSize = 0;  // Bez okrajů
            button3.MouseEnter += (s, e) => { button1.BackColor = Color.DarkBlue; };
            button3.MouseLeave += (s, e) => { button1.BackColor = Color.Blue; };

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
                dataGridView1.Rows.Add(bitcoinPriceEUR, bitcoinPriceCZK, DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Chyba při získávání dat: {ex.Message}");
            }
        }

        private void FormatTimestampColumn()
        {
            if (dataGridView2.Columns.Contains("Timestamp"))
            {
                dataGridView2.Columns["Timestamp"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";  // Datum + čas
            }
        }

        private void LoadBitcoinData()
        {
            // Předpokládáme, že máš třídu DatabaseService pro práci s databází
            var bitcoinData = _databaseService.GetSavedBitcoinData();  // Tato metoda vrátí seznam BitcoinData

            // Nastavujeme data jako DataSource pro DataGridView
            dataGridView2.DataSource = bitcoinData;

            if (dataGridView2.Columns.Contains("Note"))
            {
                dataGridView2.Columns["Note"].ReadOnly = false;
            }

            FormatTimestampColumn();
        }

        // Tato metoda bude zavolána při každém tiku timeru
        private async void OnTimerElapsed(object? state)
        {
            try
            {
                // Získání ceny Bitcoinu v CZK
                (bitcoinPriceEUR, bitcoinPriceCZK) = await _priceCalculatorService.GetBitcoinPriceAsync();

                // Vyprázdnění DataGridView před přidáním nových dat
                dataGridView1.Invoke((Action)(() =>
                {
                    dataGridView1.Rows.Clear();  // Vymaže všechny předchozí řádky
                    dataGridView1.Rows.Add(bitcoinPriceEUR, bitcoinPriceCZK, DateTime.Now);  // Přidá nový řádek s aktuálními daty
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Pokud je aktivní tab "Saved Data" (nebo podobný)
            if (tabControl.SelectedTab?.Name == "tabSavedData")
            {
                // Zavoláme metodu pro načtení historických dat
                LoadBitcoinData();
            }
        }
    }
}
