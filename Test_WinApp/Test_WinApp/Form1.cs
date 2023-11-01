using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_WinApp
{
    public partial class Form1 : Form
    {
        private ServiceHost serviceHost;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize and start the WCF service
            Uri baseAddress = new Uri("http://localhost:8000/");
            serviceHost = new ServiceHost(typeof(Service1), baseAddress);

            // Add a service endpoint
            serviceHost.AddServiceEndpoint(typeof(IService1), new BasicHttpBinding(), "MyService");

            // Open the service host
            serviceHost.Open();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Close the service host when the form is closed
            if (serviceHost != null)
            {
                serviceHost.Close();
            }
        }
    }
}
