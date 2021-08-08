using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UI_WinForms
{
    public partial class Form_UI : Form
    {
        public Form_UI()
        {
            InitializeComponent();
        }

        private void ctrlInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                ctrlOutput.AppendText($"> {ctrlInput.Text}{Environment.NewLine}");
                ctrlInput.Text = "";
                e.Handled = true;
            }
        }
    }
}
