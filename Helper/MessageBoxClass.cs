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
    public partial class MessageBoxClass : Form
    {
        public MessageBoxClass()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            (Application.OpenForms[0] as Form1).Stop = false;
        }
    }
}
