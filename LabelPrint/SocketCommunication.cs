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
        public static FileList? ProcessFiles(string printerOutput, ref DataGridView? DGV_Files, bool updateDataGridView = false, Action? clearAction = null, Action<string[]>? addAction = null)
        {
            // Checks if output is valid
            if (printerOutput == null || printerOutput == "" || !printerOutput.Contains("\r\n") || printerOutput.IndexOf("\r\n") == printerOutput.LastIndexOf("\r\n"))
            {
                return null;
            }

            // Cleans output and splits it to strings about files
            string cleanOutput = printerOutput.Substring(printerOutput.IndexOf("\r\n") + 2);
            cleanOutput = cleanOutput.Substring(0, cleanOutput.LastIndexOf("\r\n"));

            string[] readyArr = cleanOutput.Split("\r\n");

            FileList returns = new FileList();

            if (updateDataGridView && DGV_Files != null)
            {
                // Prepares DGV_Files for new data
                if (DGV_Files.InvokeRequired && clearAction != null)
                {
                    // Safely marshal the execution to the UI thread
                    DGV_Files.BeginInvoke(clearAction);
                }
                else
                {
                    // Direct update if already on UI thread
                    DGV_Files.ColumnHeadersVisible = true;
                    DGV_Files.Rows.Clear();
                }
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
                    if (DGV_Files?.InvokeRequired == true && addAction != null)
                    {
                        // Safely marshal the execution to the UI thread
                        DGV_Files.BeginInvoke(new Action(() => addAction(data.ToArray())));
                    }
                    else
                    {
                        DataGridViewCellStyle dataGridViewCellStyle2 = new();
                        dataGridViewCellStyle2.Padding = new Padding(100000, 0, 0, 0);

                        // Direct update if already on UI thread
                        DGV_Files?.Rows.Add(new object[] { data[0], data[1], data[2], new Button() });

                        if (DGV_Files != null && data[1].Replace(" ", "") != "IMG")
                        {
                            DGV_Files.Rows[DGV_Files.Rows.Count - 1].Cells[3].Style = dataGridViewCellStyle2;
                        }
                    }
                }
            }

            return returns;
        }

        #region Image
        /// <summary>
        /// Resizes (if sizes aren't -1) image provided and returns it as byte array
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        /// <returns>Image as a byte array</returns>
        public static byte[] ConvertImageIntoByteArray(Image image, float newWidth = -1, float newHeight = -1)
        {
            // Uses image size if new wasn't provided
            int translatedWidth = newWidth == -1 ? image.Width : (int) newWidth;
            int translatedHeight = newHeight == -1 ? image.Height : (int) newHeight;

            try
            {
                using (Bitmap original = new Bitmap(image))
                {
                    // Converts source image to 1-bit monochrome indexed bitmap
                    using (Bitmap monoImg = original.Clone(
                        new Rectangle(0, 0, original.Width, original.Height),
                        PixelFormat.Format1bppIndexed))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            monoImg.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                            // Creates blank canvas
                            Bitmap resizedImg = new Bitmap(translatedWidth, translatedHeight);
                            Graphics gfx = Graphics.FromImage(resizedImg);

                            // Resizes image
                            gfx.DrawImage(monoImg, 0, 0, translatedWidth, translatedHeight);

                            // Converts resized image to bmp & loads it into MemoryStream ms
                            Bitmap send = resizedImg.Clone(new Rectangle(0, 0, resizedImg.Width, resizedImg.Height),
                                PixelFormat.Format1bppIndexed);

                            send.Save(ms, ImageFormat.Bmp);

                            // Deletes Bitmaps from memory
                            resizedImg.Dispose();
                            send.Dispose();

                            // Returns full 1-bit BMP byte array with 54-byte header
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch { return []; }
        }

        /// <summary>
        /// Adds command header to image bytes = command understandable by printer
        /// </summary>
        /// <param name="filename">New name for the image on the printer</param>
        /// <param name="bytes">Bytes of image that will be sent</param>
        /// <returns>Printer ready command</returns>
        public static byte[] ConvertImageToPrinterCommand(string filename, byte[] bytes)
        {
            // Command for image upload
            byte[] commandHeader = Encoding.Default.GetBytes($"~Eb,{filename},{bytes.Length}\r\n");

            // Full byte array with command & image that will be sent to the printer
            byte[] command = new byte[commandHeader.Length + bytes.Length];
            Buffer.BlockCopy(commandHeader, 0, command, 0, commandHeader.Length);
            Buffer.BlockCopy(bytes, 0, command, commandHeader.Length, bytes.Length);

            // Saves command as .txt
            string pathImg = $@"{AppContext.BaseDirectory}imgs";

            byte[] saveCommandHeader = Encoding.Default.GetBytes($"~Eb,{filename},{bytes.Length}\\r\\n");

            FileHandler.SaveImageUploadCommand(pathImg, filename, saveCommandHeader, bytes);

            return command;
        }
        #endregion

        /// <summary>
        /// Converts 2 types of data into printer ready commands<br>
        /// Types are: 1. Image command (~E); 2. Other data & commands
        /// </summary>
        /// <param name="data">String input from user</param>
        /// <returns>Printer ready command</returns>
        public static byte[] ConvertDataIntoCommand(string data)
        {
            string strippedData = data.Replace(" ", "");
            if (!data.EndsWith("\r\n") && !strippedData.StartsWith("~E"))
            {
                data += "\r\n";
                return Encoding.Default.GetBytes(data);
            }
            else if (strippedData.StartsWith("~E") && data.Split("\\r\\n").Length == 2)
                return FileHandler.LoadImageUploadCommand(data);

            return [];
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
    }
}
