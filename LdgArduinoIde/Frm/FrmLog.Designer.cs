namespace LdgArduinoIde
{
    partial class FrmLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLog));
            this.txtIdeLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtIdeLog
            // 
            this.txtIdeLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(231)))), ((int)(((byte)(232)))));
            this.txtIdeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIdeLog.ForeColor = System.Drawing.Color.Black;
            this.txtIdeLog.Location = new System.Drawing.Point(0, 0);
            this.txtIdeLog.Multiline = true;
            this.txtIdeLog.Name = "txtIdeLog";
            this.txtIdeLog.ReadOnly = true;
            this.txtIdeLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtIdeLog.Size = new System.Drawing.Size(661, 172);
            this.txtIdeLog.TabIndex = 1;
            // 
            // FrmLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 172);
            this.Controls.Add(this.txtIdeLog);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmLog";
            this.Text = "编译上传日志";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLog_FormClosing);
            this.Load += new System.EventHandler(this.FrmLog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtIdeLog;
    }
}