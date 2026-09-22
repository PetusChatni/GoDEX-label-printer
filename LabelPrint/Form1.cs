using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabelPrint
{
    public partial class Form1 : Form
    {
        private TcpClient printerSocket;
        private SocketCommunicationHandler socketComHandler;

        private IPAddress? printerIP;
        private int printerPort;
        private string data;

        public event EventHandler<ConnectedEventArgs>? Connected;
        public event EventHandler<CreatedForm2EventArgs>? Form2Created;
        public event EventHandler<RequestMonitoringStateChangeEventArgs>? RequestedMonitoringStateChange;
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
            //ConnectionStatusChanged(this, new StatusChangeEventArgs(true, "standard connect"));

            Connected?.Invoke(this, new ConnectedEventArgs(printerIP, printerPort, printerSocket));
            //MessageBox.Show($"Form1.cs; 46; Connnected event fired");
        }

        private void OnForm2Created()
        {
            Form2Created?.Invoke(this, new CreatedForm2EventArgs(ref form2));
        }

        private void OnRequestMonitoringStateChange(bool newMonitoringState)
        {
            RequestedMonitoringStateChange?.Invoke(this, new RequestMonitoringStateChangeEventArgs(newMonitoringState, ref printerSocket));
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

            socketComHandler = new SocketCommunicationHandler(printerIP, printerPort);
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

            // Needed for printer to print labels
            string strippedData = data.Replace(" ", "");
            if (!data.EndsWith("\r\n") && !strippedData.StartsWith("~E"))
            {
                data += "\r\n";
                MessageBox.Show(data);
            }
            else if (strippedData.StartsWith("~E") && data.Split("\\r\\n").Length == 2)
            {
                byte[] header = Encoding.Default.GetBytes(data.Split("\\r\\n")[0]+"\r\n");

                string bodyHex = data.Split("\\r\\n")[1];
                
                byte[] body = new byte[bodyHex.Length / 2];
                for (int i = 0; i < bodyHex.Length; i += 2)
                    body[i / 2] = Convert.ToByte(bodyHex.Substring(i, 2), 16);

                byte[] command = new byte[header.Length + body.Length];
                Buffer.BlockCopy(header, 0, command, 0, header.Length);
                Buffer.BlockCopy(body, 0, command, header.Length, body.Length);


                OnSendData(command);
                //OnRequestMonitoringStateChange(false);
                //if (socketComHandler != null)
                //    socketComHandler.CommunicateWithRemote(ref printerSocket, command);
                //else
                //    SocketCommunicationHandler.CommunicateWithRemote(ref printerSocket, printerIP, printerPort, command);
                //OnRequestMonitoringStateChange(true);

                return;
            }

            OnSendData(Encoding.Default.GetBytes(data));

            // Sends data to the printer
            //OnRequestMonitoringStateChange(false);
            //if (socketComHandler != null)
            //    socketComHandler.CommunicateWithRemote(ref printerSocket, Encoding.Default.GetBytes(data));
            //else
            //    SocketCommunicationHandler.CommunicateWithRemote(ref printerSocket, printerIP, printerPort, Encoding.Default.GetBytes(data));
            //OnRequestMonitoringStateChange(true);
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
            // Closes printer socket
            if(printerIP != null)
                SocketCommunicationHandler.Connect(ref printerSocket, printerIP, printerPort, false, false, false);

            if (printerSocket == null || !printerSocket.Connected)
            {
                MessageBox.Show("Firstly, connect to the printer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //else
            //    printerSocket.Close();

            // Creates form if it's null
            if (form2 == null)
            {
                form2 = new Form2(this, printerIP, printerPort);
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
                    //SocketCommunicationHandler.Connect(ref printerSocket, printerIP, printerPort);
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

            //if (e.IsConnected)
            //{
            //    L_ConnectionStatus.Text = "Connected";
            //    L_ConnectionStatus.ForeColor = Color.ForestGreen;
            //}
            //else
            //{
            //    L_ConnectionStatus.Text = "Disconnected";
            //    L_ConnectionStatus.ForeColor = Color.DarkRed;
            //}
        }
        #endregion

        public ref Form2 Form2 { get { return ref form2; } }
        public ref TcpClient PrinterSocket { get { return ref printerSocket; } }
    }
}
