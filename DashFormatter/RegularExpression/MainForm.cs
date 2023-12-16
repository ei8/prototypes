using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ei8.Cortex.Diary.Formatter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var spaces = 0;
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                spaces = 1;
            }
            else
            {
                spaces = Convert.ToInt32(textBox2.Text);
            }
            var Formatter = new d23FormatService(d23FormatService.IndentStyleValue.Space, spaces);
            try
            {
                textBox1.Text = Formatter.Process(TextBox.Text);
            }
            catch(ArgumentException ae)
            {
                MessageBox.Show(ae.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = TextBox.Text;
            var array = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (var a = 0; a < array.Length; a++)
            {
                array[a] = array[a].Trim();
            }
            data = string.Join("", array);
            textBox1.Text = data;
        }
    }
}
