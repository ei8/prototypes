using NLog;
using Reactor.SpikeResults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Reactor
{
    public partial class LogsForm : DockContent
    {
        public LogsForm(ISpikeResultsService resultsService)
        {
            InitializeComponent();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.logTextBox.Clear();
        }

        private void LogsForm_Load(object sender, EventArgs e)
        {
            // ${basedir}/logs/${shortdate}.log
            var t = new Thread(new ThreadStart(() =>
            {
                string filename = Path.Combine(Environment.CurrentDirectory, "logs", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    //start at the end of the file
                    long lastMaxOffset = reader.BaseStream.Length;

                    while (true)
                    {
                        System.Threading.Thread.Sleep(100);

                        //if the file size has not changed, idle
                        if (reader.BaseStream.Length == lastMaxOffset)
                            continue;

                        //seek to the last max offset
                        reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                        //read out of the file until the EOF
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            this.Invoke(new MethodInvoker(() =>
                                {
                                    this.logTextBox.AppendText(line + Environment.NewLine);
                                    this.logTextBox.ScrollToCaret();
                                }
                            )
                            );
                            //Console.WriteLine(line);
                        }

                        //update the last max offset
                        lastMaxOffset = reader.BaseStream.Position;
                    }
                }
            }
            ));
            t.Start();
        }
    }
}
