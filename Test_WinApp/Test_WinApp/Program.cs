using System;
using System.IO;
using System.ServiceModel.Web;
using System.Windows.Forms;

namespace Test_WinApp
{
    internal static class Program
    {
        private static WebServiceHost Host;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Directory.SetCurrentDirectory(Path.Combine(Application.StartupPath, "Lib"));

            Program.Host = new WebServiceHost(typeof(Service1), new Uri("http://localhost:8200"));
            Program.Host.Open();
            Application.Run(new Form1());
            Program.Host.Close();
        }
    }
}
