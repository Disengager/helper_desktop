using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Helper
{
    public partial class Input_Box : Form
    {
        public Input_Box()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (Application.OpenForms[0] as Form1).NewName = textBox1.Text;
            this.Hide();
            (Application.OpenForms[0] as Form1).Stop = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (Application.OpenForms[0] as Form1).NewName = "null";
            this.Hide();
            (Application.OpenForms[0] as Form1).Stop = false;
        }
    }
}
