using System.Windows.Forms;

namespace LabelPrint
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BTN_Retrieve = new Button();
            BTN_AddImg = new Button();
            DGV_Files = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            btnColumn = new DataGridViewButtonColumn();
            menuStrip1 = new MenuStrip();
            homeToolStripMenuItem = new ToolStripMenuItem();
            printerMemoryManagerToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)DGV_Files).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // BTN_Retrieve
            // 
            BTN_Retrieve.Location = new Point(12, 27);
            BTN_Retrieve.Name = "BTN_Retrieve";
            BTN_Retrieve.Size = new Size(84, 23);
            BTN_Retrieve.TabIndex = 0;
            BTN_Retrieve.Text = "Get All Files";
            BTN_Retrieve.UseVisualStyleBackColor = true;
            BTN_Retrieve.Click += BTN_Retrieve_Click;
            // 
            // BTN_AddImg
            // 
            BTN_AddImg.Location = new Point(117, 27);
            BTN_AddImg.Name = "BTN_AddImg";
            BTN_AddImg.Size = new Size(97, 23);
            BTN_AddImg.TabIndex = 2;
            BTN_AddImg.Text = "Add Image File";
            BTN_AddImg.UseVisualStyleBackColor = true;
            BTN_AddImg.Click += BTN_AddImg_Click;
            // 
            // DGV_Files
            // 
            DGV_Files.AllowUserToAddRows = false;
            DGV_Files.AllowUserToDeleteRows = false;
            DGV_Files.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGV_Files.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Files.ColumnHeadersVisible = false;
            DGV_Files.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, btnColumn });
            DGV_Files.EditMode = DataGridViewEditMode.EditProgrammatically;
            DGV_Files.Location = new Point(12, 66);
            DGV_Files.Name = "DGV_Files";
            DGV_Files.RowHeadersVisible = false;
            DGV_Files.Size = new Size(442, 383);
            DGV_Files.TabIndex = 3;
            DGV_Files.CellContentClick += DGV_Files_CellContentClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Name";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Type";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 50;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Size";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // btnColumn
            // 
            btnColumn.HeaderText = "Action";
            btnColumn.Name = "btnColumn";
            btnColumn.Text = "Delete";
            btnColumn.UseColumnTextForButtonValue = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { homeToolStripMenuItem, printerMemoryManagerToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(466, 24);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // homeToolStripMenuItem
            // 
            homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            homeToolStripMenuItem.Size = new Size(52, 20);
            homeToolStripMenuItem.Text = "Home";
            homeToolStripMenuItem.Click += homeToolStripMenuItem_Click;
            // 
            // printerMemoryManagerToolStripMenuItem
            // 
            printerMemoryManagerToolStripMenuItem.Name = "printerMemoryManagerToolStripMenuItem";
            printerMemoryManagerToolStripMenuItem.Size = new Size(152, 20);
            printerMemoryManagerToolStripMenuItem.Text = "Printer Memory Manager";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(466, 461);
            Controls.Add(DGV_Files);
            Controls.Add(BTN_AddImg);
            Controls.Add(BTN_Retrieve);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(380, 500);
            Name = "Form2";
            Text = "Printer Memory Manager";
            FormClosed += Form2_FormClosed;
            ((System.ComponentModel.ISupportInitialize)DGV_Files).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BTN_Retrieve;
        private Button BTN_AddImg;
        private DataGridView DGV_Files;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewButtonColumn btnColumn;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem homeToolStripMenuItem;
        private ToolStripMenuItem printerMemoryManagerToolStripMenuItem;
    }
}