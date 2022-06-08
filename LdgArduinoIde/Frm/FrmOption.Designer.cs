namespace ArduinoIde2
{
    partial class FrmOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOption));
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.cmbDebug = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRelation = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.chkBuild_verbose = new System.Windows.Forms.CheckBox();
            this.chkUpload_verbose = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSmartTip = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIndentCol1Comments = new System.Windows.Forms.CheckBox();
            this.chkUnpadParen = new System.Windows.Forms.CheckBox();
            this.chkDeleteEmptyLines = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.cmbCodeStyle = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(189, 184);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 36);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "代码配色：";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(83, 52);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(134, 20);
            this.cmbTheme.TabIndex = 2;
            // 
            // cmbDebug
            // 
            this.cmbDebug.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDebug.FormattingEnabled = true;
            this.cmbDebug.Items.AddRange(new object[] {
            "无",
            "默认",
            "更多",
            "全部"});
            this.cmbDebug.Location = new System.Drawing.Point(83, 82);
            this.cmbDebug.Name = "cmbDebug";
            this.cmbDebug.Size = new System.Drawing.Size(134, 20);
            this.cmbDebug.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "编译警告：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "文件关联：";
            // 
            // btnRelation
            // 
            this.btnRelation.Location = new System.Drawing.Point(82, 144);
            this.btnRelation.Name = "btnRelation";
            this.btnRelation.Size = new System.Drawing.Size(135, 22);
            this.btnRelation.TabIndex = 6;
            this.btnRelation.Text = "关联.INO文件";
            this.btnRelation.UseVisualStyleBackColor = true;
            this.btnRelation.Click += new System.EventHandler(this.btnRelation_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "详细模式：";
            // 
            // chkBuild_verbose
            // 
            this.chkBuild_verbose.AutoSize = true;
            this.chkBuild_verbose.Checked = true;
            this.chkBuild_verbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBuild_verbose.Location = new System.Drawing.Point(85, 112);
            this.chkBuild_verbose.Name = "chkBuild_verbose";
            this.chkBuild_verbose.Size = new System.Drawing.Size(48, 16);
            this.chkBuild_verbose.TabIndex = 7;
            this.chkBuild_verbose.Text = "编译";
            this.chkBuild_verbose.UseVisualStyleBackColor = true;
            // 
            // chkUpload_verbose
            // 
            this.chkUpload_verbose.AutoSize = true;
            this.chkUpload_verbose.Checked = true;
            this.chkUpload_verbose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpload_verbose.Location = new System.Drawing.Point(139, 112);
            this.chkUpload_verbose.Name = "chkUpload_verbose";
            this.chkUpload_verbose.Size = new System.Drawing.Size(48, 16);
            this.chkUpload_verbose.TabIndex = 8;
            this.chkUpload_verbose.Text = "上传";
            this.chkUpload_verbose.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "智能提示：";
            // 
            // cmbSmartTip
            // 
            this.cmbSmartTip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSmartTip.FormattingEnabled = true;
            this.cmbSmartTip.Items.AddRange(new object[] {
            "无需提示",
            "详细提示",
            "仅提示名称"});
            this.cmbSmartTip.Location = new System.Drawing.Point(83, 5);
            this.cmbSmartTip.Name = "cmbSmartTip";
            this.cmbSmartTip.Size = new System.Drawing.Size(134, 20);
            this.cmbSmartTip.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(83, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "*需重新启动使其生效";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkIndentCol1Comments);
            this.groupBox1.Controls.Add(this.chkUnpadParen);
            this.groupBox1.Controls.Add(this.chkDeleteEmptyLines);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.cmbCodeStyle);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(238, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(361, 174);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "代码格式化选项";
            // 
            // chkIndentCol1Comments
            // 
            this.chkIndentCol1Comments.AutoSize = true;
            this.chkIndentCol1Comments.Location = new System.Drawing.Point(284, 18);
            this.chkIndentCol1Comments.Name = "chkIndentCol1Comments";
            this.chkIndentCol1Comments.Size = new System.Drawing.Size(72, 16);
            this.chkIndentCol1Comments.TabIndex = 8;
            this.chkIndentCol1Comments.Text = "注释缩进";
            this.chkIndentCol1Comments.UseVisualStyleBackColor = true;
            // 
            // chkUnpadParen
            // 
            this.chkUnpadParen.AutoSize = true;
            this.chkUnpadParen.Location = new System.Drawing.Point(209, 18);
            this.chkUnpadParen.Name = "chkUnpadParen";
            this.chkUnpadParen.Size = new System.Drawing.Size(72, 16);
            this.chkUnpadParen.TabIndex = 7;
            this.chkUnpadParen.Text = "紧凑括号";
            this.chkUnpadParen.UseVisualStyleBackColor = true;
            // 
            // chkDeleteEmptyLines
            // 
            this.chkDeleteEmptyLines.AutoSize = true;
            this.chkDeleteEmptyLines.Location = new System.Drawing.Point(134, 18);
            this.chkDeleteEmptyLines.Name = "chkDeleteEmptyLines";
            this.chkDeleteEmptyLines.Size = new System.Drawing.Size(72, 16);
            this.chkDeleteEmptyLines.TabIndex = 6;
            this.chkDeleteEmptyLines.Text = "清除空行";
            this.chkDeleteEmptyLines.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtCode);
            this.groupBox2.Location = new System.Drawing.Point(14, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(341, 123);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "风格预览";
            // 
            // txtCode
            // 
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.Location = new System.Drawing.Point(3, 17);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(335, 103);
            this.txtCode.TabIndex = 0;
            // 
            // cmbCodeStyle
            // 
            this.cmbCodeStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCodeStyle.FormattingEnabled = true;
            this.cmbCodeStyle.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cmbCodeStyle.Location = new System.Drawing.Point(77, 17);
            this.cmbCodeStyle.Name = "cmbCodeStyle";
            this.cmbCodeStyle.Size = new System.Drawing.Size(48, 20);
            this.cmbCodeStyle.TabIndex = 4;
            this.cmbCodeStyle.SelectedIndexChanged += new System.EventHandler(this.cmbCodeStyle_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "代码风格：";
            // 
            // FrmOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 231);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chkUpload_verbose);
            this.Controls.Add(this.chkBuild_verbose);
            this.Controls.Add(this.btnRelation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbDebug);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSmartTip);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbTheme);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "程序设置";
            this.Load += new System.EventHandler(this.FrmOption_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.ComboBox cmbDebug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRelation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkBuild_verbose;
        private System.Windows.Forms.CheckBox chkUpload_verbose;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSmartTip;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.ComboBox cmbCodeStyle;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkIndentCol1Comments;
        private System.Windows.Forms.CheckBox chkUnpadParen;
        private System.Windows.Forms.CheckBox chkDeleteEmptyLines;
    }
}