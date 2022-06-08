using GAC_Collection.Ex;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LdgArduinoIde
{
    public partial class FrmCode : DockContentEx
    {
        public bool AllowToClose = false;

        public FrmCode()
        {
            InitializeComponent();
        }

        private void FrmCode_Load(object sender, EventArgs e)
        {

        }

        private void FrmCode_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }

        }
    }
}
