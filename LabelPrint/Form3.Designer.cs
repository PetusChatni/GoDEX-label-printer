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
            L_Name = new Label();
            TB_Name = new TextBox();
            L_Path = new Label();
            TB_Path = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            BTN_OpenFileWizard = new Button();
            BTN_AddImg = new Button();
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
            L_Path.Location = new Point(24, 95);
            L_Path.Name = "L_Path";
            L_Path.Size = new Size(264, 15);
            L_Path.TabIndex = 2;
            L_Path.Text = "Path to image (BMP, GIF, EXIF, JPG, PNG, or TIFF)";
            // 
            // TB_Path
            // 
            TB_Path.Location = new Point(24, 113);
            TB_Path.Name = "TB_Path";
            TB_Path.Size = new Size(285, 23);
            TB_Path.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "png";
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Image Files(*.BMP;*.GIF;*.EXIF;*.JPG;*.PNG;*.TIFF)|*.BMP;*.GIF;*.EXIF;*.JPG;*.PNG;*.TIFF";
            // 
            // BTN_OpenFileWizard
            // 
            BTN_OpenFileWizard.BackColor = SystemColors.Window;
            BTN_OpenFileWizard.FlatAppearance.BorderColor = Color.DimGray;
            BTN_OpenFileWizard.FlatAppearance.MouseDownBackColor = Color.Gainsboro;
            BTN_OpenFileWizard.FlatAppearance.MouseOverBackColor = Color.WhiteSmoke;
            BTN_OpenFileWizard.FlatStyle = FlatStyle.Flat;
            BTN_OpenFileWizard.Location = new Point(308, 113);
            BTN_OpenFileWizard.Name = "BTN_OpenFileWizard";
            BTN_OpenFileWizard.Size = new Size(28, 23);
            BTN_OpenFileWizard.TabIndex = 4;
            BTN_OpenFileWizard.Text = "...";
            BTN_OpenFileWizard.UseVisualStyleBackColor = false;
            BTN_OpenFileWizard.Click += BTN_OpenFileWizard_Click;
            // 
            // BTN_AddImg
            // 
            BTN_AddImg.Location = new Point(144, 155);
            BTN_AddImg.Name = "BTN_AddImg";
            BTN_AddImg.Size = new Size(75, 23);
            BTN_AddImg.TabIndex = 5;
            BTN_AddImg.Text = "Add image";
            BTN_AddImg.UseVisualStyleBackColor = true;
            BTN_AddImg.Click += BTN_AddImg_Click;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 201);
            Controls.Add(BTN_AddImg);
            Controls.Add(BTN_OpenFileWizard);
            Controls.Add(TB_Path);
            Controls.Add(L_Path);
            Controls.Add(TB_Name);
            Controls.Add(L_Name);
            MinimumSize = new Size(380, 240);
            Name = "Form3";
            Text = "Form3";
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
    }
}