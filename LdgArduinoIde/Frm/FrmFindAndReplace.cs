using ScintillaNET;
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
    public partial class FrmFindAndReplace : Form
    {
        Scintilla scintilla;
        public FrmFindAndReplace(Scintilla scintilla)
        {
            InitializeComponent();
            this.scintilla=scintilla;
            
        }

        private void FrmFindAndReplace_Load(object sender, EventArgs e)
        {

        }
        protected override void OnActivated(EventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
            base.OnActivated(e);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            FindNext();
        }
        int keyLength = 0;
        public void FindNext(bool Replace=false)
        {

            Search(txtSearch.Text,GetFlags(),true, Replace);
        }
        public void FindPrev()
        {
            Search(txtSearch.Text, GetFlags(), false);
        }
        string lastKey = "";
        private void Search(string text, SearchFlags flags,bool Next =true,bool Replace=false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            const int NUM = 8;

            // Remove all uses of our indicator
            scintilla.IndicatorCurrent = NUM;
            scintilla.IndicatorClearRange(0, scintilla.TextLength);

            // Update indicator appearance
            scintilla.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            scintilla.Indicators[NUM].Under = true;
            scintilla.Indicators[NUM].ForeColor = Color.OrangeRed;
            scintilla.Indicators[NUM].OutlineAlpha = 50;
            scintilla.Indicators[NUM].Alpha = 30;

            scintilla.SearchFlags = flags;
            if (lastKey != text)
            {

                scintilla.TargetStart = 0;
                scintilla.TargetEnd = scintilla.TextLength;
                
                while (scintilla.SearchInTarget(text) != -1)
                {
                    scintilla.IndicatorFillRange(scintilla.TargetStart, scintilla.TargetEnd - scintilla.TargetStart);
                    scintilla.TargetStart = scintilla.TargetEnd;
                    scintilla.TargetEnd = scintilla.TextLength;
                }
            }
            if (Next)
            {
                scintilla.TargetStart = Math.Max(scintilla.CurrentPosition, scintilla.AnchorPosition);
                scintilla.TargetEnd =  scintilla.TextLength;
            }
            else
            {
                scintilla.TargetStart = Math.Min(scintilla.CurrentPosition, scintilla.AnchorPosition); ;
                scintilla.TargetEnd = 0;
            }
            int pos = scintilla.SearchInTarget(text);
            if (pos != -1)
            {
                scintilla.SetSel(scintilla.TargetStart, scintilla.TargetEnd);
                tsslStatus.Visible = false;
            }
            else
            {
                if (Replace)
                {
                    tsslStatus.Text = "替换:没有找到匹配内容";
                }
                else {
                    tsslStatus.Text = "查找:找不到更多 \"" + text + " \"相关内容";
                }
                
                tsslStatus.Visible = true; ;
            }


        }
        SearchFlags GetFlags()
        {
            return SearchFlags.None;
            //SearchFlags flags = chkMatchCase.Checked ? SearchFlags.MatchCase : 0;
            //return flags;
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            keyLength=txtSearch.Text.Length;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            FindPrev();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar=='\r')
            {
                FindNext();
            }
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            scintilla.Text= scintilla.Text.Replace(txtSearch.Text, txtReplace.Text);
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            Replace();
        }
        void Replace()
        {
            if (scintilla.SelectedText != txtSearch.Text)
            {
                FindNext(true);
                if (!tsslStatus.Visible)
                {
                    scintilla.ReplaceTarget(txtReplace.Text);
                    scintilla.SetSel(scintilla.CurrentPosition, scintilla.CurrentPosition + replaceLength);
                }
            }
            else
            {

                scintilla.SetTargetRange(scintilla.CurrentPosition- keyLength, scintilla.CurrentPosition );
                scintilla.ReplaceTarget(txtReplace.Text);
                scintilla.SetSel(scintilla.CurrentPosition, scintilla.CurrentPosition + replaceLength);
                
            }
           
            
        }
        int replaceLength = 0;
        private void txtReplace_TextChanged(object sender, EventArgs e)
        {
            replaceLength = txtReplace.Text.Length;
        }

        private void FrmFindAndReplace_FormClosed(object sender, FormClosedEventArgs e)
        {
            //scintilla.IndicatorClearRange(0, scintilla.Text.Length);
        }

        private void txtReplace_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==13)
            {
                Replace();
            }
           
        }

        private void FrmFindAndReplace_FormClosing(object sender, FormClosingEventArgs e)
        {
            scintilla.IndicatorClearRange(0, scintilla.Text.Length);
            this.Visible = false;
            e.Cancel = true;
        }
    }
}
