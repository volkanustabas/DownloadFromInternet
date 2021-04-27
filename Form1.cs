using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace DownloadFromInternet
{
    public partial class Form1 : Form
    {
        private WebClient _client;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

            var url = textBox1.Text;
            if (!string.IsNullOrEmpty(url))
            {
                var thread = new Thread(() =>
                {
                    var uri = new Uri(url);
                    var fileName = Path.GetFileName(uri.AbsolutePath);
                    _client.DownloadFileAsync(uri, Application.StartupPath + "/" + fileName);
                });

                thread.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _client = new WebClient();
            _client.DownloadProgressChanged += Client_DownloadProgressChanged;
            _client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            var getSetupName = textBox1.Text.Substring(textBox1.Text.LastIndexOf('/') + 1);

            if (MessageBox.Show(@"Download complete", @"Would you like to move on to the run or open phase?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(getSetupName);
            }
            else
            {
                Application.Exit();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                progressBar1.Minimum = 0;
                var receive = double.Parse(e.BytesReceived.ToString());
                var total = double.Parse(e.TotalBytesToReceive.ToString());
                var percentage = receive / total * 100;
                label1.Text = $@"Downloaded {$"{percentage:0.##}"}%";
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.CurrentCulture));
            }));
        }
    }
}