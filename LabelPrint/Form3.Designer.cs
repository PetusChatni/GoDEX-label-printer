namespace LabelPrint
{
    partial class Form3
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
            components = new System.ComponentModel.Container();
            L_Name = new Label();
            TB_Name = new TextBox();
            L_Path = new Label();
            TB_Path = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            BTN_OpenFileWizard = new Button();
            BTN_AddImg = new Button();
            L_Width = new Label();
            L_Height = new Label();
            MIRV_Preview = new MmImageRulerViewer();
            NUD_Width = new NumericUpDown();
            NUD_Height = new NumericUpDown();
            L_DPI = new Label();
            NUD_DPI = new NumericUpDown();
            toolTip1 = new ToolTip(components);
            L_ImgDiskSize = new Label();
            ((System.ComponentModel.ISupportInitialize)NUD_Width).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Height).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUD_DPI).BeginInit();
            SuspendLayout();
            // 
            // L_Name
            // 
            L_Name.AutoSize = true;
            L_Name.Location = new Point(24, 30);
            L_Name.Name = "L_Name";
            L_Name.Size = new Size(90, 15);
            L_Name.TabIndex = 0;
            L_Name.Text = "Name in printer";
            // 
            // TB_Name
            // 
            TB_Name.Location = new Point(24, 48);
            TB_Name.MaxLength = 20;
            TB_Name.Name = "TB_Name";
            TB_Name.PlaceholderText = "Enter a new name for the file";
            TB_Name.Size = new Size(162, 23);
            TB_Name.TabIndex = 1;
            // 
            // L_Path
            // 
            L_Path.AutoSize = true;
            L_Path.Location = new Point(24, 87);
            L_Path.Name = "L_Path";
            L_Path.Size = new Size(264, 15);
            L_Path.TabIndex = 2;
            L_Path.Text = "Path to image (BMP, GIF, EXIF, JPG, PNG, or TIFF)";
            // 
            // TB_Path
            // 
            TB_Path.Location = new Point(24, 105);
            TB_Path.Name = "TB_Path";
            TB_Path.Size = new Size(285, 23);
            TB_Path.TabIndex = 3;
            TB_Path.Leave += TB_Path_Leave;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.Filter = "Image Files(*.BMP;*.GIF;*.EXIF;*.JPG;*.PNG;*.TIFF)|*.BMP;*.GIF;*.EXIF;*.JPG;*.PNG;*.TIFF";
            // 
            // BTN_OpenFileWizard
            // 
            BTN_OpenFileWizard.BackColor = SystemColors.Window;
            BTN_OpenFileWizard.FlatAppearance.BorderColor = Color.DimGray;
            BTN_OpenFileWizard.FlatAppearance.MouseDownBackColor = Color.Gainsboro;
            BTN_OpenFileWizard.FlatAppearance.MouseOverBackColor = Color.WhiteSmoke;
            BTN_OpenFileWizard.FlatStyle = FlatStyle.Flat;
            BTN_OpenFileWizard.Location = new Point(308, 105);
            BTN_OpenFileWizard.Name = "BTN_OpenFileWizard";
            BTN_OpenFileWizard.Size = new Size(28, 23);
            BTN_OpenFileWizard.TabIndex = 4;
            BTN_OpenFileWizard.Text = "...";
            BTN_OpenFileWizard.UseVisualStyleBackColor = false;
            BTN_OpenFileWizard.Click += BTN_OpenFileWizard_Click;
            // 
            // BTN_AddImg
            // 
            BTN_AddImg.Location = new Point(144, 143);
            BTN_AddImg.Name = "BTN_AddImg";
            BTN_AddImg.Size = new Size(75, 23);
            BTN_AddImg.TabIndex = 5;
            BTN_AddImg.Text = "Add image";
            BTN_AddImg.UseVisualStyleBackColor = true;
            BTN_AddImg.Click += BTN_AddImg_Click;
            // 
            // L_Width
            // 
            L_Width.AutoSize = true;
            L_Width.Location = new Point(24, 184);
            L_Width.Name = "L_Width";
            L_Width.Size = new Size(72, 15);
            L_Width.TabIndex = 6;
            L_Width.Text = "Width (mm)";
            // 
            // L_Height
            // 
            L_Height.AutoSize = true;
            L_Height.Location = new Point(203, 184);
            L_Height.Name = "L_Height";
            L_Height.Size = new Size(76, 15);
            L_Height.TabIndex = 8;
            L_Height.Text = "Height (mm)";
            // 
            // MIRV_Preview
            // 
            MIRV_Preview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MIRV_Preview.AutoScroll = true;
            MIRV_Preview.BackColor = Color.FromArgb(240, 240, 240);
            MIRV_Preview.DPI = 203F;
            MIRV_Preview.Location = new Point(360, 30);
            MIRV_Preview.Name = "MIRV_Preview";
            MIRV_Preview.Size = new Size(250, 250);
            MIRV_Preview.TabIndex = 12;
            MIRV_Preview.DPIUpdated += UpdateScaleFromMIRV;
            MIRV_Preview.ImageSizeUpdated += UpdateImageDiskSize;
            // 
            // NUD_Width
            // 
            NUD_Width.DecimalPlaces = 2;
            NUD_Width.Location = new Point(24, 202);
            NUD_Width.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NUD_Width.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            NUD_Width.Name = "NUD_Width";
            NUD_Width.Size = new Size(120, 23);
            NUD_Width.TabIndex = 7;
            toolTip1.SetToolTip(NUD_Width, "0,01 - 1000");
            NUD_Width.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            NUD_Width.ValueChanged += NUD_Width_ValueChanged;
            // 
            // NUD_Height
            // 
            NUD_Height.DecimalPlaces = 2;
            NUD_Height.Location = new Point(203, 202);
            NUD_Height.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            NUD_Height.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            NUD_Height.Name = "NUD_Height";
            NUD_Height.Size = new Size(120, 23);
            NUD_Height.TabIndex = 9;
            toolTip1.SetToolTip(NUD_Height, "0,01 - 1000");
            NUD_Height.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            NUD_Height.ValueChanged += NUD_Height_ValueChanged;
            // 
            // L_DPI
            // 
            L_DPI.AutoSize = true;
            L_DPI.Location = new Point(110, 239);
            L_DPI.Name = "L_DPI";
            L_DPI.Size = new Size(71, 15);
            L_DPI.TabIndex = 10;
            L_DPI.Text = "Printer's DPI";
            // 
            // NUD_DPI
            // 
            NUD_DPI.DecimalPlaces = 2;
            NUD_DPI.Location = new Point(110, 257);
            NUD_DPI.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            NUD_DPI.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NUD_DPI.Name = "NUD_DPI";
            NUD_DPI.Size = new Size(120, 23);
            NUD_DPI.TabIndex = 11;
            toolTip1.SetToolTip(NUD_DPI, "Enter DPI that is used on your printer");
            NUD_DPI.Value = new decimal(new int[] { 203, 0, 0, 0 });
            NUD_DPI.ValueChanged += NUD_DPI_ValueChanged;
            // 
            // toolTip1
            // 
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            // 
            // L_ImgDiskSize
            // 
            L_ImgDiskSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            L_ImgDiskSize.Location = new Point(360, 12);
            L_ImgDiskSize.Name = "L_ImgDiskSize";
            L_ImgDiskSize.Size = new Size(250, 15);
            L_ImgDiskSize.TabIndex = 13;
            L_ImgDiskSize.Text = "0 B";
            L_ImgDiskSize.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(621, 291);
            Controls.Add(L_ImgDiskSize);
            Controls.Add(NUD_DPI);
            Controls.Add(L_DPI);
            Controls.Add(NUD_Height);
            Controls.Add(NUD_Width);
            Controls.Add(MIRV_Preview);
            Controls.Add(L_Width);
            Controls.Add(L_Height);
            Controls.Add(BTN_AddImg);
            Controls.Add(BTN_OpenFileWizard);
            Controls.Add(TB_Path);
            Controls.Add(L_Path);
            Controls.Add(TB_Name);
            Controls.Add(L_Name);
            MinimumSize = new Size(637, 330);
            Name = "Form3";
            Text = "Image upload";
            ((System.ComponentModel.ISupportInitialize)NUD_Width).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_Height).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUD_DPI).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label L_Name;
        private TextBox TB_Name;
        private Label L_Path;
        private TextBox TB_Path;
        private OpenFileDialog openFileDialog1;
        private Button BTN_OpenFileWizard;
        private Button BTN_AddImg;
        private Label L_Width;
        private Label L_Height;
        private MmImageRulerViewer MIRV_Preview;
        private NumericUpDown NUD_Width;
        private NumericUpDown NUD_Height;
        private Label L_DPI;
        private NumericUpDown NUD_DPI;
        private ToolTip toolTip1;
        private Label L_ImgDiskSize;
    }
}