using System.Drawing.Imaging;
using System.Text.Json;

namespace LabelPrint
{
    [Serializable]
    public struct Config
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }

    internal static class FileHandler
    {
        /// <summary>
        /// Imports .txt or .cmd file into provided RichTextBox
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="rtb">Reference to the RichTextBox</param>
        /// <returns>Whether importing was successful(true) or not(false)</returns>
        public static bool ReadFileToRTB(string path, ref RichTextBox rtb)
        {
            string[] acceptedExts = { "txt", "cmd" };

            if (!File.Exists(path))
            {
                MessageBox.Show("Access to file denied.\nPath is either invalid or file has insufficient permisions.", "Access error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Checks extension
            if (Array.IndexOf(acceptedExts, path.Substring(path.LastIndexOf('.') + 1).ToLower()) < 0)
            {
                return false;
            }

            try
            {
                rtb.Text = File.ReadAllText(path);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Loads image and converts it into byte array
        /// </summary>
        /// <param name="path">Where is image located</param>
        /// <returns>Byte array representing image</returns>
        public static byte[] GetImageBytes(string? path, string? filename)
        {
            if (path == null || filename == null)
                return [];

            try
            {
                // Loads image into Bitmap
                using (Bitmap original = new Bitmap(path))
                {
                    // Converts source image to 1-bit monochrome indexed bitmap
                    using (Bitmap monoImg = original.Clone(
                        new Rectangle(0, 0, original.Width, original.Height),
                        PixelFormat.Format1bppIndexed))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Returns full 1-bit BMP byte array with 54-byte header
                            monoImg.SetResolution(203, 203);
                            monoImg.Save(ms, ImageFormat.Bmp);
                            SaveImg(monoImg, filename);
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch
            {
                return [];
            }
        }

        private static void SaveImg(Bitmap bmp, string filename)
        {
            string path = $@"{AppContext.BaseDirectory}imgs";

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = $"{path}\\{filename}.bmp";

                if (File.Exists(path))
                    File.Delete(path);

                bmp.Save($@"imgs/{filename}.bmp", ImageFormat.Bmp);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #region Config file
        /// <summary>
        /// Saves Config into json file
        /// </summary>
        /// <param name="config">Configuration struct</param>
        /// <returns>Whether saving succeeded(true) or not(false)</returns>
        public static bool SaveConfig(Config config)
        {
            string path = $@"{AppContext.BaseDirectory}saved";

            try
            {
                // Creates directory "saved" if it doesn't exist
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = $"{path}\\config.json";

                // Creates json file if it doesn't exist
                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }

                File.WriteAllText(path, JsonSerializer.Serialize(config));
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Returns json file as Config
        /// </summary>
        /// <returns>Config from json file</returns>
        public static Config? LoadConfig()
        {
            try
            {
                return JsonSerializer.Deserialize<Config>(File.ReadAllText($"{AppContext.BaseDirectory}saved\\config.json"));
            }
            catch { return null; }
        }
        #endregion
    }
}
