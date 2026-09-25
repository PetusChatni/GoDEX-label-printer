using System.Net;
using System.Net.Sockets;

namespace LabelPrint
{
    public partial class Form1 : Form
    {
        private TcpClient printerSocket;

        private IPAddress? printerIP;
        private int printerPort;
        private string data;

        public event EventHandler<ConnectedEventArgs>? Connected;
        public event EventHandler<CreatedForm2EventArgs>? Form2Created;
        public event EventHandler<byte[]> SendData;

        private Form2 form2;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string ip, int port)
        {
            InitializeComponent();

            if (FieldChecker.CheckIPInput(ip, out printerIP))
                TB_IP.Text = ip;

            if (FieldChecker.CheckPort(NUD_Port.Value, out printerPort))
                NUD_Port.Value = (decimal)port;
        }

        private void OnConnected()
        {
            Connected?.Invoke(this, new ConnectedEventArgs(printerIP, printerPort, printerSocket));
        }

        private void OnForm2Created()
        {
            Form2Created?.Invoke(this, new CreatedForm2EventArgs(ref form2));
        }

        private void OnSendData(byte[] commandToSend)
        {
            SendData?.Invoke(this, commandToSend);
        }

        #region Wrapper functions for SocketCommunnnicationHandler
        /// <summary>
        /// Tries to establish connection with a printer
        /// using ip address and port provided by the user
        /// </summary>
        /// <param name="printSuccessMsg">If true and connection <b>is</b> established, message will be displayed</param>
        /// <param name="printFailMsg">If true and connection <b>isn't</b> established, message will be displayed</param>
        private void ConnectToPrinter(bool printSuccessMsg = false, bool printFailMsg = true)
        {
            // Checks if printer is already connected to the PC
            if (printerSocket != null && printerSocket.Connected)
            {
                return;
            }

            // Checks IP & Port inputs
            if (!FieldChecker.CheckIPInput(TB_IP.Text, out printerIP) |
                !FieldChecker.CheckPort(NUD_Port.Value, out printerPort))
            {
                return;
            }

            // Connects to the printer
            if (SocketCommunicationHandler.Connect(ref printerSocket, printerIP, printerPort, true, printSuccessMsg, printFailMsg))
            {
                OnConnected();
            }
        }

        /// <summary>
        /// Sends EZPL Data provided by user to the printer
        /// </summary>
        private void SendDataToPrinter(bool showConnectivityWarning = true)
        {
            // Checks if PC is connected to the printer
            if (printerSocket == null || !printerSocket.Connected)
            {
                if (showConnectivityWarning)
                    MessageBox.Show("Printer isn't connected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Checks if data are entered
            if (!FieldChecker.CheckData(RTB_Data.Text, out data))
                return;

            OnSendData(SocketCommunicationTranslator.ConvertDataIntoCommand(data.Replace(" ", "")));
        }
        #endregion

        #region Event Subscription functions
        private void BTN_Connect_Click(object sender, EventArgs e)
        {
            ConnectToPrinter(true);
        }
        private void BTN_Send_Click(object sender, EventArgs e)
        {
            SendDataToPrinter();
        }

        private void BTN_ConnectSend_Click(object sender, EventArgs e)
        {
            ConnectToPrinter();

            SendDataToPrinter(false);
        }

        /// <summary>
        /// Closes socket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_Close_Click(object sender, EventArgs e)
        {
            if (printerSocket != null)
            {
                ConnectionStatusChanged(this, new StatusChangeEventArgs(false, "standard disconnect"));
                printerSocket.Close();
            }
        }

        /// <summary>
        /// Closes socket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (printerSocket != null)
                printerSocket.Close();

            System.Environment.Exit(1);
        }

        /// <summary>
        /// Opens Form2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_OpenMemoryManager_Click(object sender, EventArgs e)
        {
            if (printerSocket == null || !printerSocket.Connected)
            {
                MessageBox.Show("Firstly, connect to the printer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Creates form if it's null
            if (form2 == null)
            {
                form2 = new Form2(this);
                OnForm2Created();
            }

            form2.Show();
            form2.VisibleChanged += Loaded;
            Hide();
        }

        /// <summary>
        /// Shows Form1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Loaded(object sender, EventArgs e)
        {
            if (form2 == null || form2.Visible)
                return;

            Show();
            try
            {
                if ((printerSocket == null || !printerSocket.Connected) && form2.IsConnected)
                    ConnectToPrinter(false, false);
            }
            catch { }
        }

        /// <summary>
        /// Reads file into RTB_Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RTB_Data_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null || e.Data.GetData(DataFormats.FileDrop) == null)
                return;

            foreach (string el in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                // Path is valid & files extension is supported -> File contents
                // are written into RTB_Data
                if (File.Exists(el) && FileHandler.ReadFileToRTB(el, ref RTB_Data))
                {
                    break;
                }
                else if (File.Exists(el))
                {
                    MessageBox.Show("File has unsupported extension.\nUse files with extensions like TXT or CMD.", "Extension error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Reads file from OpenFileDialog into RTB_Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                FileHandler.ReadFileToRTB(openFileDialog1.FileName, ref RTB_Data);
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

        public ref Form2 Form2 { get { return ref form2; } }
        public ref TcpClient PrinterSocket { get { return ref printerSocket; } }
    }
}
