using BitcoinApp.Models;
using BitcoinApp.Services;
using BitcoinApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace BitcoinApp;

public partial class MainForm : Form
{
    private readonly IDatabaseService _databaseService;
    private readonly BitcoinPriceCalculatorService _priceCalculatorService;

    // Declare variables to hold Bitcoin prices
    private decimal bitcoinPriceEUR;
    private decimal bitcoinPriceCZK;
    // Declare the _timer field as nullable
    private System.Threading.Timer? _timer; // Use Windows Forms Timer
    private readonly int _fetchInterval; // Interval for fetching Bitcoin prices
    private bool _isTimerProcessing = false;
    private bool _isProgrammaticallyUpdating = false;
    private int _bulkSize = 100;
    private const string FETCH_INTERVAL_KEY = "AppSettings:DataFetchInterval";
    private const string BULK_SIZE_KEY = "Database:BulkSize";
    private List<BitcoinDataTable> originalData = new List<BitcoinDataTable>();

    #region Constructor
    // Constructor
    public MainForm(IConfiguration configuration, BitcoinPriceCalculatorService priceCalculatorService,
        IDatabaseService databaseService)
    {
        _priceCalculatorService = priceCalculatorService;

        _fetchInterval = configuration.GetValue<int>(FETCH_INTERVAL_KEY);
        _bulkSize = configuration.GetValue<int>(BULK_SIZE_KEY);

        _databaseService = databaseService;

        InitializeComponent();

    }
    #endregion

    #region Initialization
    private async Task InitializeAsync()
    {
        // Create database schema if it doesn't exist
        await _databaseService.CreateDatabaseSchemaAsync(CancellationToken.None);

        // Initial settings and starting the timer
        InitialSettings();
        StartTimer();
    }

    // Initial setup for button styles
    private void InitialSettings()
    {
        SetupButtonStyle(button1);
        SetupButtonStyle(buttonDelete);
        SetupButtonStyle(buttonSaveChanges);
    }

    // Helper method to set up button styles
    private void SetupButtonStyle(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.MouseEnter += (s, e) => { button.BackColor = Color.DarkBlue; };
        button.MouseLeave += (s, e) => { button.BackColor = SystemColors.Control; };
    }

    // Format the Timestamp column in the DataGridView
    private void FormatTimestampColumn()
    {
        if (dataGridViewSaved.Columns.Contains("Timestamp"))
        {
            dataGridViewSaved.Columns["Timestamp"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
        }
    }
    #endregion
    
    #region Timer methods
    // Start the timer for periodic updates
    private void StartTimer()
    {
        if (!_isTimerProcessing)
        {
            // Initialize the timer with an initial delay of 0 ms and the specified interval
            _timer = new System.Threading.Timer(OnTimerElapsed, null, 0, _fetchInterval);
            _isTimerProcessing = false;
        }
    }

    // Stop the timer
    private void StopTimer()
    {
        if (!_isTimerProcessing)
        {
            // Stop the timer by changing it to Timeout.Infinite (no future triggers)
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _isTimerProcessing = true;
        }
    }

    #endregion

    #region Form events handling
    // Form load event
    private async void MainForm_Load(object sender, EventArgs e)
    {
        await InitializeAsync();

        // Setting up DataGridView for TabLiveData on the main UI thread
        await Task.Run(() => SetupDataGridViewsForTabLiveData());
        await Task.Run(() => SetupDataGridViewsForTabSavedData());
    }

    private async void OnTimerElapsed(object? state)
    {
        try
        {
            // Prevent concurrent execution if already processing
            if (_isTimerProcessing)
                return;

            StopTimer();

            // Temporarily stop the timer to prevent overlap
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            // Fetch the latest Bitcoin prices
            (bitcoinPriceEUR, bitcoinPriceCZK) = await _priceCalculatorService.GetBitcoinPriceAsync();

            // Update the DataGridView safely on the UI thread
            if (dataGridViewLive.InvokeRequired)
            {
                dataGridViewLive.Invoke(new Action(() =>
                {
                    // Add a new row to the DataGridView instead of clearing it
                    dataGridViewLive.Rows.Add(
                        true,
                        bitcoinPriceEUR,
                        bitcoinPriceCZK,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) // Format the current time to include seconds
                    );
                }));
            }
            else
            {
                // In case Invoke is not required (UI thread)
                dataGridViewLive.Rows.Add(
                    bitcoinPriceEUR,
                    bitcoinPriceCZK,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // Format the current time to include seconds
                );
            }

            // Restart the timer with the specified interval
            _timer?.Change(_fetchInterval, Timeout.Infinite);

            _isTimerProcessing = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while fetching data: {ex.Message}");
            _isTimerProcessing = false;
        }
    }
    #endregion

    #region DBdata loading

    // Method to load Bitcoin data from the database and save original data
    private async void LoadBitcoinData()
    {
        try
        {
            // Asynchronously load Bitcoin data from the database
            var bitcoinData = await Task.Run(() => _databaseService.GetAllBitcoinDataAsync(CancellationToken.None));

            // Uložíme kopii původních dat - Vytvoření hluboké kopie
            originalData = bitcoinData.Select(b => new BitcoinDataTable
            {
                Id = b.Id,
                Note = b.Note,
                PriceEUR = b.PriceEUR,
                PriceCZK = b.PriceCZK,
                Timestamp = b.Timestamp
            }).ToList(); // Uložíme novou instanci pro každý objekt

            // Update DataGridView on the main thread
            dataGridViewSaved.Invoke((MethodInvoker)(() =>
            {
                dataGridViewSaved.DataSource = bitcoinData;
                if (dataGridViewSaved.Columns.Contains("Note"))
                {
                    dataGridViewSaved.Columns["Note"].ReadOnly = false;
                }
                SetupDataGridViewsForTabSavedData();
            }));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading historical data: {ex.Message}");
        }
    }
    #endregion 

    #region Helper methods

    // Method to change focus to the "Select All" button for Live Data
    private void CommitFocusChangeToSelectAllButtonLive()
    {
        // Change focus to the "Select All" button for Live data
        buttonSelectAllLive.Focus();
    }

    // Method to change focus to the "Select All" button for Saved Data
    private void CommitFocusChangeToSelectAllbuttonSaved()
    {
        // Change focus to the "Select All" button for Saved data
        buttonSelectAllSaved.Focus();
    }

    private void SetupDataGridViewsForTabLiveData()
    {
        // Get all DataGridViews within tabPage1 (assuming you have one or more DataGridViews)
        foreach (Control control in tabLiveData.Controls)
        {
            if (control is DataGridView dataGridView)
            {
                // Nastavení šířky sloupců pro TabPage1
                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].Width = 150;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].Width = 150;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].Width = 220;

                if (dataGridView.Columns.Contains("Select"))
                    dataGridView.Columns["Select"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss"; // Formát pro datum
            }
        }
    }

    private void SetupDataGridViewsForTabSavedData()
    {
        // Get all DataGridViews within tabPage2 (assuming you have multiple DataGridViews)
        foreach (Control control in tabSavedData.Controls)
        {
            if (control is DataGridView dataGridView)
            {
                // Nastavení šířky sloupců
                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].Width = 150;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].Width = 150;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].Width = 220;
                if (dataGridView.Columns.Contains("Note"))
                    dataGridView.Columns["Note"].Width = 330;

                if (dataGridView.Columns.Contains("Select"))
                    dataGridView.Columns["Select"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("Note"))
                    dataGridView.Columns["Note"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (dataGridView.Columns.Contains("Note"))
                    dataGridView.Columns["Note"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].DefaultCellStyle.Format = "N3";
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].DefaultCellStyle.Format = "N3";
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

                if (dataGridView.Columns.Contains("Note"))
                    dataGridView.Columns["Note"].ReadOnly = false;
                if (dataGridView.Columns.Contains("PriceEUR"))
                    dataGridView.Columns["PriceEUR"].ReadOnly = true;
                if (dataGridView.Columns.Contains("PriceCZK"))
                    dataGridView.Columns["PriceCZK"].ReadOnly = true;
                if (dataGridView.Columns.Contains("Timestamp"))
                    dataGridView.Columns["Timestamp"].ReadOnly = true;

            }
        }
    }

    // Method to check if all checkboxes are checked based on a flag (Live/Saved)
    private bool AreAllCheckboxesChecked(bool flagChangeLive)
    {
        // Select grid based on the flag
        DataGridView dataGridView = flagChangeLive ? dataGridViewLive : dataGridViewSaved;

        // Select checkbox column based on the flag
        string checkboxColumnName = flagChangeLive ? "chbSelectLive" : "chbSelectSaved";

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            // If any checkbox is unchecked or null, return false
            if (row.Cells[checkboxColumnName].Value == null || !Convert.ToBoolean(row.Cells[checkboxColumnName].Value))
            {
                return false;
            }
        }
        return true; // Return true if all checkboxes are checked
    }

    // Method to update Select All button text based on the state of checkboxes
    private void UpdateSelectAllButtonState(bool flagChangeLive)
    {
        // Select grid and button based on the flag
        DataGridView dataGridView = flagChangeLive ? dataGridViewLive : dataGridViewSaved;
        Button button = flagChangeLive ? buttonSelectAllLive : buttonSelectAllSaved;

        bool allChecked = AreAllCheckboxesChecked(flagChangeLive); // Check if all checkboxes are checked

        // Update the button text based on whether all checkboxes are checked or not
        button.Text = allChecked ? "Deselect All" : "Select All";
    }

    #region Data manipulation
    private async void buttonSave_Click(object sender, EventArgs e)
    {
        try
        {
            var dataToInsert = new List<BitcoinData>();

            // Iterate through the rows in the Live Data DataGridView
            foreach (DataGridViewRow row in dataGridViewLive.Rows)
            {
                if (row.IsNewRow || !Convert.ToBoolean(row.Cells["chbSelectLive"].Value)) continue; // Skip new row and unchecked rows

                // Extract values from the cells
                var priceEUR = Convert.ToDecimal(row.Cells["PriceEUR"].Value);
                var priceCZK = Convert.ToDecimal(row.Cells["PriceCZK"].Value);
                var timestamp = Convert.ToDateTime(row.Cells["Timestamp"].Value);

                // Add to list for bulk insert
                dataToInsert.Add(new BitcoinData
                {
                    PriceEUR = priceEUR,
                    PriceCZK = priceCZK,
                    Timestamp = timestamp,
                    Note = string.Empty
                });
            }

            if (dataToInsert.Count > 0)
            {
                // Call the stored procedure for bulk insert with CancellationToken.None
                await _databaseService.BulkInsertBitcoinDataAsync(dataToInsert, _bulkSize, CancellationToken.None);
                MessageBox.Show("Data successfully saved.");
            }
            else
            {
                MessageBox.Show("No data to save.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while saving changes: {ex.Message}");
        }
    }


    // Delete selected Bitcoin data rows from the database using the Bulk Delete stored procedure
    private async void buttonDelete_Click(object sender, EventArgs e)
    {
        try
        {
            var idsToDelete = new List<int>();

            // Iterate through all rows in the Saved Data DataGridView
            foreach (DataGridViewRow row in dataGridViewSaved.Rows)
            {
                // Skip new row (empty row for user input)
                if (row.IsNewRow) continue;

                // Check if the checkbox in the row is checked (the checkbox is in the "chbSelectSaved" column)
                if (Convert.ToBoolean(row.Cells["chbSelectSaved"].Value))
                {
                    var id = (int)row.Cells["Id"].Value; // Get the ID of the row

                    // Add the ID to the list for bulk delete
                    idsToDelete.Add(id);
                }
            }

            // If there are IDs to delete, call the BulkDelete stored procedure
            if (idsToDelete.Count > 0)
            {
                // Call the stored procedure for bulk delete
                await _databaseService.BulkDeleteBitcoinDataAsync(idsToDelete, _bulkSize, CancellationToken.None);  // Assuming batch size 100
                MessageBox.Show("Selected rows were deleted.");
                // Refresh the DataGridView to show updated data
                LoadBitcoinData();
            }
            else
            {
                MessageBox.Show("No rows selected for deletion.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while deleting data: {ex.Message}");
        }
    }


    // Method to save changes
    private async void buttonSaveChanges_Click(object sender, EventArgs e)
    {
        try
        {
            var dataToUpdate = new List<BitcoinDataTable>();

            // Iterate through the rows in the Saved Data DataGridView
            foreach (DataGridViewRow row in dataGridViewSaved.Rows)
            {
                if (row.IsNewRow) continue; // Skip the new row

                // Check if the checkbox in the row is checked (the checkbox is in the "chbSelectSaved" column)
                if (Convert.ToBoolean(row.Cells["chbSelectSaved"].Value))
                {
                    var id = (int)row.Cells["Id"].Value;
                    var note = row.Cells["Note"].Value?.ToString() ?? string.Empty;

                    // Find the original data from the list
                    var original = originalData.FirstOrDefault(x => x.Id == id);

                    if (original != null && original.Note != note)  // Only include if Note changed
                    {
                        dataToUpdate.Add(new BitcoinDataTable
                        {
                            Id = id,
                            Note = note,
                            PriceEUR = Convert.ToDecimal(row.Cells["PriceEUR"].Value),
                            PriceCZK = Convert.ToDecimal(row.Cells["PriceCZK"].Value),
                            Timestamp = Convert.ToDateTime(row.Cells["Timestamp"].Value)
                        });
                    }
                }
            }

            if (dataToUpdate.Count > 0)
            {
                // Call the stored procedure for bulk update
                await _databaseService.BulkUpdateBitcoinDataAsync(dataToUpdate, _bulkSize, CancellationToken.None);  // Assuming batch size 100
                MessageBox.Show("Changes successfully saved.");

                // Refresh the DataGridView to show updated data
                LoadBitcoinData();
            }
            else
            {
                MessageBox.Show("No changes detected.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while saving changes: {ex.Message}");
        }
    }


    #endregion

    // Handle tab control selection change
    private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tabControl.SelectedTab?.Name == "tabSavedData")
        {
            // Load saved Bitcoin data when the "Saved Data" tab is selected
            LoadBitcoinData();
        }
    }

    // Method for handling Select/Deselect All on Live Data grid
    private void buttonSelectAllLive_Click(object sender, EventArgs e)
    {
        _isProgrammaticallyUpdating = true; // Set the flag before updating checkboxes

        bool allChecked = !string.Equals(buttonSelectAllLive.Text, "Deselect All", StringComparison.OrdinalIgnoreCase);

        // Update all checkboxes in the DataGridView
        foreach (DataGridViewRow row in dataGridViewLive.Rows)
        {
            row.Cells["chbSelectLive"].Value = allChecked;
        }

        // Update button text
        buttonSelectAllLive.Text = allChecked ? "Deselect All" : "Select All";

        // Update the button state immediately after updating the checkboxes
        UpdateSelectAllButtonState(true); // true means Live grid

        _isProgrammaticallyUpdating = false; // Reset the flag after updating
    }

    // Method for handling Select/Deselect All on Saved Data grid
    private void buttonSelectAllSaved_Click(object sender, EventArgs e)
    {
        _isProgrammaticallyUpdating = true; // Set the flag before updating checkboxes

        bool allChecked = !string.Equals(buttonSelectAllSaved.Text, "Deselect All", StringComparison.OrdinalIgnoreCase);

        // Update all checkboxes in the DataGridView
        foreach (DataGridViewRow row in dataGridViewSaved.Rows)
        {
            row.Cells["chbSelectSaved"].Value = allChecked;
        }

        // Update button text
        buttonSelectAllSaved.Text = allChecked ? "Deselect All" : "Select All";

        // Update the button state immediately after updating the checkboxes
        UpdateSelectAllButtonState(false); // false means Saved grid

        _isProgrammaticallyUpdating = false; // Reset the flag after updating
    }

    // Event for checkbox change in DataGridView Live (Live Data)
    private void dataGridViewLive_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (_isProgrammaticallyUpdating) return; // Skip when updating programmatically

        // Check if the clicked cell is in the checkbox column (for Live Grid)
        if (dataGridViewLive.Columns.Contains("chbSelectLive") && e.ColumnIndex == dataGridViewLive.Columns["chbSelectLive"].Index)
        {
            CommitFocusChangeToSelectAllButtonLive(); // Commit focus change to Select All button
                                                      // Now update the button text after the change is committed
            UpdateSelectAllButtonState(true); // true for Live grid
        }
    }

    // Event for checkbox change in DataGridView Saved (Saved Data)
    private void dataGridViewSaved_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (_isProgrammaticallyUpdating) return; // Skip when updating programmatically

        // Check if the clicked cell is in the checkbox column (for Saved Grid)
        if (dataGridViewSaved.Columns.Contains("chbSelectSaved") && e.ColumnIndex == dataGridViewSaved.Columns["chbSelectSaved"].Index)
        {
            // Set the clicked cell as the current cell
            CommitFocusChangeToSelectAllbuttonSaved(); // Commit focus change to Select All button
                                                       // Now update the button text after the change is committed
            UpdateSelectAllButtonState(false); // false for Saved grid
        }
    }
    // Event triggered when closing the form
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        try
        {
            // Stop the timer
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);

            // Release the timer resources
            _timer?.Dispose();
        }
        catch (Exception ex)
        {
            // Show an error message if there is an issue stopping the timer
            MessageBox.Show($"Error stopping the timer: {ex.Message}");
        }
        finally
        {
            // Ensure the base class method is called to handle form closing
            base.OnFormClosing(e);
        }
    }


    #endregion

}

