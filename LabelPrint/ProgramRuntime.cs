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

    internal class ProgramRuntime
    {
        private Form1 form1;
        private SocketReadReceiver socketReadReceiver;
        private CancellationTokenSource cancelTokenSource;
        private readonly object _stateLock = new object();
        public bool IsMonitoring { get; private set; }

        // Routes Received data to specific funcitons - because there's only one, it isn't used
        // private string ReceivedDataRecepient;

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
            form1.RequestedMonitoringStateChange += ChangeSocketMonitoring;
            form1.SendData += SendDataToPrinter;

            //Application.Run(form1);
            
            // DEBUG ONLY
            Application.Run(new Form3());
        }

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

                MessageBox.Show("PR; 107; Changing Monitoring State ... ");

                ChangeSocketMonitoring(this, new RequestMonitoringStateChangeEventArgs(true, ref form1.PrinterSocket));

                // Offloads the async loop to a background thread without blocking
                //Task.Run(async () =>
                //{
                //    try
                //    {
                //        await socketReadReceiver.MonitorSocketAsync(e.PrinterSocket.Client, new TimeSpan(TimeSpan.TicksPerMillisecond*200), cancelTokenSource.Token);
                //    }
                //    catch (Exception ex)
                //    {
                //        MessageBox.Show($"Exeption - sRR: {ex.Message}");
                //        socketReadReceiver.OnStatusChanged(new StatusChangeEventArgs(false, "Disconnected"));

                //        // Log or handle any unhandled exceptions from the background task
                //        //Console.WriteLine($"Background socket monitor fault: {ex.Message}");
                //    }
                //});
            }
        }

        private void MonitoringStateChanged(object? sender, StatusChangeEventArgs e)
        {
            IsMonitoring = e.IsConnected;
        }

        private void ChangeSocketMonitoring(object? sender, RequestMonitoringStateChangeEventArgs e)
        {
            if (!e.ResumeMonitoring)
            {
                MessageBox.Show("Canceled");
                
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
                MessageBox.Show("Resumed");

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
                            MessageBox.Show($"PR; 163; Exception - SRR: {ex.Message}");
                            socketReadReceiver.OnStatusChanged(new StatusChangeEventArgs(false, "Disconnected"));
                        }
                    }, cancelTokenSource.Token);
                }
            }
        }

        private void Form2Created(object? sender, CreatedForm2EventArgs e)
        {
            if (form1.Form2 != null)
            {
                //form1.Form2.RequestedMonitoringStateChange += ChangeSocketMonitoring;
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
