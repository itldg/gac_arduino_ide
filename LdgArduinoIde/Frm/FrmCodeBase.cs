using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using Microsoft.VisualBasic;
using ScintillaNET;

namespace LdgArduinoIde
{
    public partial class FrmCodeBase : DockContent
    {
        public event InsertCodeDelegate InsertCode;
        public delegate void InsertCodeDelegate(string Include, string Code);
        string codepath = Application.StartupPath + "\\Codes\\";
        string codeSplit = "\n---|gacSplit|------|gacSplit|---\n";
        public FrmCodeBase(string KeyWords1, string KeyWords2, string KeyWords3,string Theme)
        {
            InitializeComponent();
            txtCode = new Scintilla();
            LdgIdeHelper lih = new LdgIdeHelper(txtCode);
            lih.UseStyle(Theme);
            txtCode.SetKeywords(0, KeyWords1);//For If Setup等
            txtCode.SetKeywords(1, KeyWords2);//类和方法
            txtCode.SetKeywords(3, KeyWords3);//常量类 LOW HIGH
            txtCode.Dock = DockStyle.Fill;
            panel2.Controls.Add(txtCode);



            txtInclude = new Scintilla();
            LdgIdeHelper lih2 = new LdgIdeHelper(txtInclude);
            lih2.UseStyle(Theme);
            txtInclude.SetKeywords(0, KeyWords1);//For If Setup等
            txtInclude.SetKeywords(1, KeyWords2);//类和方法
            txtInclude.SetKeywords(3, KeyWords3);//常量类 LOW HIGH
            txtInclude.Dock = DockStyle.Fill;
            panel3.Controls.Add(txtInclude);

        }
        Scintilla txtCode;
        Scintilla txtInclude;
        private void FrmCodeBase_Load(object sender, EventArgs e)
        {
            InitFloder(codepath);
          
        }
        void InitFloder(string path,TreeNode tnParent=null)
        {
            if (tnParent == null)
            {
                treeView1.Nodes.Clear();
                tnParent = new TreeNode("代码库", 1, 1);
                tnParent.Tag = codepath;
                treeView1.Nodes.Add(tnParent);
            }
            else
            {
                tnParent.Nodes.Clear();
            }
            string[] dirs = Directory.GetDirectories(path);
            foreach (var item in dirs)
            {
                string name = Path.GetFileName(item);
                TreeNode tn = new TreeNode(name, 1, 1);
                tn.Tag = item;
                tnParent.Nodes.Add(tn);
                tn.Nodes.Add("");
            }

            string[] files = Directory.GetFiles(path);
            foreach (var item in files)
            {
                string name = Path.GetFileNameWithoutExtension(item);
                TreeNode tn = new TreeNode(name, 0, 0);
                tn.Tag = item;
                tnParent.Nodes.Add(tn);
            }
            if (treeView1.Nodes[0]==tnParent)
            {
                treeView1.Nodes[0].Expand();
            }
        }
        void SaveNode(string Name,string Path,TreeNode nodeParent = null, TreeNode nodeCurr = null)
        {

            if (nodeParent == null)
            {
                nodeParent = treeView1.Nodes[0];
            }
            if (nodeCurr == null)
            {
                int imageindex = Path.EndsWith(".code") ? 0 : 1;
                TreeNode tn = new TreeNode(Name, imageindex, imageindex);
                tn.Tag = Path;
                nodeParent.Nodes.Add(tn);
            }
            else
            {
                nodeCurr.Text = Name;
                nodeCurr.Tag = Path;
            }
           

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddFile();


        }
        void AddFile()
        {
            panel1.Visible = true;
            panel1.Tag = null;
            txtName.Text = "";
            txtInclude.Text = "";
            txtCode.Text = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckFileName(txtName.Text))
            {
                string code = txtInclude.Text + codeSplit + txtCode.Text;
                string filename = codepath + txtName.Text + ".code";
                TreeNode tnCurr = null;
                TreeNode tnParnet = null;
                if (panel1.Tag != null )
                {
                    TreeNode tn = null;
                    tn = panel1.Tag as TreeNode;
                    if (tn.Tag.ToString().EndsWith(".code"))
                    {
                        tnCurr = tn;
                        filename = tn.Tag.ToString();
                        File.WriteAllText(filename, code);
                        string oldname = Path.GetFileNameWithoutExtension(tn.Tag.ToString());
                        if (oldname != txtName.Text)
                        {
                            string dir = Path.GetDirectoryName(tn.Tag.ToString());
                            filename = dir + "\\" + txtName.Text + ".code";
                            if (File.Exists(filename))
                            {
                                toolTip1.Show("该名称已存在,请更换其他名称",txtName);
                                return;
                            }
                            File.Move(tn.Tag.ToString(), filename);
                        }
                    }
                    else
                    {
                        tnParnet = tn;
                        string dir = tn.Tag.ToString();
                        filename = dir + "\\" + txtName.Text + ".code";
                        if (File.Exists(filename))
                        {
                            toolTip1.Show("该名称已存在,请更换其他名称", txtName);
                            return;
                        }
                        File.WriteAllText(filename, code);
                    }
                   
                }
                else
                {
                    if (File.Exists(filename))
                    {
                        toolTip1.Show("该名称已存在,请更换其他名称", txtName);
                        return;
                    }
                    File.WriteAllText(filename, code);
                }
                
                SaveNode(txtName.Text, filename, tnParnet, tnCurr);

            }
            else
            {
                toolTip1.Show("名称不合法,请不要输入特殊字符", txtName, 3000);
            }
        }
        /// <summary>
        /// 检查文件名是否有效。
        /// </summary>
        /// <param name="FileName"></param>
        public static Boolean CheckFileName(String fileName)
        {
            Boolean isValid = true;
            String[] strList = new String[] { " ", "/", "\"", @"\", @"\/", ":", "*", "?", "<", ">", "|", "\r\n" };

            if (String.IsNullOrEmpty(fileName))
            {
                isValid = false;
            }
            else
            {
                foreach (String errStr in strList)
                {
                    if (fileName.Contains(errStr))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        private void tsmiAddDir_Click(object sender, EventArgs e)
        {
            string str = Interaction.InputBox("请输入要创建的分类名称", "输入名字", "", -1, -1);
            if (!string.IsNullOrEmpty(str))
            {
                string dir = codepath;
                if (treeView1.SelectedNode != null)
                {
                    if (treeView1.SelectedNode.SelectedImageIndex == 1)
                    {
                        dir = treeView1.SelectedNode.Tag.ToString();
                    }
                    else
                    {
                        dir = Path.GetDirectoryName(treeView1.SelectedNode.Tag.ToString());
                    }
                    
                }
                dir += "\\" + str;
                if (Directory.Exists(dir))
                {
                    MessageBox.Show("分类已存在");
                }
                else
                {
                    Directory.CreateDirectory(dir);
                    SaveNode(str,dir, treeView1.SelectedNode);
                    MessageBox.Show("创建成功");
                }
            }
            


        }

        private void tsmiAddFile_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1.Tag = treeView1.SelectedNode;
            txtName.Text = "";
            txtInclude.Text = "";
            txtCode.Text = "";
        }

        private void treeView1_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            TreeNode tn = panel1.Tag as TreeNode;
            try
            {
                File.Delete(tn.Tag.ToString());
                tn.Remove();
                panel1.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "删除失败");
            }
        }

        private void tsmiDeleteNode_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.SelectedImageIndex == 0)
            {
                try
                {
                    File.Delete(treeView1.SelectedNode.Tag.ToString());
                    treeView1.SelectedNode.Remove();
                    panel1.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "删除失败");
                }

            }
            else
            {
                if (treeView1.SelectedNode.Nodes.Count > 0 && MessageBox.Show("该分类下存在子项目,是否删除全部子项目?", "确认删除", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                try
                {
                    Directory.Delete(treeView1.SelectedNode.Tag.ToString());
                    treeView1.SelectedNode.Remove();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "删除失败");
                }
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (InsertCode!=null)
            {
                InsertCode(txtInclude.Text, txtCode.Text);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.SelectedImageIndex == 0)
            {
                panel1.Visible = true;
                panel1.Tag = treeView1.SelectedNode;
                txtName.Text = treeView1.SelectedNode.Text;
                txtInclude.Text = "";
                txtCode.Text = "";
                string filename = treeView1.SelectedNode.Tag.ToString();
                if (File.Exists(filename))
                {
                    string[] codes = File.ReadAllText(filename).Split(new string[] { codeSplit }, StringSplitOptions.None);
                    if (codes.Length == 2)
                    {
                        txtInclude.Text = codes[0];
                        txtCode.Text = codes[1];
                    }
                }
            }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.SelectedImageIndex == 1)
            { 
                if ((e.Node.Nodes.Count>0&&e.Node.Nodes[0].Text == "") || e.Node.Nodes.Count == 0)
                {
                    InitFloder(e.Node.Tag.ToString(), e.Node);
                }
            }
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
           
                // 开始进行拖放操作，并将拖放的效果设置成移动。
                this.DoDragDrop(e.Item, DragDropEffects.Move);
            
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            // 拖动效果设成移动
            e.Effect = DragDropEffects.Move;
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            // 定义一个中间变量
            TreeNode treeNode;
            //判断拖动的是否为TreeNode类型，不是的话不予处理
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                // 拖放的目标节点
                TreeNode targetTreeNode;
                // 获取当前光标所处的坐标
                // 定义一个位置点的变量，保存当前光标所处的坐标点
                Point point = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                // 根据坐标点取得处于坐标点位置的节点
                targetTreeNode = ((TreeView)sender).GetNodeAt(point);
                // 获取被拖动的节点
                treeNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                if (treeNode.Parent==null)
                {
                    MessageBox.Show("根节点不能移动");
                    return;
                }
                if (treeNode== targetTreeNode)
                {
                    return;
                }
                if (targetTreeNode.SelectedImageIndex==0)//说明这个节点是文件,改父级接收
                {
                    targetTreeNode = targetTreeNode.Parent;
                }
                try
                {
                    string newdir = targetTreeNode.Tag.ToString();
                    string oldpath = treeNode.Tag.ToString();
                    if (treeNode.SelectedImageIndex == 0)//文件移动
                    {
                        string filename = Path.GetFileName(oldpath);
                        string newfilename = newdir + "\\" + filename;
                        File.Move(oldpath, newfilename);
                        treeNode.Tag = newfilename;
                    }
                    else
                    {
                        newdir += "\\" + Path.GetFileName(oldpath);
                        MoveFolder(oldpath, newdir);
                        treeNode.Tag = newdir;
                        treeNode.Nodes.Clear();
                        treeNode.Nodes.Add("");
                    }
                    // 往目标节点中加入被拖动节点的一份克隆
                    targetTreeNode.Nodes.Add((TreeNode)treeNode.Clone());
                    // 将被拖动的节点移除
                    treeNode.Remove();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "移动失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                

                // 判断拖动的节点与目标节点是否是同一个,同一个不予处理
                //if (BaseInterfaceLogic.TreeNodeCanMoveTo(treeNode, targetTreeNode))
                //{
                //    if (BaseSystemInfo.ShowInformation)
                //    {
                //        // 是否移动部门
                //        if (MessageBox.Show(AppMessage.Format(AppMessage.MSG0038, treeNode.Text, targetTreeNode.Text), AppMessage.MSG0000, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                //        {
                //            return;
                //        }
                //    }
                //    ServiceManager.Instance.OrganizeService.MoveTo(UserInfo, treeNode.Tag.ToString(), targetTreeNode.Tag.ToString());
                //    // 往目标节点中加入被拖动节点的一份克隆
                //    targetTreeNode.Nodes.Add((TreeNode)treeNode.Clone());
                //    // 将被拖动的节点移除
                //    treeNode.Remove();
                //}
            }
        }
        /// <summary>
        /// 移动文件夹中的所有文件夹与文件到另一个文件夹 //转载请注明来自 http://www.uzhanbao.com
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destPath">目标文件夹</param>
        public  void MoveFolder(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //覆盖模式
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                //获得源文件下所有目录文件
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //Directory.Move必须要在同一个根目录下移动才有效，不能在不同卷中移动。
                    //Directory.Move(c, destDir);

                    //采用递归的方法实现
                    MoveFolder(c, destDir);
                });
                Directory.Delete(sourcePath);
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }

        private void FrmCodeBase_Shown(object sender, EventArgs e)
        {

        }
    }
    }
