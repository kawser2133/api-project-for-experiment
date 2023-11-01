using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
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
            Program.Host = new CorsEnabledServiceHost(typeof(Service1), new Uri[] { new Uri("http://localhost:9119") });
            Program.Host.Open();
            Application.Run(new Form1());
            Host.Close();
        }
    }
}
