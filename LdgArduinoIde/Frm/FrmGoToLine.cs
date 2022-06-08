using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LdgArduinoIde
{
    public partial class FrmGoToLine : Form
    {
        public int Line = 0, Column = 0, MaxLine = 0;
        public FrmGoToLine(int CurrLine,int CurrColumn,int LastLine)
        {
            InitializeComponent();
            lblCurrLine.Text = CurrLine.ToString();
            lblCurrColumn.Text = CurrColumn.ToString();
            lblLastLine.Text = LastLine.ToString();
            this.MaxLine = LastLine;
        }

        private void TextKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                GoTo();
            }
            else if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
            else if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
                return;
            }
            
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            GoTo();
        }

        private void FrmGoToLine_Load(object sender, EventArgs e)
        {

        }

        void GoTo()
        {
            Line = Convert.ToInt32(txtLine.Text);
            if (Line <= MaxLine)
            {
                Column = Convert.ToInt32(txtColumn.Text);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("您输入的行号过大", "行号错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLine.Focus();
            }
            
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
