using System;
using System.IO;
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

    public class ReadDataEventArgs : EventArgs
    {
        private byte[] data;

        public ReadDataEventArgs(byte[] data)
        {
            this.data = data;
        }

        public byte[] Data { get { return data; } }
    }

    internal class SocketReadReceiver
    {
        public event EventHandler<StatusChangeEventArgs>? StatusChanged;
        //public event EventHandler<ReadDataEventArgs>? ReadData;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

        public event Action<string> DataReceived;

        public void OnStatusChanged(StatusChangeEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        // Image upload isn't working still - suspecting two send & receive fns firing
        /// <summary>
        /// Meant to test connection, but currently destroys sending & receiving in app :)
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="pingInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task MonitorSocketAsync(TcpClient socket, TimeSpan pingInterval, CancellationToken token)
        {
            //MessageBox.Show("Launched");

            // Enable aggressive TCP Keep-Alive (.NET 5+)
            socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 1);     // Wait 1s before probing
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 1); // Probe every 1s
            socket.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 2); // Drop after 2 failed probes

            var buffer = new byte[1024];

            try
            {
                //byte[] heartbeatPayload = new byte[] { 0x00 };

                while (!token.IsCancellationRequested)
                {
                    //MessageBox.Show("Running");
                    //await Task.Delay(pingInterval, token);

                    //// Sending to a powered-off device fails once TCP retransmissions expire
                    //await socket.SendAsync(heartbeatPayload, SocketFlags.None, token);

                    int bytesRead = await socket.Client.ReceiveAsync(buffer, SocketFlags.None, token);

                    //MessageBox.Show("SRR; 82; Continue travajon");

                    if (bytesRead == 0)
                    {
                        //MessageBox.Show("0 bytes");
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

                        //bool isFirstLoop = true;

                        //while (socket.Available > 0 || isFirstLoop)
                        //{
                        //    bytesRead = await socket.GetStream(buffer, SocketFlags.None);
                        //    response.Append(Encoding.Default.GetString(buffer, 0, bytesRead));

                        //    isFirstLoop = false;
                        //}

                        DataReceived?.Invoke(response.ToString());                        
                        //MessageBox.Show($"Received + SRR: {response.ToString()}");
                    }

                    // Copy received bytes and publish to the app
                    //byte[] receivedData = new byte[bytesRead];
                    //Array.Copy(buffer, receivedData, bytesRead);

                    // Exclude heartbeat bytes (e.g., 0x00) if applicable, then trigger event
                    

                    //// Pending read completes instantly when the OS detects a state change
                    //int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);

                    //MessageBox.Show("Received");

                    //if (bytesRead == 0)
                    //{
                    //    OnStatusChanged(new StatusChangeEventArgs(false, "Graceful Disconnect"));
                    //    break;
                    //}

                    //StringBuilder response = new();
                    //response.Append(Encoding.Default.GetString(buffer, 0, bytesRead));

                    // Process data...
                    // Change to some other fn (idk which exactly)
                    //using (NetworkStream stream = socket.GetStream())
                    //{
                    //    while (stream.DataAvailable)
                    //    {
                    //        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    //        response.Append(Encoding.Default.GetString(buffer, 0, bytesRead));
                    //    }
                    //}


                    //OnReadData(new ReadDataEventArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SRR; 158; {ex.Message}");
                if (socket != null)
                    socket.Close();
                OnStatusChanged(new StatusChangeEventArgs(false, "Disconnected"));
                return;
            }
            //catch (SocketException ex)
            //{
            //    // Triggered immediately by Keep-Alive failure, reset, or network drop
            //    //MessageBox.Show("Status Changed");
            //    OnStatusChanged(new StatusChangeEventArgs(false, ex.SocketErrorCode.ToString()));
            //}
            //catch (OperationCanceledException ex)
            //{
            //    // Normal shutdown
            //    OnStatusChanged(new StatusChangeEventArgs(false, ex.Message));
            //}
        }

        public void SendData(byte[] data, ref TcpClient socket)
        {
            // Wait synchronously for the heartbeat or other sends to finish
            _sendLock.Wait();
            try
            {
                if (socket != null && socket.Client != null)
                {
                    socket.Client.Send(data);
                    //MessageBox.Show($"SRR; 183; Sent: {Encoding.Default.GetString(data)}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SRR; 191; {ex.Message}");
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
