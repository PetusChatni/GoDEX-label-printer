using System.Net;
using System.Net.Sockets;

namespace LabelPrint
{
    public class ConnectedEventArgs : EventArgs
    {
        private IPAddress? printerIP;
        private int printerPort;
        private TcpClient printerSocket;

        public ConnectedEventArgs() { }
        public ConnectedEventArgs(IPAddress? printerIP, int printerPort, TcpClient printerSocket)
        {
            this.printerIP = printerIP;
            this.printerPort = printerPort;
            this.printerSocket = printerSocket;
        }

        public IPAddress? PrinterIP { get { return printerIP; } }
        public int PrinterPort { get { return printerPort; } }
        public TcpClient PrinterSocket { get { return printerSocket; } }
    }

    public class CreatedForm2EventArgs : EventArgs 
    {
        private Form2 form2;

        public CreatedForm2EventArgs(ref Form2 form2)
        {
            this.form2 = form2;
        }

        public ref Form2 Form2 { get { return ref form2; } }
    }

    public class RequestMonitoringStateChangeEventArgs : EventArgs 
    {
        private bool resumeMonitoring;
        private TcpClient socket;

        public RequestMonitoringStateChangeEventArgs(bool resumeMonitoring, ref TcpClient socket)
        {
            this.resumeMonitoring = resumeMonitoring;
            this.socket = socket;
        }

        public bool ResumeMonitoring { get { return resumeMonitoring; } }
        public ref TcpClient Socket { get { return ref socket; } }
    }

    public class ImageSizeUpdatedEventArgs : EventArgs
    {
        private long size;

        public ImageSizeUpdatedEventArgs(long size)
        {
            this.size = size;
        }

        public long Size { get { return size; } }
    }

    public enum ExpectedReceivedInfoType 
    {
        None,
        FileList,
        CheckFileList,
        StatusInfo
    }

    internal class ProgramRuntime
    {
        private Form1 form1;
        private SocketReadReceiver socketReadReceiver;
        private CancellationTokenSource cancelTokenSource;
        private readonly object _stateLock = new object();
        public bool IsMonitoring { get; private set; }

        public ProgramRuntime()
        {
            // Tries to load configuration (ip & port)
            Config? config = FileHandler.LoadConfig();

            if (config != null)
            {
                Config newConfig = (Config)config;
                form1 = new Form1(newConfig.IP, newConfig.Port);
            }
            else
            {
                form1 = new Form1();
            }

            form1.Connected += ConnectedToThePrinter;
            form1.Form2Created += Form2Created;
            form1.SendData += SendDataToPrinter;

            Application.Run(form1);
        }

        /// <summary>
        /// Function that initializes socket monitoring and binds associated events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectedToThePrinter(object? sender, ConnectedEventArgs e)
        {
            bool socketReadReceiverCreated = false;
            if (socketReadReceiver == null)
            {
                socketReadReceiver = new SocketReadReceiver();
                socketReadReceiverCreated = true;
            }

            cancelTokenSource = new CancellationTokenSource();

            if (e.PrinterSocket != null && e.PrinterSocket.Client != null)
            {
                if (socketReadReceiverCreated)
                {
                    socketReadReceiver.StatusChanged += form1.ConnectionStatusChanged;
                    socketReadReceiver.StatusChanged += MonitoringStateChanged;
                    if (form1.Form2 != null)
                        socketReadReceiver.StatusChanged += form1.Form2.ConnectionStatusChanged;
                }

                ChangeSocketMonitoring(this, new RequestMonitoringStateChangeEventArgs(true, ref form1.PrinterSocket));
            }
        }

        private void MonitoringStateChanged(object? sender, StatusChangeEventArgs e)
        {
            IsMonitoring = e.IsConnected;
        }

        /// <summary>
        /// Stops / starts socket monitoring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeSocketMonitoring(object? sender, RequestMonitoringStateChangeEventArgs e)
        {
            if (!e.ResumeMonitoring)
            {
                lock (_stateLock)
                {
                    if (!IsMonitoring) return;

                    // Signal all background loops (ReceiveAsync and Task.Delay) to cancel instantly
                    cancelTokenSource?.Cancel();
                    cancelTokenSource?.Dispose();
                    cancelTokenSource = null;

                    IsMonitoring = false;
                }
            }
            else
            {
                lock (_stateLock)
                {
                    // Prevent starting multiple monitoring tasks simultaneously
                    if (IsMonitoring) return;

                    cancelTokenSource = new CancellationTokenSource();
                    IsMonitoring = true;

                    socketReadReceiver.OnStatusChanged(new StatusChangeEventArgs(true, "standard connection"));

                    // Launch Monitor Socket Task on background thread
                    Task.Run(async () =>
                    {
                        try
                        {
                            await socketReadReceiver.MonitorSocketAsync(e.Socket, new TimeSpan(TimeSpan.TicksPerMillisecond * 200), cancelTokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            IsMonitoring = false;
                            //MessageBox.Show($"PR; 168; Exception - SRR: {ex.Message}");
                            socketReadReceiver.OnStatusChanged(new StatusChangeEventArgs(false, "Disconnected"));
                        }
                    }, cancelTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// Binds functions to events in Form2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2Created(object? sender, CreatedForm2EventArgs e)
        {
            if (form1.Form2 != null)
            {
                form1.Form2.SendData += SendDataToPrinter;
                socketReadReceiver.DataReceived += form1.Form2.UpdateFiles;

                if (socketReadReceiver != null)
                {
                    socketReadReceiver.StatusChanged += form1.Form2.ConnectionStatusChanged;
                }
            }
        }

        private void SendDataToPrinter(object? sender, byte[] e)
        {
            socketReadReceiver.SendData(e, ref form1.PrinterSocket);
        }
    }
}
