using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabelPrint
{
    public partial class Form2 : Form
    {
        private Form3 form3;

        //private TcpClient printerSocket;
        private Form1 parent;
        private SocketCommunicationHandler socketComHandler;

        private IPAddress? printerIP;
        private int printerPort;

        private string path;
        private string filename;
        private byte[] imageBytes;

        private bool isReceivingData = false;
        private bool isConnected;
        private bool updateDGVWithPrinterOutput = false;

        public event EventHandler<RequestMonitoringStateChangeEventArgs>? RequestedMonitoringStateChange;
        public event EventHandler<byte[]> SendData;

        public Form2(Form1 form1, IPAddress? printerIP, int printerPort)
        {
            InitializeComponent();

            this.printerIP = printerIP;
            this.printerPort = printerPort;
            socketComHandler = new SocketCommunicationHandler(printerIP, printerPort);
            parent = form1;
        }

        private void OnRequestMonitoringStateChange(bool newMonitoringState)
        {
            RequestedMonitoringStateChange?.Invoke(this, new RequestMonitoringStateChangeEventArgs(newMonitoringState, ref parent.PrinterSocket));
        }

        private void OnSendData(byte[] commandToSend)
        {
            //MessageBox.Show("Form2.cs; 46; Invoked SendData");
            SendData?.Invoke(this, commandToSend);
        }

        #region Wrapper functions for SocketCommunication
        /// <summary>
        /// Updates file list in DGV_Files
        /// </summary>
        private void UpdateFiles()
        {
            isReceivingData = true;

            //MessageBox.Show("Form2.cs; 57; In UpdateFiles()");

            //SocketCommunicationTranslator.ProcessFiles(), ref DGV_Files, true);
            updateDGVWithPrinterOutput = true;
            CommunicateWithPrinter(Encoding.Default.GetBytes("~MDIR\r\n"));
        }

        public void UpdateFiles(string printerOutput)
        {
            //MessageBox.Show("Form2.cs; 67; In UpdateFiles(string)");

            if (isReceivingData && updateDGVWithPrinterOutput)
            {
                //MessageBox.Show("Form2.cs; 68; Here");
                SocketCommunicationTranslator.ProcessFiles(printerOutput, ref DGV_Files, true, new Action(
                    () =>
                    {
                        DGV_Files.ColumnHeadersVisible = true;
                        DGV_Files.Rows.Clear();
                    }), new Action<string[]>((string[] data) =>
                    {
                        DGV_Files.Rows.Add(new object[] { data[0], data[1], data[2], new Button() });
                    }));
            }
            else if (isReceivingData)
            {
                //MessageBox.Show("Form2.cs; 85; Receiving check data");
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

                //MessageBox.Show("Form2.cs; 85; Sending image ... ");
                CommunicateWithPrinter(socketComHandler.AddImageToPrinter(ref parent.PrinterSocket, path, filename, imageBytes));

                // Closes Form3
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

            isReceivingData = false;
        }

        /// <summary>
        /// Processes data from Form3; if correct, uploades image to the printer; 
        /// if file with provided filename aready exists, deletes the file with user's permision
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFilesEventHandler(object sender, EventArgs e)
        {
            // Gets and checks data from Form3
            string path = form3.Path;
            string filename = form3.FileName;

            if (filename == null || filename == "")
                return;

            this.filename = filename;
            this.path = path;

            try
            {
                // Enable resizing
                using (Bitmap original = new Bitmap(form3.MIRV_PreviewImage.Image))
                {
                    // Converts source image to 1-bit monochrome indexed bitmap
                    using (Bitmap monoImg = original.Clone(
                        new Rectangle(0, 0, original.Width, original.Height),
                        PixelFormat.Format1bppIndexed))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Returns full 1-bit BMP byte array with 54-byte header
                            monoImg.SetResolution(203, 203);

                            int translatedWidth = (int)((form3.MIRV_PreviewImage.MmWidth / 25.4f) * monoImg.HorizontalResolution);
                            int translatedHeight = (int)((form3.MIRV_PreviewImage.MmHeight / 25.4f) * monoImg.VerticalResolution);

                            // Create blank canvas
                            Bitmap resizedImg = new Bitmap(translatedWidth, translatedHeight);
                            Graphics gfx = Graphics.FromImage(resizedImg);

                            gfx.DrawImage(monoImg, 0, 0, translatedWidth, translatedHeight);

                            Bitmap send = resizedImg.Clone(new Rectangle(0, 0, resizedImg.Width, resizedImg.Height),
                                PixelFormat.Format1bppIndexed);

                            send.Save(ms, ImageFormat.Bmp);
                            imageBytes = ms.ToArray();

                            resizedImg.Dispose();
                            send.Dispose();
                        }
                    }
                }
            }
            catch { }
            //catch (Exception ex) { MessageBox.Show($"Form2.cs; 152; {ex.Message}"); }

            if (imageBytes == null || imageBytes.Length < 1)
            {
                MessageBox.Show("Error occured when trying to translate image into bytes.", "Translation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (imageBytes.Length > 512000)
            {
                MessageBox.Show($"Image disk size is larger than 512 kB. {imageBytes.Length}", "Image disk size (kB) error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isReceivingData = true;
            updateDGVWithPrinterOutput = false;
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
            if (!parent.PrinterSocket.Connected)
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
            if (!parent.PrinterSocket.Connected)
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
        }

        /// <summary>
        /// Hides Form2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            //if (!e.IsConnected)
                //MessageBox.Show("Disconnected");

            // Check if the current thread is NOT the main UI thread
            if (L_ConnectionStatus.InvokeRequired)
            {
                // Safely marshal the execution to the UI thread
                L_ConnectionStatus.BeginInvoke(new Action(() =>
                {
                    L_ConnectionStatus.Text = e.IsConnected ? "Connected" : "Disconnected";
                    L_ConnectionStatus.ForeColor = e.IsConnected ? Color.ForestGreen : Color.DarkRed;
                }));
            }
            else
            {
                // Direct update if already on UI thread
                L_ConnectionStatus.Text = e.IsConnected ? "Connected" : "Disconnected";
                L_ConnectionStatus.ForeColor = e.IsConnected ? Color.ForestGreen : Color.DarkRed;
            }
        }
        #endregion

        public bool IsConnected { get { return isConnected; } }
    }
}
