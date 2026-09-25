using System.Text;

namespace LabelPrint
{
    public partial class Form2 : Form
    {
        private Form1 parent;
        private Form3 form3;

        private string filename;
        private byte[] imageBytes;

        private bool isReceivingData = false;
        // Used when Form2 is hidden and Form1 is shown
        private bool isConnected;
        private bool updateDGVWithPrinterOutput = false;
        private ExpectedReceivedInfoType expectedReceive = ExpectedReceivedInfoType.None;
        
        private CancellationTokenSource ctk;

        public event EventHandler<byte[]> SendData;

        public Form2(Form1 form1)
        {
            InitializeComponent();

            parent = form1;
            ctk = new();
        }

        private void OnSendData(byte[] commandToSend)
        {
            SendData?.Invoke(this, commandToSend);
        }

        #region Event Subsctiption Functions
        /// <summary>
        /// Updates file list in DGV_Files
        /// </summary>
        private void UpdateFiles()
        {
            isReceivingData = true;
            expectedReceive = ExpectedReceivedInfoType.FileList;
            updateDGVWithPrinterOutput = true;

            CommunicateWithPrinter(Encoding.Default.GetBytes("~MDIR\r\n"));

            ctk = new();
            Task.Run(async () =>
            {
                try
                {
                    await TimeoutAfterUpload(7500, false, ctk.Token);
                }
                catch { }
            }, ctk.Token);
        }

        /// <summary>
        /// Processes a list of files from printer as a string
        /// </summary>
        /// <param name="printerOutput">list of files from printer</param>
        public void UpdateFiles(string printerOutput)
        {
            if (!isReceivingData)
            {
                return;
            }

            switch (expectedReceive)
            {
                case ExpectedReceivedInfoType.FileList:
                    SocketCommunicationTranslator.ProcessFiles(printerOutput, ref DGV_Files, true, new Action(
                    () =>
                    {
                        DGV_Files.ColumnHeadersVisible = true;
                        DGV_Files.Rows.Clear();
                    }), new Action<string[]>((string[] data) =>
                    {
                        DataGridViewCellStyle dataGridViewCellStyle2 = new();
                        dataGridViewCellStyle2.Padding = new Padding(100000, 0, 0, 0);

                        DGV_Files.Rows.Add(new object[] { data[0], data[1], data[2], new Button() });

                        if (DGV_Files != null && data[1].Replace(" ", "") != "IMG")
                        {
                            DGV_Files.Rows[DGV_Files.Rows.Count - 1].Cells[3].Style = dataGridViewCellStyle2;
                        }
                    }));

                    expectedReceive = ExpectedReceivedInfoType.StatusInfo;

                    return;
                case ExpectedReceivedInfoType.CheckFileList:
                    updateDGVWithPrinterOutput = false;

                    if (SocketCommunicationTranslator.ProcessFiles(printerOutput, ref DGV_Files)?.ContainsFilename(filename) == true)
                    {
                        // Asks user whether they would like to delete the existing file, and does so if user agrees
                        if (MessageBox.Show($"File with name: {filename} already exists. Would you like to delete the existing file?",
                        "Filename conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            CommunicateWithPrinter(Encoding.Default.GetBytes($"~MDELG,{filename}\r\n"));
                        }
                        else
                        {
                            isReceivingData = false;
                            return;
                        }
                    }

                    expectedReceive = ExpectedReceivedInfoType.StatusInfo;
                    if (printerOutput.EndsWith("KB free\r\n"))
                        UpdateFiles(printerOutput);

                    return;
                case ExpectedReceivedInfoType.StatusInfo:
                    if (printerOutput.EndsWith("KB free\r\n"))
                    {
                        if (updateDGVWithPrinterOutput)
                        {
                            if (L_MemoryLeft.InvokeRequired)
                                L_MemoryLeft.BeginInvoke(new Action (() => { L_MemoryLeft.Text = printerOutput; }));
                            else
                                L_MemoryLeft.Text = printerOutput;

                            isReceivingData = false;
                            updateDGVWithPrinterOutput = false;
                            ctk.Cancel();
                            return;
                        }

                        // Gets free memory amount in B
                        int bytesRemaining = -1;

                        string[] split = printerOutput.Split(" ");

                        foreach (string el in split)
                        {
                            if (bytesRemaining == -1 && int.TryParse(el, out bytesRemaining)) { }
                            else if (el.EndsWith("B"))
                            {
                                bytesRemaining *= el == "MB" ? 1000000 : el == "KB" ? 1000 : 1;
                                break;
                            }
                        }

                        if (imageBytes.Length <= bytesRemaining)
                        {
                            // Sends a bonus ~S,CHECK = waits for the printer to finish uploading image
                            byte[] idk = SocketCommunicationTranslator.ConvertImageToPrinterCommand(filename, imageBytes);

                            byte[] added = Encoding.Default.GetBytes("\r\n~S,CHECK\r\n");

                            byte[] command = new byte[idk.Length + added.Length];
                            Buffer.BlockCopy(idk, 0, command, 0, idk.Length);
                            Buffer.BlockCopy(added, 0, command, idk.Length, added.Length);

                            CommunicateWithPrinter(command);

                            ctk = new();
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await TimeoutAfterUpload(15000, true, ctk.Token);
                                }
                                catch {}
                            }, ctk.Token);

                            return;
                        }

                        MessageBox.Show("Image disk size exceeds printer's free memory.", "Image disk size error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (printerOutput == "00\r\n")
                    {
                        ctk.Cancel();

                        // Closes Form3
                        CloseForm3();

                        UpdateFiles();
                        return;
                    }

                    break;
            }

            isReceivingData = false;
        }

        /// <summary>
        /// DEBUG ONLY
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceCtrl(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                var isCtrl = char.IsControl(c);
                var n = char.ConvertToUtf32(s, i);

                if (isCtrl)
                {
                    sb.Append($"\\u{n:X4}");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Processes data from Form3; if correct, uploades image to the printer; 
        /// if file with provided filename aready exists, deletes the file with user's permision
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFilesEventHandler(object sender, EventArgs e)
        {
            if (isReceivingData)
                return;

            // Gets and checks data from Form3
            string filename = form3.FileName;

            if (filename == null || filename == "")
                return;

            this.filename = filename;

            // mm -> px
            // px = (mm / 25.4) * dpi

            float[] size = form3.MIRV_PreviewImage.GetImageScaleMm();

            float width = (size[0] / 25.4f) * form3.MIRV_PreviewImage.Image.HorizontalResolution;
            float height = (size[1] / 25.4f) * form3.MIRV_PreviewImage.Image.VerticalResolution;

            imageBytes = SocketCommunicationTranslator.ConvertImageIntoByteArray(form3.MIRV_PreviewImage.Image, width, height);

            if (imageBytes == null || imageBytes.Length < 1)
            {
                MessageBox.Show("Error occured when trying to translate image into bytes.", "Translation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (imageBytes.Length > 524288)
            {
                MessageBox.Show($"Image disk size is larger than 512 kB. {imageBytes.Length}", "Image disk size (kB) error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isReceivingData = true;
            expectedReceive = ExpectedReceivedInfoType.CheckFileList;
            CommunicateWithPrinter(Encoding.Default.GetBytes("~MDIR\r\n"));
        }

        /// <summary>
        /// Wrapper for SocketCommunicationHandler.CommunicateWithRemote()
        /// </summary>
        /// <param name="commandToSend">Command / Data that will be sent to the printer</param>
        /// <param name="shouldReceiveData">Whether app should wait for data from printer or not</param>
        /// <returns>Whatever printer returned</returns>
        private void CommunicateWithPrinter(byte[] commandToSend)
        {
            OnSendData(commandToSend);
        }
        #endregion

        #region Event Subscription functions
        /// <summary>
        /// Updates files if socket is free
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_Retrieve_Click(object sender, EventArgs e)
        {
            if (!isReceivingData && parent.PrinterSocket.Connected)
                UpdateFiles();
        }

        /// <summary>
        /// Opens Form3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_AddImg_Click(object sender, EventArgs e)
        {
            if (!parent.PrinterSocket.Connected || isReceivingData)
                return;

            if (form3 == null)
                form3 = new Form3();
                form3.FormSubmitted += UpdateFilesEventHandler;

            form3.ShowDialog();
        }
        
        /// <summary>
        /// Closes socket and app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (parent.PrinterSocket != null)
                parent.PrinterSocket.Close();

            System.Environment.Exit(1);
        }

        /// <summary>
        /// Checks if "Delete" was clicked button, if so, deletes the file at button's row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV_Files_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!parent.PrinterSocket.Connected || isReceivingData)
                return;

            // Checks if the button was clicked
            if (e.RowIndex < 0 || DGV_Files.Columns[e.ColumnIndex].Name != "btnColumn" || parent.PrinterSocket == null)
                return;

            // Tries to get the filename
            string? filename = DGV_Files.Rows[e.RowIndex].Cells[0].Value.ToString();

            if (filename == null || filename == "")
                return;

            // Promts user to confirm deletion of the file & deletes it from DGV_Files & printer
            if (MessageBox.Show($"Do you want to delete file: {filename}?", "Confirm deleting",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            DGV_Files.Rows.RemoveAt(e.RowIndex);

            CommunicateWithPrinter(Encoding.Default.GetBytes($"~MDELG,{filename}\r\n"));
            UpdateFiles();
        }

        /// <summary>
        /// Hides Form2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isReceivingData)
                return;

            isConnected = parent.PrinterSocket == null ? false : parent.PrinterSocket.Connected;
            Visible = false;
        }

        /// <summary>
        /// Changes L_ConnectionStatus text & color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void ConnectionStatusChanged(object? sender, StatusChangeEventArgs e)
        {
            // Check if the current thread is NOT the main UI thread
            if (L_ConnectionStatus.InvokeRequired)
            {
                // Safely marshal the execution to the UI thread
                L_ConnectionStatus.BeginInvoke(new Action(() =>
                {
                    L_ConnectionStatus.Text = e.IsConnected ? "Connected" : "Disconnected";
                    L_ConnectionStatus.ForeColor = e.IsConnected ? Color.ForestGreen : Color.DarkRed;
                    if (!e.IsConnected) 
                    {
                        isReceivingData = false;
                        CloseForm3();
                    }
                }));
            }
            else
            {
                // Direct update if already on UI thread
                L_ConnectionStatus.Text = e.IsConnected ? "Connected" : "Disconnected";
                L_ConnectionStatus.ForeColor = e.IsConnected ? Color.ForestGreen : Color.DarkRed;
                if (!e.IsConnected) 
                { 
                    isReceivingData = false;
                    CloseForm3();
                }
            }
        }
        #endregion

        public void CloseForm3()
        {
            if (form3 == null)
                return;

            if (form3.InvokeRequired)
            {
                form3.BeginInvoke(new Action(() =>
                {
                    form3.FormSubmitted -= UpdateFilesEventHandler;
                    form3.Close();
                }));
            }
            else
            {
                form3.FormSubmitted -= UpdateFilesEventHandler;
                form3.Close();
            }
        }

        public async Task TimeoutAfterUpload(int milisecDelay, bool calledFromUpload, CancellationToken ct)
        {
            await Task.Delay(milisecDelay);

            if (!ct.IsCancellationRequested && calledFromUpload)
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => { UpdateFiles("00\r\n"); }));
                }
                else
                {
                    UpdateFiles("00\r\n");
                }
            else if (!ct.IsCancellationRequested)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(new Action(() => { isReceivingData = false; }));
                else
                    isReceivingData = false;
            }
        }

        public bool IsConnected { get { return isConnected; } }
    }
}
