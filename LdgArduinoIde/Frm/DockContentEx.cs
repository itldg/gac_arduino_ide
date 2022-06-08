using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace GAC_Collection.Ex
{
    /// <summary>
    /// 很多窗体都在Tab中有个右键菜单，右击的里面有关闭，所以最好继承一下DockContent，
    /// 让其它窗体只要继承这个就有了这个右键菜单
    /// </summary>
    public class DockContentEx : DockContent
    {
        //在标签上点击右键显示关闭菜单
        public DockContentEx()
        {
            System.Windows.Forms.ContextMenuStrip cms = new System.Windows.Forms.ContextMenuStrip();
            // 
            // tsmiClose
            // 
            System.Windows.Forms.ToolStripMenuItem tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            tsmiClose.Name = "cms";
            tsmiClose.Size = new System.Drawing.Size(98, 22);
            tsmiClose.Text = "关闭当前";
            tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
            // 
            // tsmiALLClose
            // 
            System.Windows.Forms.ToolStripMenuItem tsmiALLClose = new System.Windows.Forms.ToolStripMenuItem();
            tsmiALLClose.Name = "cms";
            tsmiALLClose.Size = new System.Drawing.Size(98, 22);
            tsmiALLClose.Text = "全部关闭";
            tsmiALLClose.Click += new System.EventHandler(this.tsmiALLClose_Click);
            // 
            // tsmiApartFromClose
            // 
            System.Windows.Forms.ToolStripMenuItem tsmiApartFromClose = new System.Windows.Forms.ToolStripMenuItem();
            tsmiApartFromClose.Name = "cms";
            tsmiApartFromClose.Size = new System.Drawing.Size(98, 22);
            tsmiApartFromClose.Text = "关闭其他标签";
            tsmiApartFromClose.Click += new System.EventHandler(this.tsmiApartFromClose_Click);
            //// 
            //// tsmiCloseLeft
            //// 
            //System.Windows.Forms.ToolStripMenuItem tsmiCloseLeft = new System.Windows.Forms.ToolStripMenuItem();
            //tsmiCloseLeft.Name = "cms";
            //tsmiCloseLeft.Size = new System.Drawing.Size(98, 22);
            //tsmiCloseLeft.Text = "关闭左边所有";
            //tsmiCloseLeft.Click += new System.EventHandler(this.tsmiCloseLeft_Click);
            //// 
            //// tsmiCloseRight
            //// 
            //System.Windows.Forms.ToolStripMenuItem tsmiCloseRight = new System.Windows.Forms.ToolStripMenuItem();
            //tsmiCloseRight.Name = "cms";
            //tsmiCloseRight.Size = new System.Drawing.Size(98, 22);
            //tsmiCloseRight.Text = "关闭右边所有";
            //tsmiCloseRight.Click += new System.EventHandler(this.tsmiCloseRight_Click);
            // 
            // tsmiClose
            // 
            cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsmiClose,tsmiApartFromClose,tsmiALLClose});//tsmiCloseLeft,tsmiCloseRight
            cms.Name = "tsmiClose";
            cms.Size = new System.Drawing.Size(99, 26);
            this.TabPageContextMenuStrip = cms;
        }
        private void tsmiClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void tsmiALLClose_Click(object sender, EventArgs e)
        {
            DockContentCollection contents = DockPanel.Contents;
            int num = 0;
            while (num < contents.Count)
            {
                if (contents[num].DockHandler.DockState == DockState.Document)
                {
                    contents[num].DockHandler.Hide();
                }
                else
                {
                    num++;
                }
            }
        }
        private void tsmiApartFromClose_Click(object sender, EventArgs e)
        {
            DockContentCollection contents = DockPanel.Contents;
            int num = 0;
            while (num < contents.Count)
            {
                if (contents[num].DockHandler.DockState == DockState.Document && DockPanel.ActiveContent != contents[num])
                {
                    contents[num].DockHandler.Hide();
                }
                else
                {
                    num++;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DockContentEx
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "DockContentEx";
            this.Load += new System.EventHandler(this.DockContentEx_Load);
            this.ResumeLayout(false);

        }

        private void DockContentEx_Load(object sender, EventArgs e)
        {

        }

        //private void tsmiCloseRight_Click(object sender, EventArgs e)
        //{
        //    DockContentCollection contents = DockPanel.Contents;
        //    int num = 0; bool isok = false;
        //    while (num < contents.Count)
        //    {

        //        if (contents[num].DockHandler.DockState == DockState.Document && DockPanel.ActiveContent != contents[num])
        //        {
        //            if (isok)
        //            {
        //                contents[num].DockHandler.Hide();
        //            }
        //        }
        //        else if (contents[num].DockHandler.DockState == DockState.Document)
        //        {
        //            isok = true;
        //        }
        //        num++;
        //    }

        //}
        //private void tsmiCloseLeft_Click(object sender, EventArgs e)
        //{
        //    DockContentCollection contents = DockPanel.Contents;
        //    int num = 0; bool isok = false;
        //    while (num < contents.Count)
        //    {

        //        if (contents[num].DockHandler.DockState == DockState.Document && DockPanel.ActiveContent != contents[num])
        //        {
        //            if (!isok)
        //            {
        //                contents[num].DockHandler.Hide();
        //            }
        //        }
        //        else if (contents[num].DockHandler.DockState == DockState.Document)
        //        {
        //            isok = true;
        //        }
        //        num++;
        //    }
        //}
    }
}
