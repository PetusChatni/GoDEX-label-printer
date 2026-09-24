using System.Windows.Forms;

namespace LabelPrint
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            L_IP = new Label();
            L_Port = new Label();
            TB_IP = new TextBox();
            BTN_Connect = new Button();
            NUD_Port = new NumericUpDown();
            L_Data = new Label();
            menuStrip1 = new MenuStrip();
            printerMemoryManagerToolStripMenuItem1 = new ToolStripMenuItem();
            printerMemoryManagerToolStripMenuItem2 = new ToolStripMenuItem();
            menuStrip2 = new MenuStrip();
            importToolStripMenuItem = new ToolStripMenuItem();
            RTB_Data = new RichTextBox();
            BTN_Send = new Button();
            BTN_ConnectSend = new Button();
            BTN_Close = new Button();
            openFileDialog1 = new OpenFileDialog();
            toolStripContainer1 = new ToolStripContainer();
            panel1 = new Panel();
            toolTip1 = new ToolTip(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panel2 = new Panel();
            L_ConnectionStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).BeginInit();
            menuStrip1.SuspendLayout();
            menuStrip2.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // L_IP
            // 
            L_IP.Anchor = AnchorStyles.Top;
            L_IP.AutoSize = true;
            L_IP.Location = new Point(57, 56);
            L_IP.Name = "L_IP";
            L_IP.Size = new Size(60, 15);
            L_IP.TabIndex = 0;
            L_IP.Text = "IP address";
            // 
            // L_Port
            // 
            L_Port.Anchor = AnchorStyles.Top;
            L_Port.AutoSize = true;
            L_Port.Location = new Point(195, 56);
            L_Port.Name = "L_Port";
            L_Port.Size = new Size(29, 15);
            L_Port.TabIndex = 1;
            L_Port.Text = "Port";
            // 
            // TB_IP
            // 
            TB_IP.Anchor = AnchorStyles.Top;
            TB_IP.Location = new Point(57, 74);
            TB_IP.Name = "TB_IP";
            TB_IP.Size = new Size(100, 23);
            TB_IP.TabIndex = 3;
            // 
            // BTN_Connect
            // 
            BTN_Connect.Anchor = AnchorStyles.Top;
            BTN_Connect.Location = new Point(138, 112);
            BTN_Connect.Name = "BTN_Connect";
            BTN_Connect.Size = new Size(75, 23);
            BTN_Connect.TabIndex = 2;
            BTN_Connect.Text = "Connect";
            BTN_Connect.UseVisualStyleBackColor = true;
            BTN_Connect.Click += BTN_Connect_Click;
            // 
            // NUD_Port
            // 
            NUD_Port.Anchor = AnchorStyles.Top;
            NUD_Port.Location = new Point(195, 74);
            NUD_Port.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            NUD_Port.Name = "NUD_Port";
            NUD_Port.Size = new Size(99, 23);
            NUD_Port.TabIndex = 10;
            // 
            // L_Data
            // 
            L_Data.AutoSize = true;
            L_Data.Location = new Point(58, 165);
            L_Data.Name = "L_Data";
            L_Data.Size = new Size(170, 15);
            L_Data.TabIndex = 6;
            L_Data.Text = "Data to print (in ezpl language)";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { printerMemoryManagerToolStripMenuItem1, printerMemoryManagerToolStripMenuItem2 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(364, 24);
            menuStrip1.TabIndex = 13;
            menuStrip1.Text = "menuStrip1";
            // 
            // printerMemoryManagerToolStripMenuItem1
            // 
            printerMemoryManagerToolStripMenuItem1.Name = "printerMemoryManagerToolStripMenuItem1";
            printerMemoryManagerToolStripMenuItem1.Size = new Size(52, 20);
            printerMemoryManagerToolStripMenuItem1.Text = "Home";
            // 
            // printerMemoryManagerToolStripMenuItem2
            // 
            printerMemoryManagerToolStripMenuItem2.Name = "printerMemoryManagerToolStripMenuItem2";
            printerMemoryManagerToolStripMenuItem2.Size = new Size(152, 20);
            printerMemoryManagerToolStripMenuItem2.Text = "Printer Memory Manager";
            printerMemoryManagerToolStripMenuItem2.Click += BTN_OpenMemoryManager_Click;
            // 
            // menuStrip2
            // 
            menuStrip2.Dock = DockStyle.None;
            menuStrip2.Items.AddRange(new ToolStripItem[] { importToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Size = new Size(237, 24);
            menuStrip2.TabIndex = 0;
            menuStrip2.Text = "menuStrip2";
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.AccessibleDescription = "Import .txt or .cmd files";
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new Size(55, 20);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.ToolTipText = "Import .txt or .cmd files";
            importToolStripMenuItem.Click += importToolStripMenuItem_Click;
            // 
            // RTB_Data
            // 
            RTB_Data.AllowDrop = true;
            RTB_Data.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RTB_Data.BorderStyle = BorderStyle.None;
            RTB_Data.Location = new Point(-1, 0);
            RTB_Data.Name = "RTB_Data";
            RTB_Data.Size = new Size(239, 180);
            RTB_Data.TabIndex = 5;
            RTB_Data.Text = "";
            RTB_Data.DragDrop += RTB_Data_DragDrop;
            // 
            // BTN_Send
            // 
            BTN_Send.Anchor = AnchorStyles.Bottom;
            BTN_Send.Location = new Point(57, 393);
            BTN_Send.Name = "BTN_Send";
            BTN_Send.Size = new Size(100, 23);
            BTN_Send.TabIndex = 7;
            BTN_Send.Text = "Send";
            BTN_Send.UseVisualStyleBackColor = true;
            BTN_Send.Click += BTN_Send_Click;
            // 
            // BTN_ConnectSend
            // 
            BTN_ConnectSend.Anchor = AnchorStyles.Bottom;
            BTN_ConnectSend.Location = new Point(177, 393);
            BTN_ConnectSend.Name = "BTN_ConnectSend";
            BTN_ConnectSend.Size = new Size(118, 23);
            BTN_ConnectSend.TabIndex = 8;
            BTN_ConnectSend.Text = "Connect and Send";
            BTN_ConnectSend.UseVisualStyleBackColor = true;
            BTN_ConnectSend.Click += BTN_ConnectSend_Click;
            // 
            // BTN_Close
            // 
            BTN_Close.Anchor = AnchorStyles.Bottom;
            BTN_Close.Location = new Point(108, 422);
            BTN_Close.Name = "BTN_Close";
            BTN_Close.Size = new Size(119, 23);
            BTN_Close.TabIndex = 9;
            BTN_Close.Text = "Close Connection";
            BTN_Close.UseVisualStyleBackColor = true;
            BTN_Close.Click += BTN_Close_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "cmd";
            openFileDialog1.Filter = "Text files (*.txt;*.cmd)|*.txt;*.cmd|All files (*.*)|*.*";
            // 
            // toolStripContainer1
            // 
            toolStripContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(RTB_Data);
            toolStripContainer1.ContentPanel.Size = new Size(237, 179);
            toolStripContainer1.Location = new Point(57, 184);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(237, 203);
            toolStripContainer1.TabIndex = 14;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(menuStrip2);
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Location = new Point(56, 183);
            panel1.Name = "panel1";
            panel1.Size = new Size(239, 205);
            panel1.TabIndex = 15;
            // 
            // toolTip1
            // 
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Location = new Point(56, 207);
            panel2.Name = "panel2";
            panel2.Size = new Size(238, 1);
            panel2.TabIndex = 16;
            // 
            // L_ConnectionStatus
            // 
            L_ConnectionStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            L_ConnectionStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            L_ConnectionStatus.ForeColor = Color.DarkRed;
            L_ConnectionStatus.Location = new Point(254, 24);
            L_ConnectionStatus.Name = "L_ConnectionStatus";
            L_ConnectionStatus.Size = new Size(98, 19);
            L_ConnectionStatus.TabIndex = 17;
            L_ConnectionStatus.Text = "Disconnected";
            L_ConnectionStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            AccessibleDescription = "Import .txt or .cmd files";
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 461);
            Controls.Add(L_ConnectionStatus);
            Controls.Add(panel2);
            Controls.Add(toolStripContainer1);
            Controls.Add(menuStrip1);
            Controls.Add(NUD_Port);
            Controls.Add(BTN_Close);
            Controls.Add(BTN_ConnectSend);
            Controls.Add(BTN_Send);
            Controls.Add(L_Data);
            Controls.Add(TB_IP);
            Controls.Add(BTN_Connect);
            Controls.Add(L_Port);
            Controls.Add(L_IP);
            Controls.Add(panel1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(380, 500);
            Name = "Form1";
            Text = "Label Printer";
            FormClosed += Form1_FormClosed;
            ((System.ComponentModel.ISupportInitialize)NUD_Port).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label L_IP;
        private Label L_Port;
        private Button BTN_Connect;
        private TextBox TB_IP;
        private RichTextBox RTB_Data;
        private Label L_Data;
        private Button BTN_Send;
        private Button BTN_ConnectSend;
        private Button BTN_Close;
        private NumericUpDown NUD_Port;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem printerMemoryManagerToolStripMenuItem1;
        private ToolStripMenuItem printerMemoryManagerToolStripMenuItem2;
        private OpenFileDialog openFileDialog1;
        private ToolStripContainer toolStripContainer1;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem importToolStripMenuItem;
        private Panel panel1;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel panel2;
        private Label L_ConnectionStatus;
    }
}
