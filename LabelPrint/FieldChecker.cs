using System.Net;

namespace LabelPrint
{
    internal static class FieldChecker
    {
        /// <summary>
        /// Checks if "name" input is valid
        /// </summary>
        /// <param name="ipInp">IP provided by user</param>
        /// <param name="ip">Variable in which IP address should be stored</param>
        /// <returns>Validity of "name" input</returns>
        public static bool CheckIPInput(string ipInp, out IPAddress? ip)
        {
            ip = null;

            // Checks if IP is empty
            if (ipInp == null || ipInp == "")
            {
                MessageBox.Show("Empty IP address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Tries to parse IP
            if (!IPAddress.TryParse(ipInp, out ip) || ip == null)
            {
                MessageBox.Show("Invalid IP address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }   

            return true;
        }

        /// <summary>
        /// Checks if "port" input is valid
        /// </summary>
        /// <param name="portInp">Port provided by user</param>
        /// <param name="port">Variable in which port should be stored</param>
        /// <returns>Validity of "port" input</returns>
        public static bool CheckPort(decimal portInp, out int port)
        {
            port = -1;

            // Checks if port isn't negative
            if (portInp < 0)
                MessageBox.Show("Invalid port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            port = (int)portInp;

            return (port >= 0);
        }

        /// <summary>
        /// Checks if EZPL script input is valid
        /// </summary>
        /// <param name="dataInp">Data provided by the user</param>
        /// <param name="data">Variable in which data should be stored</param>
        /// <returns>Validity of EZPL script input</returns>
        public static bool CheckData(string dataInp, out string data)
        {
            data = "";

            // Checks if input isn't empty
            if (dataInp == null || dataInp == "")
            {
                MessageBox.Show("Empty data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            data = dataInp;

            return true;
        }

        /// <summary>
        /// Checks if "name" input is valid
        /// </summary>
        /// <param name="nameInp">Name provided</param>
        /// <param name="filename">Variable in which name should be stored</param>
        /// <returns>Validity of "name" input</returns>
        public static bool IsNameValid(string nameInp, out string filename)
        {
            filename = "";

            if (nameInp == null || nameInp == "")
            {
                MessageBox.Show("Name wasn't provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (nameInp.Length > 20)
            {
                MessageBox.Show("Name is too long (max 20 characters).", "Invalid name error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (nameInp.Contains(" "))
            {
                MessageBox.Show("Name can't contain whitespaces.", "Invalid name error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            filename = nameInp;
            return true;
        }

        /// <summary>
        /// Checks if file path is valid
        /// </summary>
        /// <param name="pathInp">Path provided</param>
        /// <param name="path">Variable in which path should be stored</param>
        /// <returns>File path validity</returns>
        public static bool IsFilePathValid(string pathInp, out string path)
        {
            path = "";

            if (!File.Exists(pathInp))
            {
                MessageBox.Show("Access to file denied.\nPath is either invalid or file has insufficient permisions.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string[] acceptedExts = { "bmp", "gif", "exif", "jpg", "jpeg", "png", "tiff" };

            if (Array.IndexOf(acceptedExts, pathInp.Substring(pathInp.LastIndexOf('.') + 1).ToLower()) == -1)
            {
                MessageBox.Show("File has unsupported extension.\nUse files with extensions like BMP, GIF, EXIF, JPG, PNG, or TIFF.", "Extension error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            path = pathInp;
            return true;
        }
    }
}
