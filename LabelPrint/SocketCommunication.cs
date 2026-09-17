using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabelPrint
{
    struct FileList
    {
        private List<List<string>> files = new List<List<string>>();

        public FileList(){}

        /// <summary>
        /// Checks if file list contains a file with provided filename
        /// </summary>
        /// <param name="fileList">Searched list</param>
        /// <param name="filename">Searched filename</param>
        /// <returns></returns>
        public bool ContainsFilename(string filename)
        {
            if (files == null)
                return false;

            foreach (List<string> file in files)
            {
                if (file.Count > 0 && file[0] == filename)
                    return true;
            }

            return false;
        }

        public List<List<string>> Files
        { get { return files; } set { files = value; } }
    }

    internal static class SocketCommunicationTranslator
    {
        /// <summary>
        /// Processes output of command "~MDIR" sent to the printer
        /// </summary>
        /// <param name="printerOutput">Output of the command "~MDIR" that has been sen to the printer</param>
        /// <param name="updateDataGridView">If true, DGV_Files will be updated</param>
        /// <returns>List of files: [0] = filename, [1] = type, [2] = size</returns>
        public static FileList? ProcessFiles(string printerOutput, ref DataGridView? DGV_Files, bool updateDataGridView = false)
        {
            // Checks if output is valid
            if (printerOutput == null || printerOutput == "" || !printerOutput.Contains("\r\n") || printerOutput.IndexOf("\r\n") == printerOutput.LastIndexOf("\r\n"))
                return null;

            // Cleans output and splits it to strings about files
            string cleanOutput = printerOutput.Substring(printerOutput.IndexOf("\r\n") + 2);
            cleanOutput = cleanOutput.Substring(0, cleanOutput.LastIndexOf("\r\n"));

            string[] readyArr = cleanOutput.Split("\r\n");

            FileList returns = new FileList();

            if (updateDataGridView && DGV_Files != null)
            {
                // Prepares DGV_Files for new data
                DGV_Files.ColumnHeadersVisible = true;

                DGV_Files.Rows.Clear();
            }

            foreach (string file in readyArr)
            {
                // Extracts data about file
                List<string> data = file.Split("  ").ToList();
                data.ForEach(el => el.Replace(" ", ""));
                data.RemoveAll(el => el == "");

                if (data.Count() != 3)
                    continue;

                // Adds file to file list and (if it should) DGV_Files
                returns.Files.Add(data);

                if (updateDataGridView)
                {
                    DGV_Files?.Rows.Add(new object[] { data[0], data[1], data[2], new Button() });
                }
            }

            return returns;
        }
    }

    internal class SocketCommunicationHandler
    {
        private IPAddress ip;
        private int port;

        public SocketCommunicationHandler(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        /// <summary>
        /// Connects to a remote socket
        /// </summary>
        /// <param name="socket">Socket variable</param>
        /// <param name="ip">IP of the remote socket</param>
        /// <param name="port">Port of the remote socket</param>
        /// <param name="saveConfig">Will save configuration into JSON file (if connection is established)</param>
        /// <param name="printSuccessMsg">If true & connection <b>is</b> established, message will be displayed</param>
        /// <param name="printFailMsg">If true & connection <b>isn't</b> established, message will be displayed</param>
        /// <returns></returns>
        public static bool Connect(ref TcpClient socket, IPAddress ip, int port, bool saveConfig = false, bool printSuccessMsg = false, bool printFailMsg = true)
        {
            // Checks if printer is already connected to the PC
            if (socket != null && socket.Connected)
                return false;

            // Connects to the printer
            if(socket != null)
                socket.Close();

            socket = new TcpClient();
            try
            {
                IAsyncResult result = socket.BeginConnect(ip, port, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                if (socket.Connected)
                {
                    socket.EndConnect(result);
                }
                else
                {
                    // NOTE, MUST CLOSE THE SOCKET
                    socket.Close();
                    throw new ApplicationException("Connection failed");
                }
            }
            catch
            {
                if (printFailMsg)
                    MessageBox.Show("Connection failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (printSuccessMsg)
                MessageBox.Show("Connection succeeded", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (saveConfig)
            {
                Config config = new Config();

                config.IP = ip.ToString();
                config.Port = port;
                
                FileHandler.SaveConfig(config);
            }

            return true;
        }

        #region Communication
        /// <summary>
        /// Sends a byte array message to a remote socket
        /// </summary>
        /// <param name="socket">Reference to remote socket</param>
        /// <param name="ip">Remote socket's ip</param>
        /// <param name="port">Remote socket's port</param>
        /// <param name="commandToSend">Message to send</param>
        /// <param name="shouldReceiveData">Should receive data or not</param>
        /// <returns></returns>
        public static string CommunicateWithRemote(ref TcpClient socket, IPAddress ip, int port, byte[] commandToSend, bool shouldReceiveData = false)
        {
            Connect(ref socket, ip, port);

            if (socket == null || !socket.Connected)
            {
                return "";
            }

            try
            {
                using (NetworkStream stream = socket.GetStream())
                {
                    stream.Write(commandToSend, 0, commandToSend.Length);
                    stream.Flush();

                    if (!shouldReceiveData)
                        return "";

                    System.Threading.Thread.Sleep(300);

                    StringBuilder response = new StringBuilder();
                    byte[] buffer = new byte[1024];

                    // Reads response back from the printer
                    while (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        response.Append(Encoding.Default.GetString(buffer, 0, bytesRead));
                    }

                    return response.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Connection failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        // Non-static version
        public string CommunicateWithRemote(ref TcpClient socket, byte[] commandToSend, bool shouldReceiveData = false)
        {
            return CommunicateWithRemote(ref socket, ip, port, commandToSend, shouldReceiveData);
        }

        /// <summary>
        /// Sents image to the printer
        /// </summary>
        /// <param name="socket">Printer socket</param>
        /// <param name="ip">Printers IP adress</param>
        /// <param name="port">Printers port</param>
        /// <param name="path">Path to the image on local PC</param>
        /// <param name="filename">New name for the image on the printer</param>
        /// <returns>Whether image was sent (true) or not (false)</returns>
        public static bool AddImageToPrinter(ref TcpClient socket, IPAddress ip, int port, string path, string filename)
        {
            // Translates the file into byte array
            byte[] bytes = FileHandler.GetImageBytes(path);
            if (bytes.Length < 1)
            {
                MessageBox.Show("Error occured when trying to translate image into bytes.", "Translation error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            DataGridView? fn = null;

            // Checks if file with provided filename already exists
            if (SocketCommunicationTranslator.ProcessFiles(CommunicateWithRemote(ref socket, ip, port,
                Encoding.Default.GetBytes("~MDIR\r\n"), true), ref fn)?.ContainsFilename(filename) == true)
            {
                // Asks user whether they would like to delete the existing file, and does so if user agrees
                if (MessageBox.Show($"File with name: {filename} already exists. Would you like to delete the existing file?",
                "Filename conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    CommunicateWithRemote(ref socket, ip, port, Encoding.Default.GetBytes($"~MDELG,{filename}\r\n"));
                }
                else
                {
                    return false;
                }
            }

            // Command for image upload
            byte[] commandHeader = Encoding.Default.GetBytes($"~Eb,{filename},{bytes.Length}\r\n");

            // Full byte array with command & image that will be sent to the printer
            byte[] command = new byte[commandHeader.Length + bytes.Length];
            Buffer.BlockCopy(commandHeader, 0, command, 0, commandHeader.Length);
            Buffer.BlockCopy(bytes, 0, command, commandHeader.Length, bytes.Length);

            CommunicateWithRemote(ref socket, ip, port, command);
            return true;
        }

        // Non-static version
        public bool AddImageToPrinter(ref TcpClient socket, string path, string filename)
        {
            return AddImageToPrinter(ref socket, ip, port, path, filename);
        }
        #endregion
    }
}
