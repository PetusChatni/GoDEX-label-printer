using System.Net.Sockets;
using System.Text;

namespace LabelPrint
{
    public class StatusChangeEventArgs : EventArgs
    {
        private bool isConnected;
        private string reason;

        public StatusChangeEventArgs(bool isConnected, string reason)
        {
            this.isConnected = isConnected;
            this.reason = reason;
        }

        public bool IsConnected { get { return isConnected; } }
        public string Reason { get { return reason; } }
    }

    internal class SocketReadReceiver
    {
        public event EventHandler<StatusChangeEventArgs>? StatusChanged;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public event Action<string> DataReceived;

        public void OnStatusChanged(StatusChangeEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Meant to test connection, but currently destroys sending & receiving in app :)
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pingInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task MonitorSocketAsync(TcpClient socket, TimeSpan pingInterval, CancellationToken token)
        {
            // Enable aggressive TCP Keep-Alive (.NET 5+)
            socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 1);     // Wait 1s before probing
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 1); // Probe every 1s
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 2); // Drop after 2 failed probes

            var buffer = new byte[1024];

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await socket.Client.ReceiveAsync(buffer, SocketFlags.None, token);

                    if (bytesRead == 0)
                    {
                        OnStatusChanged(new StatusChangeEventArgs(false, "Graceful Disconnect"));
                        break;
                    }
                    else
                    {
                        StringBuilder response = new();
                        response.Append(Encoding.Default.GetString(buffer, 0, bytesRead));

                        System.Threading.Thread.Sleep(300);

                        using (NetworkStream ns = new(socket.Client, ownsSocket: false))
                        {
                            while (ns.DataAvailable)
                            {
                                int bytesReadd = ns.Read(buffer, 0, buffer.Length);
                                response.Append(Encoding.Default.GetString(buffer, 0, bytesReadd));
                            }
                        }

                        DataReceived?.Invoke(response.ToString());                        
                        //MessageBox.Show($"SRR; 78; Received: {response.ToString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"SRR; 84; {ex.Message}");
                if (socket != null)
                    socket.Close();
                OnStatusChanged(new StatusChangeEventArgs(false, "Disconnected"));
                return;
            }
        }

        /// <summary>
        /// Sends data safely (takes into account possible asynchronous sends)
        /// </summary>
        /// <param name="data">Data to send as byte array</param>
        /// <param name="socket">Socket used to send data</param>
        public void SendData(byte[] data, ref TcpClient socket)
        {
            // Wait synchronously for other sends to finish
            _sendLock.Wait();
            try
            {
                if (socket != null && socket.Client != null)
                {
                    socket.Client.Send(data);
                    //MessageBox.Show($"SRR; 106; Sent: {Encoding.Default.GetString(data)}");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"SRR; 111; {ex.Message}");
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Dispose()
        {
            _sendLock.Dispose(); // Releases OS handles created by SemaphoreSlim
        }
    }
}
