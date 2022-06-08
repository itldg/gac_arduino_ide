using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Management;
using ScintillaNET;
using LdgArduinoIde;
using System.Text;
using GacArduinoHelper;

namespace ArduinoIde2
{
    public partial class FrmMain : Form
    {
        string currentFileName = "";
        public Scintilla scintilla;
        LdgIdeHelper lih;
        LdgArduino lad;
        IdeConfigInfo ic = new IdeConfigInfo();
       string welcome = "";
        bool NeedSave = false;
        public FrmMain()
        {
            InitializeComponent();

        }
        public FrmMain(string[] args)
        {
            InitializeComponent();
            
            if (File.Exists(args[0]))
            {
                currentFileName = args[0];
            }
        }
        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            NeedSave = true;
        }

        
        FrmLog fl;
        FrmCode fc = new FrmCode();
        class ComInfo {
             string _Text = "";
             string _Name = "";
             int _Com = 0;

            public string Text
            {
                get
                {
                    return _Text;
                }

                set
                {
                    _Text = value;
                }
            }

            public string Name
            {
                get
                {
                    return _Name;
                }

                set
                {
                    _Name = value;
                }
            }

            public int Com
            {
                get
                {
                    return _Com;
                }

                set
                {
                    _Com = value;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //CheckUpdate(false);
            
            this.scintilla = new Scintilla();
            lih = new LdgIdeHelper(scintilla);
            scintilla.Dock = DockStyle.Fill;

           
            fc.Controls.Add(scintilla);
            fc.Text = "新建代码";
            fc.Show(dp);
            //dp.Controls.Add(scintilla);
            scintilla.TextChanged += Scintilla_TextChanged;

           //更新左下角行号 列号和选择文字
            scintilla.UpdateUI += Scintilla_UpdateUI;
            //防止特殊字符
            scintilla.KeyPress += Scintilla_KeyPress;
            //热键F5和F6
            scintilla.KeyDown += Scintilla_KeyDown;

            //拖动打开
            scintilla.AllowDrop = true;
            scintilla.DragDrop += Scintilla_DragDrop;
            

        }
        protected override void WndProc(ref Message m)
        {
            if (fSerial!=null&&!fSerial.IsDisposed)
            {
                fSerial.WndProcNew(ref m);
            }
            base.WndProc(ref m);
        }

        private void Scintilla_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();       //获得路径
            if (path.EndsWith(".ino"))
            {
                if (path != currentFileName && File.Exists(path))
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath.ToString(), path);
                }
            }
        }



        private void Scintilla_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) == true)
                e.Handled = true;
        }

        private void Scintilla_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                UploadCode();
            }
            else if (e.KeyCode == Keys.F6)
            {
                CheckCode();
            }
        }

        private void Scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            tsslSel.Text = "第 " + (scintilla.CurrentLine+1) + " 行，第 " + (scintilla.GetColumn(scintilla.CurrentPosition)+1) + " 列";
            int selcount = scintilla.SelectedText.Length;
            if (selcount>0)
            {
                tsslSel.Text += " 选择了" + selcount + "个字符";
            }
        }



        #region 文件菜单
        void Option()
        {

            FrmOption fo = new FrmOption(ic);
            if (fo.ShowDialog() == DialogResult.OK)
            {
                if (fo.Config.ThemeName != ic.ThemeName)
                {
                    ic.ThemeName = fo.Config.ThemeName;
                    lih.UseStyle(ic.ThemeName);
                }
                ic.DebugModule = fo.Config.DebugModule;
                ic.BuildVerbose = fo.Config.BuildVerbose;
                ic.UploadVerbose = fo.Config.UploadVerbose;
                ic.SmartTip = fo.Config.SmartTip;

                ic.CodeStyle = fo.Config.CodeStyle;
                ic.DeleteEmptyLines = fo.Config.DeleteEmptyLines;
                ic.UnpadParen = fo.Config.UnpadParen;
                ic.IndentCol1Comments = fo.Config.IndentCol1Comments;
                IdeConfig.SaveConfig(ic);
            }
        }
        private void tsmiOption_Click(object sender, EventArgs e)
        {
            Option();
        }
        private void tsmiNew_Click(object sender, EventArgs e)
        {
            NewCode();
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            LoadCode();
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            SaveCode();
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();



        }
        void SaveAs()
        {
            string tempfile = currentFileName;

            currentFileName = "";
            if (!SaveCode(true))
            {
                currentFileName = tempfile;
            }
        }

        private void tsbOpen_Click(object sender, EventArgs e)
        {
            LoadCode();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveCode();
        }
        void LoadCode()
        {
            AskToSave();
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "打开您的项目代码";
                dlg.Filter = "Arduino|*.ino";
                dlg.CheckFileExists = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {

                 
                    AutoLoadCode(dlg.FileName);

                }
            }
            catch (Exception ex)
            {

                SetStatus("打开失败:" + ex.Message);
            }
            NeedSave = false;

        }
        void AutoLoadCode(string FileName)
        {
            SetStatus("正在等开文件");
            currentFileName = FileName;
            string project = Path.GetFileNameWithoutExtension(currentFileName);
            this.Text = project + " | " + title; ;
            fc.Text = project;
            string code = File.ReadAllText(currentFileName);
            scintilla.Text = code;
            SetStatus("打开成功");

            AddLately();

        }
        void AskToSave()
        {
            if (NeedSave&&MessageBox.Show("您的当前代码尚未保存,是否保存?","是否保存",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.OK)
            {
                SaveCode();
            }
            
        }
        /// <summary>
        /// 添加最近打开的文件
        /// </summary>
        void AddLately()
        {
            ic = IdeConfig.ReadConfig();
            List<string> listfiles = ic.Lately.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            if (listfiles.Contains(currentFileName))
            {
                listfiles.Remove(currentFileName);
                listfiles.Insert(0, currentFileName);
            }
            else
            {
                if (listfiles.Count >= 20)
                {
                    listfiles.RemoveAt(19);
                    listfiles.Insert(0, currentFileName);
                }
                else
                {
                    listfiles.Insert(0, currentFileName);
                }
            }

            ic.Lately = string.Join(";", listfiles);
            ReadLately();
            IdeConfig.SaveConfig(ic);
        }
        bool SaveCode(bool Show=true)
        {
            if (string.IsNullOrEmpty(currentFileName))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = "保存您的项目代码";
                dlg.Filter = "Arduino|*.ino";
                dlg.DefaultExt = ".ino";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //string filename = dlg.FileName;
                    //string dir = Path.GetDirectoryName(filename);
                    //string dirname = GetDir(dir);
                    //filename = Path.GetFileNameWithoutExtension(filename);
                    //if (filename != dirname)
                    //{
                    //    string newfile = dir + "\\" + dirname + "\\" + Path.GetFileName(dlg.FileName);
                    //    if (MessageBox.Show("Arduino项目需要同名文件夹,是否需要帮您创建同名文件夹?", "项目文件夹", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //    {
                    //        if (Directory.Exists(dir + "\\" + dirname) && File.Exists(newfile))
                    //        {
                    //            MessageBox.Show("已存在 " + dirname + " 文件夹,且已经有项目存在,请重新选择合适的保存位置", "项目存在", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //            return false;
                    //        }
                    //        else
                    //        {

                    //        }
                    //    }
                    //}

                    currentFileName = dlg.FileName;

                    string project= Path.GetFileNameWithoutExtension(dlg.FileName);
                    string dir = GetDir(Path.GetDirectoryName(dlg.FileName));
                    if (dir != project)
                    {
                        string newdir = Path.GetDirectoryName(dlg.FileName) + "\\" + project + "\\";
                        string newfile = newdir + Path.GetFileName(dlg.FileName);
                        if (MessageBox.Show("Arduino项目需要同名文件夹,是否需要帮您创建同名文件夹?", "项目文件夹", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (File.Exists(newfile))
                            {
                                MessageBox.Show("已存在 " + dir + " 文件夹,且已经有项目存在,请重新选择合适的保存位置", "项目存在", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                            else
                            {
                                if(!Directory.Exists(newdir))
                                {
                                    Directory.CreateDirectory(newdir);
                                }
                                currentFileName = newfile;
                            }
                        }
                    }

                    this.Text = project + " | " + title; ;
                    fc.Text = project;
                }
                else
                {

                    return false;
                }
            }
          
            try
            {
                NeedSave = false;
                File.WriteAllText(currentFileName, scintilla.Text);
                if (Show)
                {
                    SetStatus("保存成功");
                }
                AddLately();
                return true ;
            }
            catch (Exception ex)
            {
                SetStatus("保存失败:" + ex.Message);
                return false;
            }
        }
        void NewCode()
        {
            AskToSave();
            Text = "新建" + " | " + title;
            fc.Text = "新建";
            scintilla.Text = welcome;
            currentFileName = "";
            NeedSave = false;
        }
        

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            scintilla.Redo();
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            
            scintilla.Undo();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            NewCode();
        }
        void InitMenu(string project)
        {
            SetStatus("正在加载项目文件夹...");
            //项目文件夹
           
            AddMenus(project, tsmiProject);
            SetStatus("正在加载内置示例...");
            //内置示例
            string arduinopath = Path.GetDirectoryName(ic.IdePath);
            string userdata = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"/Arduino15/";
            AddMenu(arduinopath+ "\\examples", tsmiExamples, "官方示例");
            AddMenus(arduinopath + "\\libraries", tsmiExamples, "所有开发板示例");
            AddMenus(userdata,tsmiExamples,"");
            AddMenus(project, tsmiExamples, "第三方库示例");

            SetStatus("正在加载库信息...");
            //libraries
            AddMenu(arduinopath + "\\libraries", tsmiUseLib, "Arduino库",1);
            AddMenu(project + "\\libraries", tsmiUseLib, "自己使用库", 1);
            AddLibs(userdata); 

            //Boards  开发板
            //AddBoards(arduinopath);
            //AddBoards(userdata);


        }
        void AddMenus(string dir, ToolStripMenuItem tsmi, string Name = "")
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            string[] dirs = Directory.GetDirectories(dir, "examples", SearchOption.AllDirectories);
            if (dirs.Length>0)
            {
                if (tsmi.DropDownItems.Count != 0)
                {
                    tsmi.DropDownItems.Add(new ToolStripSeparator());
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    tsmi.DropDownItems.Add(Name);
                    tsmi= tsmi.DropDownItems[tsmi.DropDownItems.Count-1] as ToolStripMenuItem; ;
                    //tsmi.DropDownItems[tsmi.DropDownItems.Count - 1].Enabled = false;
                }
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var item in dirs)
                {
                    string tempname = "";
                   string[] dirstemp = item.Split(new string[] { @"\libraries\" }, StringSplitOptions.RemoveEmptyEntries);
                    if (dirstemp.Length > 0)
                    {
                        if (!dic.ContainsKey(dirstemp[0]))
                        {
                            string boards = dirstemp[0] + "\\boards.txt";
                            if (File.Exists(boards))
                            {
                                dic.Add(dirstemp[0], ArduinoIde.GetValue(boards, "generic.name",""));
                            }
                            else
                            {
                                dic.Add(dirstemp[0], "");
                            }
                        }
                        tempname = dic[dirstemp[0]];
                        if (!string.IsNullOrEmpty(tempname))
                        {
                            if (!tsmi.DropDownItems.ContainsKey(tempname))
                            {
                                tsmi.DropDownItems.Add(tempname +"  的例子");
                                tsmi.DropDownItems[tsmi.DropDownItems.Count - 1].Name = tempname;
                            }
                        }
                    }

                    List<ToolStripMenuItem> list = GetMenus(item);
                    if (list.Count>0)
                    {
                        ToolStripMenuItem tsmitemp;
                        if (!string.IsNullOrEmpty(tempname))
                        {
                            tsmitemp = tsmi.DropDownItems[tsmi.DropDownItems.Count - 1] as ToolStripMenuItem;
                        } else
                        {
                            tsmitemp = tsmi;
                        }
                        string examplename = GetLibName(item,2);
                        tsmitemp.DropDownItems.Add(examplename);
                        var s = tsmitemp.DropDownItems[tsmitemp.DropDownItems.Count - 1] as ToolStripMenuItem;
                        for (int i = 0; i < list.Count; i++)
                        {
                            s.DropDownItems.Add(list[i]);
                        }
                    }
                }

                
            }
        }
        Regex regJsonName = new Regex("\"name\": \"(.*?)\"", RegexOptions.Compiled);
        string GetLibName(string dir,int Parent)
        {

            string dirtemp = dir + "\\library.json";
            if (!File.Exists(dirtemp))
            {
                dirtemp = Path.GetDirectoryName(dir)+ "\\library.json";
            }
           
            if (File.Exists(dirtemp ))
            {
                string json = File.ReadAllText(dirtemp);
                Match match = regJsonName.Match(json);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return GetDir(dir, Parent);

        }
        void AddMenus(string dir, ToolStripMenuItem tsmi)
        {
            if (Directory.Exists(dir))
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (var item in dirs)
                {
                    if (item.EndsWith( "\\libraries")|| item.EndsWith("\\tools")) {continue;}
                    List<ToolStripMenuItem> list = GetMenus(item);
                    if (list.Count>0)
                    {
                        string dirinfo = GetLibName(item,1);
                        tsmi.DropDownItems.Add(dirinfo);
                        var s = tsmi.DropDownItems[tsmi.DropDownItems.Count - 1] as ToolStripMenuItem;
                        for (int i = 0; i < list.Count; i++)
                        {
                            s.DropDownItems.Add(list[i]);
                        }
                    }

                }
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Dir"></param>
        /// <param name="tsmi"></param>
        /// <param name="Name"></param>
        /// <param name="MenuType">0为示例 1为库 2为开发板</param>
        void AddMenu(string Dir, ToolStripMenuItem tsmi, string Name="",int MenuType=0)
        {
            List<ToolStripMenuItem> list=new List<ToolStripMenuItem>();
            if (MenuType == 0)
            {
                list = GetMenus(Dir);

            }
            else if (MenuType == 1)
            {
                list = GetLibs(Dir);
            }
            else if (MenuType == 2)
            {
                list = GetBoards(Dir);
            }
            
            if (list!=null&&list.Count > 0)
            {
                if (MenuType==0)
                {
                    if (tsmi.DropDownItems.Count != 0)
                    {
                        tsmi.DropDownItems.Add(new ToolStripSeparator());
                    }
                    if (!string.IsNullOrEmpty(Name))
                    {
                        tsmi.DropDownItems.Add(Name);
                        tsmi.DropDownItems[tsmi.DropDownItems.Count - 1].Enabled = false;
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        tsmi.DropDownItems.Add(list[i]);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        tsmi.DropDownItems.Add(Name);
                    }
                    var s = tsmi.DropDownItems[tsmi.DropDownItems.Count - 1] as ToolStripMenuItem;
                    for (int i = 0; i < list.Count; i++)
                    {
                        s.DropDownItems.Add(list[i]);
                    }
                }
                
             
                
            }
            Application.DoEvents();
        }
        void AddLibs(string dir)
        {
            string[] dirs = Directory.GetDirectories(dir, "libraries", SearchOption.AllDirectories);
            foreach (var item in dirs)
            {
                if (!item.Contains("\\tests\\"))
                {
                    AddMenu(item, tsmiUseLib, "贡献的库", 1);
                }
                
                //List < ToolStripMenuItem > list= GetLibs(item);
            }
        }
        Regex regBoardMenuName = new Regex("^name=(.*?)$", RegexOptions.Multiline);
        void AddBoards(string dir)
        {
            string[] dirs = Directory.GetDirectories(dir, "hardware", SearchOption.AllDirectories);
            foreach (var item in dirs)
            {
                string[] files = Directory.GetFiles(item, "boards.txt", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    string dirinfo = Path.GetDirectoryName(file);
                    string menuname = "";
                    if (File.Exists(dirinfo+ "\\platform.txt"))
                    {
                        string con = File.ReadAllText(dirinfo + "\\platform.txt");
                        Match match= regBoardMenuName.Match(con);
                        if (match.Success)
                        {
                            menuname = match.Groups[1].Value;
                        }

                    }
                    if (string.IsNullOrEmpty(menuname))
                    {
                        string name = GetDir(file, 3);
                        string ver = GetDir(file, 2);
                        menuname = name.ToUpper() + " " + ver + " Boards";
                        if (ver.Contains("."))
                        {
                            menuname = name.ToUpper() + " Boards" + " (" + ver + ")";
                        }
                    }
                    AddMenu(file, tsmiBoards, menuname, 2);
                }
            }
        }
        List<ToolStripMenuItem> GetMenus(string dir)
        {
            List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            if (Directory.Exists(dir))
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (var item in dirs)
                {

                    ToolStripMenuItem tsmi = GetProject(item);
                    if (tsmi != null)
                    {
                        list.Add(tsmi);
                    }
                }
            }
           
            return list;
            
        }
        ToolStripMenuItem GetProject(string dir)
        {
            ToolStripMenuItem tsmi= null;
            string dirinfo = GetDir(dir);
            string[] dirs = Directory.GetDirectories(dir);
            if (dirs.Length > 0)
            {
                foreach (var item in dirs)
                {
                    ToolStripMenuItem tsmitemp = GetProject(item);
                    if (tsmitemp!=null)
                    {
                        if (tsmi == null)
                        {
                            tsmi = new ToolStripMenuItem(dirinfo);
                        }
                        tsmi.DropDownItems.Add(tsmitemp);
                    }
                   
                        
                    
                }
                if (tsmi!=null)
                {
                    return tsmi;
                }
            }
            
            string file = dir + "\\" + dirinfo + ".ino";
            if (File.Exists(file))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                tsmi = new ToolStripMenuItem(name);
                tsmi.Tag = file;
                tsmi.Click += OpenMenuFile;
            }
            file = dir + "\\" + dirinfo + ".pde";
            if (File.Exists(file))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                tsmi = new ToolStripMenuItem(name);
                tsmi.Tag = file;
                tsmi.Click += OpenMenuFile;
            }


            return tsmi;
        }
        void ReadLately()
        {
            tsmiLately.DropDownItems.Clear();
            string[] itmes = ic.Lately.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in itmes)
            {
                if (File.Exists(item))
                {
                    string name = Path.GetFileNameWithoutExtension(item);
                    ToolStripMenuItem tsmitemp = new ToolStripMenuItem(name);
                    tsmitemp.Tag = item;
                    tsmitemp.Click += OpenMenuFile;
                    tsmiLately.DropDownItems.Add(tsmitemp);
                }
            }
        }

        private void OpenMenuFile(object sender, EventArgs e)
        {
            ToolStripMenuItem menu=sender as ToolStripMenuItem;
            string file = menu.Tag.ToString();
            if (file!=currentFileName&& File.Exists(file))
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath.ToString(), file);
            }
        }
        List<ToolStripMenuItem> GetLibs(string dir)
        {
            List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            if (Directory.Exists(dir))
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (var item in dirs)
                {

                    ToolStripMenuItem tsmi = GetLib(item);
                    if (tsmi != null)
                    {
                        list.Add(tsmi);
                    }
                }
            }

            return list;

        }
        ToolStripMenuItem GetLib(string dir)
        {
            ToolStripMenuItem tsmi = null;
            int parent = 1;
            if (Directory.Exists(dir+"\\src"))
            {
                dir += "\\src";
                parent = 2;
            }
            string name = GetLibName(dir, parent);
            StringBuilder sb = new StringBuilder();
            string[] files = Directory.GetFiles(dir, "*.h");
            foreach (var item in files)
            {
                if (ic.SmartTip==1)//为智能提示生成xml文件
                {
                    GacHelper.SearchH(item, name,0);
                }
                
                string file = Path.GetFileName(item);
                sb.Append(file + ";");
            }
            if (sb.Length>0)
            {
                
                tsmi = new ToolStripMenuItem(name);
                tsmi.Tag = sb.ToString();
                tsmi.Click += UseLib;
            }
            
            return tsmi;
        }
        Regex regBoardsName = new Regex("^(\\w+)\\.name=(.*?)$", RegexOptions.Multiline);
        Regex regBoardsCpu = new Regex("^nano\\.menu\\.cpu\\.(\\w+)\\=(.*?)$");
        List<ToolStripMenuItem> GetBoards(string file)
        {
            List<ToolStripMenuItem> list = null;
            string con= File.ReadAllText(file);
            MatchCollection mcs = regBoardsName.Matches(con);
            foreach (Match match in mcs)
            {
                string name = match.Groups[1].Value;
                string variant = GetBoardsInfo(con, name, "build.variant");
                //string variant = GetBoardsInfo(con, name, "build.variant");
                //string variant = GetBoardsInfo(con, name, "build.variant");
                //MatchCollection mcsCpu = regBoardsCpu.Matches(con);
            }

            //int parent = 1;
            //if (Directory.Exists(dir + "\\src"))
            //{
            //    dir += "\\src";
            //    parent = 2;
            //}
            //StringBuilder sb = new StringBuilder();
            //string[] files = Directory.GetFiles(dir, "*.h");
            //foreach (var item in files)
            //{
            //    string file = Path.GetFileName(item);
            //    sb.Append(file + ";");
            //}
            //if (sb.Length > 0)
            //{
            //    string name = GetDir(dir, parent);
            //    tsmi = new ToolStripMenuItem(name);
            //    tsmi.Tag = sb.ToString();
            //    tsmi.Click += UseLib;
            //}

            return list;
        }
        string GetBoardsInfo(string con, string name, string key)
        {
            Regex regBoardInfo = new Regex("^"+name+"\\."+key.Replace(".","\\.")+"=(.*?)$",RegexOptions.Multiline); 
            Match match= regBoardInfo.Match(con);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }
        private void UseLib(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            string[] files = menu.Tag.ToString().Split(new string[] { ";"},StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in files)
            {
                string use = "#include <" + item + ">";
                if (scintilla.Text.IndexOf(use)==-1)//没引用过
                {
                    scintilla.InsertText(0, use + "\n");
                }
            }
        }
        string GetDir(string dir,int Parent=1)
        {
            string[] dirs = dir.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (dirs.Length> Parent)
            {
                string dirinfo = dirs[dirs.Length - Parent];
                return dirinfo;
            }
            return "";
            
        }
        #endregion

        #region 验证和上传
        private void tsmiCheck_Click(object sender, EventArgs e)
        {
            CheckCode();
        }

        private void tsmiUpload_Click(object sender, EventArgs e)
        {
            UploadCode();
        }

        private void tsbCheck_Click(object sender, EventArgs e)
        {
            CheckCode();
        }

        private void tsbUpload_Click(object sender, EventArgs e)
        {
            UploadCode();
        }
        void CheckCode()
        {
            if (SaveCode(false))
            {
                CmdCurr = "验证编译";
                ExcuteDosCommand(ic.IdePath, (ic.BuildVerbose ? " --verbose" : "")+" --verify " + currentFileName);
            }

        }
        bool ReSerial = false;
        void UploadCode()
        {
            if (SaveCode(false))
            {
//                fSerial.IsActivated
                if (fSerial != null &&!fSerial.IsDisposed&&fSerial.IsOpen)
                {
                    ReSerial = true;
                    fSerial.CloseSerial();
                }
                CmdCurr = "编译上传";
                
                ExcuteDosCommand(ic.IdePath, (ic.UploadVerbose ? " --verbose" : "")+" --upload " + currentFileName);
            }
        }
        Process CmdProcess = null;
        int ErrorLine = -1;
        int ErrorColumn = -1;
        string ErrorMsg = "";
        Regex regIdeError = new Regex("\\w+:(\\d+):(\\d+): error: (.*?)$");
        string CmdCurr = "";
        
        private void ExcuteDosCommand(string StartFileName, string StartFileArg)
        {
            StartFileArg = "--port " + ic.COM + "  " + StartFileArg;
            if (fl==null||fl.IsDisposed)
            {
                fl = new FrmLog();
                fl.Show(dp, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            }
            fl.Activate();
            //fl.Focus();
            SetStatus( "正在" + CmdCurr + "项目...");
            if (CmdProcess!=null)
            {
                try
                {
                    CmdProcess.Close();
                }
                catch (Exception)
                {

                }
                finally {
                    CmdProcess =null;
                }
            }
            ErrorLine = -1;
            ErrorColumn = -1;
            ErrorMsg = "";
            if (!fl.IsDisposed)
            {
                fl.txtIdeLog.Clear();
            }
           
            CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName;      // 命令
            CmdProcess.StartInfo.Arguments = StartFileArg;      // 参数
            Console.WriteLine("执行命令:" + StartFileName + " " + StartFileArg);
            
            CmdProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;// 指定编码
            CmdProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;// 指定编码
            CmdProcess.StartInfo.CreateNoWindow = true;         // 不创建新窗口
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true;  // 重定向输入
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            CmdProcess.StartInfo.RedirectStandardError = true;  // 重定向错误输出
                                                                //CmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            CmdProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            CmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);

            CmdProcess.EnableRaisingEvents = true;                      // 启用Exited事件
            CmdProcess.Exited += new EventHandler(CmdProcess_Exited);   // 注册进程结束事件

            CmdProcess.Start();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();

            // 如果打开注释，则以同步方式执行命令，此例子中用Exited事件异步执行。
            // CmdProcess.WaitForExit();     
        }
        private void CodeBeautify( string StartFileArg)
        {
            
            Process codeBeautify = new Process();
            codeBeautify.StartInfo.FileName = Application.StartupPath + @"\System\Lib\AStyle.exe";      // 命令
            codeBeautify.StartInfo.Arguments = StartFileArg;      // 参数
            Console.WriteLine("执行命令:" + Application.StartupPath + @"\System\Lib\AStyle.exe" + " " + StartFileArg);
            codeBeautify.StartInfo.CreateNoWindow = true;         // 不创建新窗口
            codeBeautify.StartInfo.UseShellExecute = false;
            codeBeautify.StartInfo.RedirectStandardInput = true;  // 重定向输入
            codeBeautify.Start();
            codeBeautify.WaitForExit();     
        }
        private void CmdProcess_Exited(object sender, EventArgs e)
        {
           
            Console.WriteLine("CMD退出");
            if (ErrorLine >= 0)
            {
                this.Invoke((MethodInvoker)delegate ()
                {

                    AddIedLog("第" + ErrorLine + "行存在错误,错误提示:" + ErrorMsg);
                    scintilla.Lines[ErrorLine].Goto();
                    scintilla.Focus();
                    SetStatus(CmdCurr + "出错");

                });
            }
            else if (!string.IsNullOrEmpty(ErrorMsg))
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    AddIedLog(CmdCurr + "出错,错误提示:" + ErrorMsg);
                    SetStatus(CmdCurr + "出错");
                });
            }
            else {
                this.Invoke((MethodInvoker)delegate ()
                {
                    SetStatus(CmdCurr + "完成");
                });
                if (ReSerial && fSerial != null && !fSerial.IsDisposed)
                {
                    ReSerial = false;
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        fSerial.Activate();
                    });
                    fSerial.OpenSerial();
                
                }
            }
        }
        Regex regWriting = new Regex("Writing at \\w+... \\((.*?)\\)", RegexOptions.Compiled);
        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data)&&!this.IsDisposed)
            {
                Console.WriteLine("CMD结果1:" + e.Data);
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (e.Data == "exit status 1")
                    {
                        ErrorMsg="编译失败或上传失败";
                    }
                    else if (e.Data == "exit status 2")
                    {
                        ErrorMsg = "找不到草图";
                    }
                    else if (e.Data == "exit status 3")
                    {
                        ErrorMsg = "无效的（参数）命令行选项";
                    }
                    else if (e.Data == "exit status 4")
                    {
                        ErrorMsg = "传递给--get-pref的首选项不存在";
                    }
                    else if (e.Data == "上传项目出错")
                    {
                        ErrorMsg = e.Data;
                    }
                    else if (e.Data.EndsWith("response code=404"))//忽略请求失败的日志
                    {

                    }

                    else
                    {
                        if (e.Data.EndsWith("...") && !e.Data.Contains("StatusLogger"))
                        {
                            SetStatus(e.Data);
                        }

                        if (e.Data.StartsWith("DEBUG ") || e.Data.Contains(" INFO "))
                        {
                            if (ic.DebugModule > 0)
                            {
                                AddIedLog(e.Data);
                            }
                        }
                        else if (e.Data.StartsWith("TRACE "))
                        {
                            if (ic.DebugModule > 1)
                            {
                                AddIedLog(e.Data);
                            }
                        }
                        else if (e.Data.StartsWith("INFO "))
                        {
                            if (ic.DebugModule > 2)
                            {
                                AddIedLog(e.Data);
                            }

                        }
                        else
                        {
                            if (e.Data.StartsWith("Writing at "))
                            {
                                Match matchWriting = regWriting.Match(e.Data);
                                if (matchWriting.Success)
                                {
                                    SetStatus("上传进度 " + matchWriting.Groups[1].Value);
                                }
                            }


                            AddIedLog(e.Data);
                        }

                        if (ErrorLine < 0)
                        {
                            Match match = regIdeError.Match(e.Data);
                            if (match.Success)
                            {
                                ErrorLine = Convert.ToInt32(match.Groups[1].Value);
                                ErrorColumn = Convert.ToInt32(match.Groups[2].Value);
                                ErrorMsg = match.Groups[3].Value;
                            }
                        }

                    }
                });
            }
        }

        private void p_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                
                Console.WriteLine("CMD结果2:" + outLine.Data);
                
                this.Invoke((MethodInvoker)delegate ()
                {
                    
                        AddIedLog(outLine.Data );

                    Application.DoEvents();
                    
                });
            }
        }
        private void AddIedLog(string msg)
        {
            if (!fl.IsDisposed)
            {
                fl.txtIdeLog.AppendText(msg + "\r\n");
            }
            

        }

        #endregion

        void SetStatus(string msg)
        {
            this.Invoke((MethodInvoker)delegate () { 
                tsslStatus.Text = msg;
                Application.DoEvents();
            });


        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //System.Environment.Exit(0);
        }
        string title = "";
        private void FrmMain_Shown(object sender, EventArgs e)
        {
            SetStatus("正在读取程序配置信息...");
            ic = IdeConfig.ReadConfig();
            if (string.IsNullOrEmpty(ic.IdePath)||!File.Exists(ic.IdePath))
            {
                FrmIdePath fip = new FrmIdePath();
                if (fip.ShowDialog() == DialogResult.OK)
                {
                    ic.IdePath = fip.txtPath.Text;
                    IdeConfig.SaveConfig(ic);
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            SetStatus("正在读取默认代码模板...");
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("LdgArduinoIde.Welcome.txt"))
            {

                byte[] StreamData = new byte[s.Length];
                s.Read(StreamData, 0, (int)s.Length);

                welcome = (new System.Text.UTF8Encoding(false).GetString(StreamData));
                string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string name = GetAssemblyTitle() ;
                title = name + "   " + ver;
                this.Text = title;
                welcome = welcome.Replace("{ver}", ver);
                welcome = welcome.Replace("{name}", name);
            }

            
            if (string.IsNullOrEmpty(currentFileName))
            {
                scintilla.Text = welcome;
                if (ic.Lately!="")
                {
                    string[] files = ic.Lately.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (files.Length>0)
                    {
                        if (File.Exists(files[0]))
                        {
                            AutoLoadCode(files[0]);
                        }
                    }
                }
                
            }
            else
            {
                AutoLoadCode(currentFileName);
            }

            SetStatus("正在加载主题1...");
            lih.UseStyle(ic.ThemeName);


            string project = ArduinoIde.GetValue("sketchbook.path");
            SetStatus("正在初始化菜单选项...");
            InitMenu(project);

            SetStatus("正在读取历史记录...");
            ReadLately();
            NeedSave = false;
            tsmiCom.MouseHover += TsmiCom_MouseHover;

            SetStatus("正在加载串口信息...");
            ReadCom();

            SetStatus("正在加载智能提示(安装或引用新的库加载会稍慢一些)...");
            //智能提示
            lad = new LdgArduino(scintilla);
            GacHelper.InitSystem(ic.IdePath);
            lad.ReadAllKey(ic.IdePath, project);
            lad.AutoComplete(ic.SmartTip);

            SetStatus("正在加载主题2...");
            lih.UseStyle(ic.ThemeName);


            menuStrip1.Enabled = toolStrip1.Enabled = true;
            SetStatus("准备就绪");

        }



        private void TsmiCom_MouseHover(object sender, EventArgs e)
        {
            ReadCom();
        }
        Regex regCom = new Regex("(.*?) \\(COM(\\d+)\\)");

        void ReadCom()
        {
            try
            {
                Console.WriteLine("重新加载串口");
                tsmiCom.DropDownItems.Clear();
                var search = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_PnPEntity");
                foreach (var hardInfo in search.Get())
                {
                    if (hardInfo.Properties["Name"].Value != null && hardInfo.Properties["Name"].Value.ToString().Contains("(COM"))
                    {
                        String strComName = hardInfo.Properties["Name"].Value.ToString();
                        Match match = regCom.Match(strComName);
                        if (match.Success)
                        {
                            string com = ("COM" + match.Groups[2].Value.ToString());
                            string name = match.Groups[1].Value == "" ? "" : "  (" + match.Groups[1].Value + ")";
                            ToolStripMenuItem tsmi = new ToolStripMenuItem(com + name);
                            tsmi.Click += Tsmi_Click;
                            tsmi.Tag = com;
                            if (ic.COM == com)
                            {
                                tsmi.Checked = true;
                            }
                            tsmiCom.DropDownItems.Add(tsmi);
                            //listCom.Add(new ComInfo() { Text = str, Com = Convert.ToInt32(match.Groups[2].Value), Name = match.Groups[1].Value });
                        }
                        Console.WriteLine(strComName);//打印串口设备名称及串口号
                    }
                }               
                //string str = mo1["Name"].ToString();
                //Console.WriteLine(str);

            }

            //string[] ports = SerialPort.GetPortNames();
            //Array.Sort(ports);//数组排序
            //Console.WriteLine(string.Join("\n", ports));
            catch (Exception)
            {
                SetStatus("串口信息加载失败,尝试方案2");
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                foreach (var item in ports)
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(item);
                    tsmi.Click += Tsmi_Click;
                    tsmi.Tag = item;
                    if (ic.COM == item)
                    {
                        tsmi.Checked = true;
                    }
                    tsmiCom.DropDownItems.Add(tsmi);
                }
            }
        }

        private void Tsmi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            ic.COM = menu.Tag.ToString();
            IdeConfig.SaveConfig(ic);
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
        #region 编辑
        private void tsmicopyAsHtml_Click(object sender, EventArgs e)
        {
            string html = scintilla.GetTextRangeAsHtml(scintilla.SelectionStart, scintilla.SelectionEnd);
            Clipboard.SetText(html);
        }

        private void tsmiCopyHtml_Click(object sender, EventArgs e)
        {
            scintilla.Copy(CopyFormat.Text | CopyFormat.Html | CopyFormat.Rtf);
        }
        private void 折叠全部ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.FoldAll(FoldAction.Contract);
        }

        private void 展开全部ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.FoldAll(FoldAction.Expand);
        }

        private void 默认大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla.Zoom = 0;
            
        }
        FrmFindAndReplace ffr = null;
        private void tsmiFind_Click(object sender, EventArgs e)
        {


            try
            {
                if (ffr == null)
                {
                    ffr = new FrmFindAndReplace(scintilla); ffr.Show();
                }
                else
                {
                    ffr.Visible = true;
                }



            }
            catch (Exception)
            {

            }
        }

        private void 查找下一个ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ffr != null)
            {
                ffr.FindNext();
            }
        }

        private void 查找上一个ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ffr != null)
            {
                ffr.FindPrev();
            }
        }
        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            scintilla.Undo();
        }

        private void tsmiRedo_Click(object sender, EventArgs e)
        {
            scintilla.Redo();
        }
        private void tsmiGoLine_Click(object sender, EventArgs e)
        {
            GoLine();
        }
        void GoLine()
        {
            int CurrLine = (scintilla.CurrentLine + 1);
            int CurrColumn = (scintilla.GetColumn(scintilla.CurrentPosition) + 1);
            int LastLine = scintilla.Lines.Count;
            FrmGoToLine fgl = new FrmGoToLine(CurrLine,CurrColumn,LastLine);
            if (fgl.ShowDialog() == DialogResult.OK)
            {
                int line = fgl.Line - 1;
                int column = fgl.Column - 1;
                scintilla.Lines[line].Goto();
                if (scintilla.Lines[line].EndPosition - scintilla.Lines[line].Position > fgl.Column)
                {
                    scintilla.GotoPosition(scintilla.Lines[line].Position + column);
                    scintilla.Focus();
                }
                
            }
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            
            this.Close();

        }

        private void tsmiMarkPrev_Click(object sender, EventArgs e)
        {
            lih.MarkPrev();
        }

        private void tsmiMarkNext_Click(object sender, EventArgs e)
        {
            lih.MarkNext();
        }
        private void tsmiZoomIn_Click(object sender, EventArgs e)
        {
            scintilla.ZoomIn();
        }

        private void tsmiZoomOut_Click(object sender, EventArgs e)
        {
            scintilla.ZoomOut();
        }

        private void tsmiSwitchMark_Click(object sender, EventArgs e)
        {
            lih.MarkSwitch(scintilla.CurrentPosition);
        }
        #endregion

        #region 项目
        FrmSerial fSerial;
        void OpenSerial()
        {
            if (fSerial == null || fSerial.IsDisposed)
            {
                fSerial = new FrmSerial(ic.COM);
            }

            //fSerial.Show(dp, WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide);
            fSerial.Show(dp, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            if (dp.DockBottomPortion < 200)
            {
                dp.DockBottomPortion = 200;
            }
        }
        private void tsbSerial_Click(object sender, EventArgs e)
        {
            OpenSerial();
        }

        private void 串口调试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSerial();
        }
        private void tsmiOpenDir_Click(object sender, EventArgs e)
        {
            OpenDir();
        }
        void OpenDir()
        {
            if (!string.IsNullOrEmpty(currentFileName))
            {
                if (!System.IO.File.Exists(currentFileName)) return;
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = " /select," + currentFileName;
                System.Diagnostics.Process.Start(psi);
            }
        }


        #endregion

        

       
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AskToSave();
            this.Visible = false;
        }

        private void tsbOpenArduinoIde_Click(object sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(ic.IdePath);
            string filename = dir + "\\arduino.exe";
            if (File.Exists(filename))
            {
                System.Diagnostics.Process.Start(filename,currentFileName);
            }
            else
            {
                MessageBox.Show("Arduino编辑器文件未找到,清检查配置!", "打开失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #region 帮助
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
             new FrmAbout().ShowDialog();
        }




        private void 访问ArduinoccToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.arduino.cc/");
        }

        private void 入门ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(ic.IdePath);
            string file = dir + "/reference/www.arduino.cc/en/Guide/Windows.html";
            if (File.Exists(file))
            {
                System.Diagnostics.Process.Start(file);
            }
            else
            {
                MessageBox.Show("本机入门文件丢失", "打开失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }
        private void 访问GacccToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gac.cc/");
        }

        #endregion

        private void tsmiCheckUpdate_Click(object sender, EventArgs e)
        {
            //CheckUpdate();

        }
        void CheckUpdate(bool ShowError=true)
        {
            if (File.Exists("Update.exe"))
            {
                System.Diagnostics.Process.Start("Update.exe", ShowError ? "" : "auto");
            }
            else if(ShowError)
            {
                MessageBox.Show("自动更新文件丢失，请重新安装或找回更新文件", "更新失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void tsmiSearchHelp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lad.CurrWord))
            {
                SetStatus("请先选择一个您要查找的关键词");
            }
            else
            {
                string helpfile = Path.GetDirectoryName(ic.IdePath)+ @"\reference\www.arduino.cc\en\";
                if (lad.CurrWord.Contains("."))
                {
                    string temphelp= lad.CurrWord.Replace(".", "/")+".html";
                    if (File.Exists(helpfile+temphelp))
                    {
                        helpfile += temphelp;
                    }
                   
                }
                if (!helpfile.EndsWith(".html")&& lad.CurrWord.Contains("."))
                {
                     string[] strs= lad.CurrWord.Split(new string[] { "." },StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length>1)
                    {
                        string lib = lad.GetLib(strs[0]);
                        if (!string.IsNullOrEmpty(lib))
                        {
                            string temphelp = "Reference/"+ lib + strs[1]+".html";
                            if (File.Exists(helpfile + temphelp))
                            {
                                helpfile += temphelp;
                            }
                        }
                    }
                    
                }
                else
                {
                    string lib = lad.GetLib(lad.CurrWord);
                    if (!string.IsNullOrEmpty(lib))
                    {
                        string temphelp = "Reference/" + lib + ".html";
                        if (File.Exists(helpfile + temphelp))
                        {
                            helpfile += temphelp;
                        }
                    }
                    if (!helpfile.EndsWith(".html"))
                    { 
                        string temphelp = "Reference/" + lad.CurrWord + ".html";
                        if (File.Exists(helpfile + temphelp))
                        {
                            helpfile += temphelp;
                        }
                    }

                   


                }
                
                if (helpfile.EndsWith(".html"))
                {
                    SetStatus("已为您打开\" " + lad.CurrWord + " \"的帮助文档");
                    System.Diagnostics.Process.Start(helpfile);
                }
                else
                {
                    SetStatus("未找到与\" " + lad.CurrWord + " \"相关的帮助文档");
                }
                
            }
        }
        bool FileExists(string filename)
        {
            if (File.Exists(filename))
            {
                FileInfo fileinfo = new FileInfo(filename);
                if (fileinfo.FullName==filename)
                {
                    return true;
                }

            }
            return false;
        }

        private void tsmiCodeBase_Click(object sender, EventArgs e)
        {
            OpenCodeBase();
        }
        FrmCodeBase fcb;
        void OpenCodeBase()
        {
            if (fcb==null||fcb.IsDisposed)
            {
                fcb = new FrmCodeBase(lad. KeyWords1, lad.KeyWords2, lad.KeyWords3, ic.ThemeName);
                fcb.InsertCode += Fcb_InsertCode;
            }
            fcb.Show(dp, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            if (dp.DockRightPortion < 300)
            {
                dp.DockRightPortion = 300;
            }
        }

        private void Fcb_InsertCode(string Include, string Code)
        {
            int currPosition = scintilla.CurrentPosition;//鼠标开始位置
           
            int last = scintilla.Text.LastIndexOf("#include");
            if (last == -1)
            {
                last = 0;
            }
            else
            {
                int line = scintilla.GetColumn(last) + 1;
                int start = scintilla.Lines[line].Position;
                last = start;
            }
            int newlast = last;
            string[] includes = Include.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var include in includes)
            {
                if (scintilla.Text.IndexOf(include) == -1)//没引用过
                {
                    string use = include + "\n";
                    scintilla.InsertText(newlast, use);
                    newlast += use.Length;
                }
            }
            Application.DoEvents();
            
            scintilla.InsertText(currPosition + newlast - last,Code.TrimStart());
           
            Console.WriteLine("要插入" + Code);//
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenCodeBase();
        }

        private void tsmiCodeBeautify_Click(object sender, EventArgs e)
        {
            if (SaveCode(false))
            {
                SetStatus("正在格式化代码.");
                string par = @"-A" + (ic.CodeStyle+1);
                if (ic.DeleteEmptyLines)
                {
                    par += " --delete-empty-lines";
                }
                if (ic.UnpadParen)
                {
                    par += " --unpad-paren";
                }
                if (ic.IndentCol1Comments)
                {
                    par += " --indent-col1-comments";
                }
                par += " --suffix=none " + currentFileName;
                CodeBeautify( par);
                string code = File.ReadAllText(currentFileName);
                int pos = scintilla.CurrentPosition;
                scintilla.Text = code;
                if (scintilla.TextLength > pos)
                {
                    scintilla.GotoPosition(pos);
                    //scintilla.CurrentPosition = pos;
                }
                else
                {
                    scintilla.GotoPosition(scintilla.TextLength - 1);
                   //scintilla.CurrentPosition = scintilla.TextLength - 1;
                }
                SetStatus("代码格式化完成.");
            }
        }
    }
}
