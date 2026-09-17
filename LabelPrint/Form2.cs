using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LabelPrint
{
    public partial class Form2 : Form
    {
        private Form3 form3;

        private TcpClient printerSocket;
        private SocketCommunicationHandler socketComHandler;

        private IPAddress? printerIP;
        private int printerPort;

        private bool isReceivingData = false;

        public Form2(IPAddress? printerIP, int printerPort)
        {
            InitializeComponent();

            this.printerIP = printerIP;
            this.printerPort = printerPort;
            socketComHandler = new SocketCommunicationHandler(printerIP, printerPort);
        }

        #region Wrapper functions for SocketCommunication
        /// <summary>
        /// Updates file list in DGV_Files
        /// </summary>
        private void UpdateFiles()
        {
            isReceivingData = true;

            SocketCommunicationTranslator.ProcessFiles(CommunicateWithPrinter(Encoding.Default.GetBytes("~MDIR\r\n"), true), ref DGV_Files, true);

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

            socketComHandler.AddImageToPrinter(ref printerSocket, path, filename);

            // Closes Form3
            form3.FormSubmitted -= UpdateFilesEventHandler;
            form3.Close();

            // Updates files
            UpdateFiles();
        }

        /// <summary>
        /// Wrapper for SocketCommunicationHandler.CommunicateWithRemote()
        /// </summary>
        /// <param name="commandToSend">Command / Data that will be sent to the printer</param>
        /// <param name="shouldReceiveData">Whether app should wait for data from printer or not</param>
        /// <returns>Whatever printer returned</returns>
        private string CommunicateWithPrinter(byte[] commandToSend, bool shouldReceiveData = false)
        {
            if (printerSocket == null)
                return SocketCommunicationHandler.CommunicateWithRemote(ref printerSocket, printerIP, printerPort, commandToSend, shouldReceiveData);
            else
                return socketComHandler.CommunicateWithRemote(ref printerSocket, commandToSend, shouldReceiveData);
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
            if(!isReceivingData)
                UpdateFiles();
        }

        /// <summary>
        /// Opens Form3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTN_AddImg_Click(object sender, EventArgs e)
        {
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
            if (printerSocket != null)
                printerSocket.Close();

            System.Environment.Exit(1);
        }

        /// <summary>
        /// Checks if "Delete" was clicked button, if so, deletes the file at button's row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGV_Files_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Checks if the button was clicked
            if (e.RowIndex < 0 || DGV_Files.Columns[e.ColumnIndex].Name != "btnColumn" || printerSocket == null)
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
            // Checks if printer socket isn't null; if so, closes it
            printerSocket?.Close();
            Visible = false;
        }
        #endregion
    }
}
