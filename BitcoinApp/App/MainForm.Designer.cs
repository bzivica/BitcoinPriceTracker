namespace BitcoinApp
{
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
            panel1 = new Panel();
            tabControl = new TabControl();
            tabLiveData = new TabPage();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            tabSavedData = new TabPage();
            button3 = new Button();
            button2 = new Button();
            dataGridView2 = new DataGridView();
            panel1.SuspendLayout();
            tabControl.SuspendLayout();
            tabLiveData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabSavedData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(tabControl);
            panel1.Location = new Point(-3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(804, 292);
            panel1.TabIndex = 0;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabLiveData);
            tabControl.Controls.Add(tabSavedData);
            tabControl.Location = new Point(3, 2);
            tabControl.Name = "tabLiveData";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(798, 289);
            tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabLiveData.Controls.Add(button1);
            tabLiveData.Controls.Add(dataGridView1);
            tabLiveData.Location = new Point(4, 29);
            tabLiveData.Name = "tabLiveData";
            tabLiveData.Padding = new Padding(3);
            tabLiveData.Size = new Size(790, 256);
            tabLiveData.TabIndex = 0;
            tabLiveData.Text = "Live Data";
            tabLiveData.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(657, 209);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 1;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonSave_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(784, 188);
            dataGridView1.TabIndex = 0;
            // 
            // tabSavedData
            // 
            tabSavedData.Controls.Add(button3);
            tabSavedData.Controls.Add(button2);
            tabSavedData.Controls.Add(dataGridView2);
            tabSavedData.Location = new Point(4, 29);
            tabSavedData.Name = "tabSavedData";
            tabSavedData.Padding = new Padding(3);
            tabSavedData.Size = new Size(790, 256);
            tabSavedData.TabIndex = 1;
            tabSavedData.Text = "Saved Data";
            tabSavedData.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(674, 218);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 2;
            button3.Text = "Save";
            button3.UseVisualStyleBackColor = true;
            button3.Click += buttonSaveChanges_Click;
            // 
            // button2
            // 
            button2.Location = new Point(532, 218);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 1;
            button2.Text = "Delete";
            button2.UseVisualStyleBackColor = true;
            button2.Click += buttonDelete_Click;
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Dock = DockStyle.Top;
            dataGridView2.Location = new Point(3, 3);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.Size = new Size(784, 188);
            dataGridView2.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 296);
            Controls.Add(panel1);
            Name = "MainForm";
            Text = "Bitcoin Price Tracker";
            Load += MainForm_Load;
            panel1.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            tabLiveData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabSavedData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TabControl tabControl;
        private TabPage tabLiveData;
        private TabPage tabSavedData;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button button1;
        private Button button3;
        private Button button2;
    }
}
