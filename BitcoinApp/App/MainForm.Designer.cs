namespace BitcoinApp;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
        panel1 = new Panel();
        tabControl = new TabControl();
        tabLiveData = new TabPage();
        buttonSelectAllLive = new Button();
        button1 = new Button();
        dataGridViewLive = new DataGridView();
        chbSelectLive = new DataGridViewCheckBoxColumn();
        PriceEUR = new DataGridViewTextBoxColumn();
        PriceCZK = new DataGridViewTextBoxColumn();
        Timestamp = new DataGridViewTextBoxColumn();
        tabSavedData = new TabPage();
        buttonSelectAllSaved = new Button();
        buttonSaveChanges = new Button();
        buttonDelete = new Button();
        dataGridViewSaved = new DataGridView();
        chbSelectSaved = new DataGridViewCheckBoxColumn();
        panel1.SuspendLayout();
        tabControl.SuspendLayout();
        tabLiveData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewLive).BeginInit();
        tabSavedData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewSaved).BeginInit();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Controls.Add(tabControl);
        panel1.Dock = DockStyle.Fill;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(1082, 603);
        panel1.TabIndex = 0;
        // 
        // tabControl
        // 
        tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControl.Controls.Add(tabLiveData);
        tabControl.Controls.Add(tabSavedData);
        tabControl.Location = new Point(3, 2);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(1079, 589);
        tabControl.TabIndex = 0;
        tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
        // 
        // tabLiveData
        // 
        tabLiveData.Controls.Add(buttonSelectAllLive);
        tabLiveData.Controls.Add(button1);
        tabLiveData.Controls.Add(dataGridViewLive);
        tabLiveData.Location = new Point(4, 29);
        tabLiveData.Name = "tabLiveData";
        tabLiveData.Padding = new Padding(3);
        tabLiveData.Size = new Size(1071, 556);
        tabLiveData.TabIndex = 0;
        tabLiveData.Text = "Live Data";
        tabLiveData.UseVisualStyleBackColor = true;
        // 
        // buttonSelectAllLive
        // 
        buttonSelectAllLive.Location = new Point(6, 521);
        buttonSelectAllLive.Name = "buttonSelectAllLive";
        buttonSelectAllLive.Size = new Size(94, 29);
        buttonSelectAllLive.TabIndex = 2;
        buttonSelectAllLive.Text = "Select All";
        buttonSelectAllLive.UseVisualStyleBackColor = true;
        buttonSelectAllLive.Click += buttonSelectAllLive_Click;
        // 
        // button1
        // 
        button1.Location = new Point(969, 521);
        button1.Name = "button1";
        button1.Size = new Size(94, 29);
        button1.TabIndex = 1;
        button1.Text = "Save";
        button1.UseVisualStyleBackColor = true;
        button1.Click += buttonSave_Click;
        // 
        // dataGridViewLive
        // 
        dataGridViewLive.AllowUserToAddRows = false;
        dataGridViewLive.AllowUserToDeleteRows = false;
        dataGridViewLive.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewLive.BackgroundColor = SystemColors.Control;
        dataGridViewLive.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewLive.Columns.AddRange(new DataGridViewColumn[] { chbSelectLive, PriceEUR, PriceCZK, Timestamp });
        dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle8.BackColor = SystemColors.Window;
        dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle8.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
        dataGridViewLive.DefaultCellStyle = dataGridViewCellStyle8;
        dataGridViewLive.Location = new Point(3, 3);
        dataGridViewLive.Name = "dataGridViewLive";
        dataGridViewLive.RowHeadersWidth = 51;
        dataGridViewLive.Size = new Size(1065, 495);
        dataGridViewLive.TabIndex = 0;
        dataGridViewLive.CellContentClick += dataGridViewLive_CellContentClick;
        // 
        // chbSelectLive
        // 
        chbSelectLive.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
        chbSelectLive.HeaderText = "Select";
        chbSelectLive.MinimumWidth = 6;
        chbSelectLive.Name = "chbSelectLive";
        chbSelectLive.Width = 55;
        // 
        // PriceEUR
        // 
        dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle5.Format = "N3";
        dataGridViewCellStyle5.NullValue = null;
        PriceEUR.DefaultCellStyle = dataGridViewCellStyle5;
        PriceEUR.HeaderText = "Price EUR";
        PriceEUR.MinimumWidth = 6;
        PriceEUR.Name = "PriceEUR";
        PriceEUR.ReadOnly = true;
        PriceEUR.Width = 150;
        // 
        // PriceCZK
        // 
        dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle6.Format = "N3";
        dataGridViewCellStyle6.NullValue = null;
        PriceCZK.DefaultCellStyle = dataGridViewCellStyle6;
        PriceCZK.HeaderText = "Pice CZK";
        PriceCZK.MinimumWidth = 6;
        PriceCZK.Name = "PriceCZK";
        PriceCZK.ReadOnly = true;
        PriceCZK.Width = 150;
        // 
        // Timestamp
        // 
        dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle7.Format = "G";
        dataGridViewCellStyle7.NullValue = null;
        Timestamp.DefaultCellStyle = dataGridViewCellStyle7;
        Timestamp.HeaderText = "Date";
        Timestamp.MinimumWidth = 6;
        Timestamp.Name = "Timestamp";
        Timestamp.ReadOnly = true;
        Timestamp.Width = 220;
        // 
        // tabSavedData
        // 
        tabSavedData.Controls.Add(buttonSelectAllSaved);
        tabSavedData.Controls.Add(buttonSaveChanges);
        tabSavedData.Controls.Add(buttonDelete);
        tabSavedData.Controls.Add(dataGridViewSaved);
        tabSavedData.Location = new Point(4, 29);
        tabSavedData.Name = "tabSavedData";
        tabSavedData.Padding = new Padding(3);
        tabSavedData.Size = new Size(1071, 556);
        tabSavedData.TabIndex = 1;
        tabSavedData.Text = "Saved Data";
        tabSavedData.UseVisualStyleBackColor = true;
        // 
        // buttonSelectAllSaved
        // 
        buttonSelectAllSaved.Location = new Point(6, 521);
        buttonSelectAllSaved.Name = "buttonSelectAllSaved";
        buttonSelectAllSaved.Size = new Size(94, 29);
        buttonSelectAllSaved.TabIndex = 3;
        buttonSelectAllSaved.Text = "Select All";
        buttonSelectAllSaved.UseVisualStyleBackColor = true;
        buttonSelectAllSaved.Click += buttonSelectAllSaved_Click;
        // 
        // buttonSaveChanges
        // 
        buttonSaveChanges.Location = new Point(967, 521);
        buttonSaveChanges.Name = "buttonSaveChanges";
        buttonSaveChanges.Size = new Size(94, 29);
        buttonSaveChanges.TabIndex = 2;
        buttonSaveChanges.Text = "Save";
        buttonSaveChanges.UseVisualStyleBackColor = true;
        buttonSaveChanges.Click += buttonSaveChanges_Click;
        // 
        // buttonDelete
        // 
        buttonDelete.Location = new Point(867, 521);
        buttonDelete.Name = "buttonDelete";
        buttonDelete.Size = new Size(94, 29);
        buttonDelete.TabIndex = 1;
        buttonDelete.Text = "Delete";
        buttonDelete.UseVisualStyleBackColor = true;
        buttonDelete.Click += buttonDelete_Click;
        // 
        // dataGridViewSaved
        // 
        dataGridViewSaved.AllowUserToAddRows = false;
        dataGridViewSaved.AllowUserToDeleteRows = false;
        dataGridViewSaved.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dataGridViewSaved.BackgroundColor = SystemColors.Control;
        dataGridViewSaved.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewSaved.Columns.AddRange(new DataGridViewColumn[] { chbSelectSaved });
        dataGridViewSaved.Location = new Point(3, 3);
        dataGridViewSaved.Name = "dataGridViewSaved";
        dataGridViewSaved.RowHeadersWidth = 51;
        dataGridViewSaved.Size = new Size(1060, 495);
        dataGridViewSaved.TabIndex = 0;
        dataGridViewSaved.CellContentClick += dataGridViewSaved_CellContentClick;
        // 
        // chbSelectSaved
        // 
        chbSelectSaved.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
        chbSelectSaved.HeaderText = "Select";
        chbSelectSaved.MinimumWidth = 6;
        chbSelectSaved.Name = "chbSelectSaved";
        chbSelectSaved.Width = 55;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        ClientSize = new Size(1082, 603);
        Controls.Add(panel1);
        ForeColor = SystemColors.ControlText;
        MinimumSize = new Size(1100, 550);
        Name = "MainForm";
        Text = "Bitcoin Price Tracker";
        Load += MainForm_Load;
        panel1.ResumeLayout(false);
        tabControl.ResumeLayout(false);
        tabLiveData.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewLive).EndInit();
        tabSavedData.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataGridViewSaved).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private TabControl tabControl;
    private TabPage tabLiveData;
    private TabPage tabSavedData;
    private DataGridView dataGridViewLive;
    private DataGridView dataGridViewSaved;
    private Button button1;
    private Button buttonSaveChanges;
    private Button buttonDelete;
    private DataGridViewCheckBoxColumn chbSelectSaved;
    private Button buttonSelectAllLive;
    private Button buttonSelectAllSaved;
    private DataGridViewCheckBoxColumn chbSelectLive;
    private DataGridViewTextBoxColumn PriceEUR;
    private DataGridViewTextBoxColumn PriceCZK;
    private DataGridViewTextBoxColumn Timestamp;
}
