using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWindow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JsonConfig.AddControl(comboBox1);
            JsonConfig.AddControl(comboBox2);
            JsonConfig.AddControl(checkBox1);
            JsonConfig.AddControl(textBox1);
        }
    }
}
