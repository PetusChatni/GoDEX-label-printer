namespace LabelPrint
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Tries to load configuration (ip & port)
            Config? config = FileHandler.LoadConfig();

            if (config != null)
            {
                Config newConfig = (Config)config;
                Application.Run(new Form1(newConfig.IP, newConfig.Port));
            }
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}