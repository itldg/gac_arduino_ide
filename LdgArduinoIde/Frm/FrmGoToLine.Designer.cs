namespace LdgArduinoIde
{
    partial class FrmGoToLine
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCurrLine = new System.Windows.Forms.Label();
            this.lblCurrColumn = new System.Windows.Forms.Label();
            this.lbn = new System.Windows.Forms.Label();
            this.lblLastLine = new System.Windows.Forms.Label();
            this.txtLine = new System.Windows.Forms.TextBox();
            this.txtColumn = new System.Windows.Forms.TextBox();
            this.btnGoTo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "跳转行号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "当前行号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(214, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "列：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(214, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "列：";
            // 
            // lblCurrLine
            // 
            this.lblCurrLine.AutoSize = true;
            this.lblCurrLine.Location = new System.Drawing.Point(71, 37);
            this.lblCurrLine.Name = "lblCurrLine";
            this.lblCurrLine.Size = new System.Drawing.Size(11, 12);
            this.lblCurrLine.TabIndex = 1;
            this.lblCurrLine.Text = "0";
            // 
            // lblCurrColumn
            // 
            this.lblCurrColumn.AutoSize = true;
            this.lblCurrColumn.Location = new System.Drawing.Point(241, 37);
            this.lblCurrColumn.Name = "lblCurrColumn";
            this.lblCurrColumn.Size = new System.Drawing.Size(11, 12);
            this.lblCurrColumn.TabIndex = 1;
            this.lblCurrColumn.Text = "0";
            // 
            // lbn
            // 
            this.lbn.AutoSize = true;
            this.lbn.Location = new System.Drawing.Point(12, 62);
            this.lbn.Name = "lbn";
            this.lbn.Size = new System.Drawing.Size(65, 12);
            this.lbn.TabIndex = 0;
            this.lbn.Text = "最后行号：";
            // 
            // lblLastLine
            // 
            this.lblLastLine.AutoSize = true;
            this.lblLastLine.Location = new System.Drawing.Point(71, 62);
            this.lblLastLine.Name = "lblLastLine";
            this.lblLastLine.Size = new System.Drawing.Size(11, 12);
            this.lblLastLine.TabIndex = 1;
            this.lblLastLine.Text = "0";
            // 
            // txtLine
            // 
            this.txtLine.Location = new System.Drawing.Point(73, 8);
            this.txtLine.Name = "txtLine";
            this.txtLine.Size = new System.Drawing.Size(135, 21);
            this.txtLine.TabIndex = 2;
            this.txtLine.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextKeyPress);
            // 
            // txtColumn
            // 
            this.txtColumn.Location = new System.Drawing.Point(243, 8);
            this.txtColumn.Name = "txtColumn";
            this.txtColumn.Size = new System.Drawing.Size(94, 21);
            this.txtColumn.TabIndex = 2;
            this.txtColumn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextKeyPress);
            // 
            // btnGoTo
            // 
            this.btnGoTo.Location = new System.Drawing.Point(353, 7);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(77, 28);
            this.btnGoTo.TabIndex = 3;
            this.btnGoTo.Text = "跳转";
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(353, 47);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 27);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmGoToLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 85);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGoTo);
            this.Controls.Add(this.txtColumn);
            this.Controls.Add(this.txtLine);
            this.Controls.Add(this.lblCurrColumn);
            this.Controls.Add(this.lblLastLine);
            this.Controls.Add(this.lblCurrLine);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGoToLine";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "跳转到行";
            this.Load += new System.EventHandler(this.FrmGoToLine_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCurrLine;
        private System.Windows.Forms.Label lblCurrColumn;
        private System.Windows.Forms.Label lbn;
        private System.Windows.Forms.Label lblLastLine;
        private System.Windows.Forms.TextBox txtLine;
        private System.Windows.Forms.TextBox txtColumn;
        private System.Windows.Forms.Button btnGoTo;
        private System.Windows.Forms.Button btnCancel;
    }
}