using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LdgArduinoIde
{
    public partial class FrmAbout : Form
    {
       
        public FrmAbout()
        {
            InitializeComponent();
            
            lblVer.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           lblName.Text = GetAssemblyTitle();
            this.Text ="关于 "+ lblName.Text;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gac.cc");
        }

        private void llbQQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://jq.qq.com/?_wv=1027&k=ebnZMmjg");
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FrmAbout_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 获取程序集标题
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyTitle()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute title = (AssemblyTitleAttribute)attributes[0];
                if (!string.IsNullOrEmpty(title.Title))
                    return title.Title;
            }
            return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }
    }
}
