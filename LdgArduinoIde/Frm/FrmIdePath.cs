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
    public partial class FrmIdePath : Form
    {
        public FrmIdePath()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择Ide路径";
            ofd.FileName = "arduino_debug.exe";
            ofd.Filter = "Ide路径|arduino_debug.exe";
            if (ofd.ShowDialog()==DialogResult.OK)
            {
                txtPath.Text = ofd.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FrmIdePath_Load(object sender, EventArgs e)
        {

        }
    }
}
