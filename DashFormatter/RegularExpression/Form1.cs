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

namespace RegularExpression
{
    public partial class Form1 : Form
    {
        static string[] result;
        static List<int> Closetabs = new List<int>();
        static int spaces = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                spaces = 1;
            }
            else
            {
                spaces = Convert.ToInt32(textBox2.Text);
            }
            var Formatter = new DashFormatter(DashFormatter.IndentStyleValue.Space, spaces);
            try
            {
                textBox1.Text = Formatter.Process(TextBox.Text);
            }
            catch
            {
                MessageBox.Show("the count of open not equal close");
            }
            
        }

        static bool CheckStringValidation(string data)
        {
            var open = Regex.Matches(data, @"{");
            var close = Regex.Matches(data, @"}");
            if(open.Count == close.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

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
