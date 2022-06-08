using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LdgArduinoIde;
using Microsoft.Win32;
using System.Security.Permissions;

namespace ArduinoIde2
{
    public partial class FrmOption : Form
    {
        public IdeConfigInfo Config = new IdeConfigInfo();
        string[] codes =new string[]{};
        public FrmOption(IdeConfigInfo ici)
        {
            InitializeComponent();
            string[] files = Directory.GetFiles(Application.StartupPath+"\\Theme", "*.txt");
            foreach (var item in files)
            {
                string name = Path.GetFileNameWithoutExtension(item);
                cmbTheme.Items.Add(name);
            }
            cmbTheme.Text = ici.ThemeName;
            cmbDebug.SelectedIndex = ici.DebugModule;
            cmbSmartTip.SelectedIndex = ici.SmartTip;
            chkBuild_verbose.Checked = ici.BuildVerbose;
            chkUpload_verbose.Checked = ici.UploadVerbose;

            codes = GetCodeStyles();
            cmbCodeStyle.SelectedIndex = ici.CodeStyle;
            chkDeleteEmptyLines.Checked = ici.DeleteEmptyLines;
            chkUnpadParen.Checked = ici.UnpadParen;
            chkIndentCol1Comments.Checked = ici.IndentCol1Comments;

            
        }
        public string[] GetCodeStyles()
        {
            string code = @"int Foo()
{
    if (isBar)
    {
        bar();
        return 1;
    }
    else
    {
        return 0;
    }
}
---
int Foo() {
    if (isBar) {
        bar();
        return 1;
    } else {
        return 0;
    }
}
---
int Foo()
{
    if (isBar) {
        bar();
        return 1;
    } else {
        return 0;
    }
}
---
int Foo()
{
    if (isBar) {
        bar();
        return 1;
    } else {
        return 0;
    }
}
---
int Foo()
    {
    if (isBar)
        {
        bar();
        return 1;
        }
    else
        {
        return 0;
        }
    }
---
int Foo() {
    if (isBar) {
        bar();
        return 1;
        }
    else {
        return 0;
        }
    }
---
int Foo()
{
    if (isBar)
        {
            bar();
            return 1;
        }
    else
        {
            return 0;
        }
}
---
int Foo()
{
    if (isBar) {
        bar();
        return 1;
    } else {
        return 0;
    }
}
---
int Foo()
{   if (isBar)
    {   bar();
        return 1;
    }
    else
    {   return 0;
    }
}
---
int Foo()
{
    if (isBar) {
        bar();
        return 1;
    } else {
        return 0;
    }
}
---
int Foo()
{   if (isBar)
    {   bar();
        return 1; }
    else
        return 0; }
---
int Foo() {
    if (isBar) {
        bar();
        return 1; }
    else
        return 0; }";
            return code.Split(new string[] { "---\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void FrmOption_Load(object sender, EventArgs e)
        {
            try
            {
                // 检查 文件关联是否创建
                RegistryKey isExCommand = Registry.ClassesRoot.OpenSubKey(".ino" + @"_auto_file\shell\open\command");
                if (isExCommand != null)
                {
                    string currexe = isExCommand.GetValue("").ToString();
                    if (currexe.Contains(Application.ExecutablePath.ToString()))
                    {
                        btnRelation.Text = "已关联GAC编辑器";
                        btnRelation.Enabled = false;
                    }
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config.ThemeName = cmbTheme.Text;
            Config.DebugModule = cmbDebug.SelectedIndex;
            Config.BuildVerbose = chkBuild_verbose.Checked;
            Config.UploadVerbose = chkUpload_verbose.Checked;
            Config.SmartTip = cmbSmartTip.SelectedIndex;

            Config.CodeStyle=cmbCodeStyle.SelectedIndex ;
            Config.DeleteEmptyLines=chkDeleteEmptyLines.Checked ;
            Config.UnpadParen=chkUnpadParen.Checked;
            Config.IndentCol1Comments=chkIndentCol1Comments.Checked;

            DialogResult = DialogResult.OK;
        }


        private void btnRelation_Click(object sender, EventArgs e)
        {
            RegFileExt(".ino");
        }
        /// <summary>
        /// 关联程序和类型
        /// </summary>
        private void RegFileExt(string ext)
        {
            try
            {
                string boardExeFullName = Application.ExecutablePath.ToString();
                if (File.Exists(boardExeFullName))
                {
                    string MyExtName = ext;
                    string MyType = ext+"_auto_file";
                    string MyContent = "application/dbb";
                    string command = "\"" + boardExeFullName + "\"" + " \"%1\"";
                    RegistryKey key = Registry.ClassesRoot.OpenSubKey(MyType);
                    if (key == null)
                    {
                        RegistryKey MyReg = Registry.ClassesRoot.CreateSubKey(MyExtName);
                        MyReg.SetValue("", MyType);
                        MyReg.SetValue("Content Type", MyContent);
                        MyReg = Registry.ClassesRoot.CreateSubKey(MyType);
                        MyReg.SetValue("", MyType);
                        MyReg = MyReg.CreateSubKey("Shell\\Open\\Command");
                        MyReg.SetValue("", command);
                        MyReg.Close();
                        btnRelation.Text = "已关联GAC编辑器";
                        btnRelation.Enabled = false;
                        MessageBox.Show("Gac编辑器成功设置为默认打开方式", "关联成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var myReg = key.OpenSubKey("Shell\\Open\\Command", true);
                        if (myReg != null && (myReg.GetValue("") == null || myReg.GetValue("").ToString() != command))
                        {
                            myReg.SetValue("", command);//解决因目录变化导致 注册表失效的问题
                            myReg.Close();
                            btnRelation.Text = "已关联GAC编辑器";
                            btnRelation.Enabled = false;
                            MessageBox.Show("Gac编辑器成功设置为默认打开方式", "关联成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "关联失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
        }

        private void cmbCodeStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCode.Text = codes[cmbCodeStyle.SelectedIndex];
        }
    }
}
