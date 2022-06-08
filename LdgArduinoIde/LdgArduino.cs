using AutocompleteMenuNS;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace LdgArduinoIde
{
    public class LdgArduino
    {
        private readonly GacArduinoHelper.GacToolTip toolTip = new GacArduinoHelper.GacToolTip();
        static Dictionary<string, ArduinoAutocompleteItem> dic = new Dictionary<string, ArduinoAutocompleteItem>();
        static  Dictionary<string, List<MethodAutocompleteItem>> dicMethods =new Dictionary<string, List<MethodAutocompleteItem>>();
        static AutocompleteMenu autocompleteMenu1 = new AutocompleteMenu();
        static int SmartTip = 0;
        //static ArduinoLexer arduinoLexer ;
        Scintilla scintilla;
        public LdgArduino(Scintilla scintilla)
        {
            this.scintilla= scintilla;
            scintilla.MouseDown += Scintilla_MouseDown;
            scintilla.MouseLeave += Scintilla_MouseLeave;
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla);
        }

        private void Scintilla_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(scintilla);
        }
        public string KeyWords1, KeyWords2, KeyWords3;

        static List<string> listKEYWORD1 = new List<string>();//关键字1 加粗
        static List<string> listKEYWORD2 = new List<string>();//关键字2
        static List<string> listKEYWORD3 = new List<string>();//语法类 goto if else for #include
        static List<string> listLITERAL1 = new List<string>();//常量
        public void ReadAllKey(string ArduinoIdePath,string UserProject)
        {
            if (File.Exists(ArduinoIdePath))
            {
                string dir = Path.GetDirectoryName(ArduinoIdePath);
                string keywordfile = dir + "\\lib\\keywords.txt";//系统核心关键字
                if (File.Exists(keywordfile))
                {
                    ReadKey(keywordfile);
                    string[] dirs = Directory.GetDirectories(dir+ "\\libraries");
                    foreach (var item in dirs)
                    {
                        string[] dirinfo = item.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                        dir= dirinfo[dirinfo.Length - 1];
                        
                        keywordfile = item + "\\keywords.txt";//扩展库关键字
                        if (File.Exists(keywordfile))
                        {
                            ReadKey(keywordfile,dir);
                           
                        }
                    }
                    //自己软件目录
                    if (Directory.Exists(Application.StartupPath + "\\Keywords"))
                    {
                        string[] files = Directory.GetFiles(Application.StartupPath + "\\Keywords", "*.txt");
                        foreach (var item in files)
                        {
                            string parentstr = Path.GetFileNameWithoutExtension(item);
                            ReadKey(item, parentstr);
                        }
                    }

                    //arduino appdata目录
                    string userdata = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"/Arduino15/";
                    dirs = Directory.GetDirectories(userdata,"libraries",SearchOption.AllDirectories);
                    foreach (var item in dirs)
                    {
                        string[] libdirs = Directory.GetDirectories(item);
                        foreach (var libdir in libdirs)
                        {
                            
                            string[] dirinfo = libdir.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            dir = dirinfo[dirinfo.Length - 1];

                            keywordfile = libdir + "\\keywords.txt";//扩展库关键字
                            if (File.Exists(keywordfile))
                            {
                                ReadKey(keywordfile, dir);

                            }
                        }
                       
                    }

                    //arduino Project目录
                    
                    dirs = Directory.GetDirectories(UserProject, "libraries", SearchOption.AllDirectories);
                    foreach (var item in dirs)
                    {
                        string[] libdirs = Directory.GetDirectories(item);
                        foreach (var libdir in libdirs)
                        {

                            string[] dirinfo = libdir.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                            dir = dirinfo[dirinfo.Length - 1];

                            keywordfile = libdir + "\\keywords.txt";//扩展库关键字
                            if (File.Exists(keywordfile))
                            {
                                ReadKey(keywordfile, dir);

                            }
                        }

                    }
                    KeyWords1 = string.Join(" ", listKEYWORD3);
                    KeyWords2 = string.Join(" ", listKEYWORD1) + " " + string.Join(" ", listKEYWORD2);
                    KeyWords3 = string.Join(" ", listLITERAL1);
                    scintilla.SetKeywords(0, KeyWords1);//For If Setup等
                    scintilla.SetKeywords(1, KeyWords2);//类和方法
                    scintilla.SetKeywords(3, KeyWords3);//常量类 LOW HIGH
                }
            }
            if (dic.ContainsKey("#"))
            {
                dic.Remove("#");
            }
            

        }
        public string CurrWord = "";
        private void Scintilla_MouseDown(object sender, MouseEventArgs e)
        {
            GetPositionWord();
           
            
        }
        public void ShowToolTip(AutocompleteItem autocompleteItem, Control control , Point point)
        {


            if (autocompleteItem._member != null)
            {
                toolTip.Show(autocompleteItem._member, control, point);
                return;
            }
            if (autocompleteItem._enum != null)
            {
                string s = "枚举名：" + autocompleteItem._enum.Name;
                if (!string.IsNullOrEmpty(autocompleteItem._enum.Value))
                {
                    s += "\r\n枚举值：" + autocompleteItem._enum.Value;
                }
                if (!string.IsNullOrEmpty(autocompleteItem._enum.Desc))
                {
                    s += "\r\n" + autocompleteItem._enum.Desc + " ";
                }
                s += "\r\n来自：" + autocompleteItem.ParentName + "的枚举选项";
                toolTip.ShowTip(s, control, point);
                return;

            }

            if (autocompleteItem._define != null)
            {
                string s = "常量名:" + autocompleteItem._define.Name;

                s += "\r\n常量值:" + autocompleteItem._define.Value;
                if (!string.IsNullOrEmpty(autocompleteItem._define.Desc))
                {
                    s += "\r\n" + autocompleteItem._define.Desc + " ";
                }
                toolTip.ShowTip(s, control, point);
                return;
            }
            string title = autocompleteItem.ToolTipTitle;
            string text = autocompleteItem.ToolTipText;

            if (string.IsNullOrEmpty(title))
            {
                toolTip.ToolTipTitle = null;
                toolTip.SetToolTip(control, null);
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                toolTip.ToolTipTitle = null;
                toolTip.ShowTip(title, control, point);//, Width + 3, 0, ToolTipDuration
            }
            else
            {
                toolTip.ToolTipTitle = title;
                toolTip.ShowTip(text, control, point);//, Width + 3, 0, ToolTipDuration
            }
        }


        private string GetPositionWord()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            var cor = scintilla.PointToClient(point);

            var pos = scintilla.CharPositionFromPoint(cor.X, cor.Y);
            int line = scintilla.LineFromPosition(pos);
            //Console.WriteLine("鼠标在第" + line + "行");
            int before = GetPosition(pos, line);
            int after = GetPosition(pos, line, false);
            //Console.WriteLine("获取位置:" + pos + "       截取位置:" + after + "  ||  " + after);
            string curr = scintilla.GetTextRange(before, after - before);
            Console.WriteLine("鼠标所在单词为:" + curr);
            CurrWord = curr;
            if (SmartTip == 1)
            {
                toolTip.Hide(scintilla);
                if(!string.IsNullOrEmpty(CurrWord))
                { 
                    if (CurrWord.Contains("."))
                    {
                        string[] strs = CurrWord.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                        if (strs.Length == 2)
                        {
                            ghc.GetCurrClassOrEnum(strs[0],before);
                            if (ghc.classInfo != null)
                            {
                                foreach (var item in ghc.classInfo.MemberList)
                                {
                                    if (item.Name==strs[1])
                                    {
                                        ShowToolTip(new GacMethod(item),scintilla, new Point(cor.X, cor.Y +12)); break;
                                    }

                                }

                            }
                            //如果存在枚举
                            if (ghc.enuminfo != null)
                            {
                                foreach (var item in ghc.enuminfo.ListEnum)
                                {
                                    ShowToolTip(new GacEnum(item, ghc.enuminfo.Name), scintilla, new Point(cor.X, cor.Y + 12));break;
                                }

                            }
                        }

                    }
                    else
                    {
                        foreach (AutocompleteItem item in autocompleteMenu1.sourceItems)
                        {
                            if (item.Text == CurrWord)
                            {
                                if (item.ToolTipTitle != null && item.ToolTipTitle.EndsWith("用户输入记录")) { continue; }
                                ShowToolTip(item, scintilla, new Point(cor.X, cor.Y + 12)); break;

                            }
                        }
                    }
                }
            }
            return curr;

        }
        private int GetPosition(int Pos, int Line, bool Before = true)
        {
            string str = "";
            if (Before)
            {
                str = scintilla.GetTextRange(scintilla.Lines[Line].Position, Pos - scintilla.Lines[Line].Position);
                //Console.WriteLine("之前文本:" + str);
            }
            else
            {
                str = scintilla.GetTextRange(Pos, scintilla.Lines[Line].EndPosition - Pos);
                //Console.WriteLine("之后文本:" + str);
            }

            for (int i = 0; i < str.Length; i++)
            {
                char c = Before ? str[str.Length - i - 1] : str[i];
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    if (Before)
                    {
                        if (c != '.')
                        {
                            return Pos - i;
                        }
                    }
                    else
                    {
                        return Pos + i;
                    }
                }
            }
            if (Before)
            {
                return scintilla.Lines[Line].Position;
            }
            else
            {
                return Pos;
            }


        }


        string[] snippets = { "if(^)\n{\n}", "if(^)\n{\n}\nelse\n{\n}", "for(^;;)\n{\n}", "while(^)\n{\n}", "do${\n^}while();", "switch(^)\n{\n\tcase : break;\n}" };
        GacHelpComplete ghc;
        public void AutoComplete(int smartTip=1)
        {
            autocompleteMenu1.ImageList = new System.Windows.Forms.ImageList();
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.类别);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.函数);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.常量);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.变量);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.声明);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.属性);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.枚举);
            autocompleteMenu1.ImageList.Images.Add(LdgArduinoIde.Properties.Resources.输入);


            //系统常用方法
            //foreach (var item in snippets)
            //    autoSystems.Add(new SnippetAutocompleteItem(item) { ImageIndex = 2});

            //foreach (var item in methods)
            //    items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            //foreach (var item in dic)
            //    autoSystemsTemp.Add(item.Value);
            ////this.autoSystems= dic.OrderBy(item => item.Value.Text);

            ////items.Add(new InsertSpaceSnippet());
            ////items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            ////items.Add(new InsertEnterSnippet());

            ////set as autocomplete source
            //this.autoSystems = autoSystemsTemp.OrderBy(item => item.Text);
            //autoSystemsTemp = autoSystemsTemp.OrderBy(item => item.Text).ToList< AutocompleteItem>();
            //autocompleteMenu1.SetAutocompleteItems(autoSystems);
            //autocompleteMenu1.SetAutocompleteItems(autoSystemsTemp);


            autocompleteMenu1.MinFragmentLength = 1;
            
            if (smartTip == 1) 
            {
                 ghc= new GacHelpComplete(scintilla);
                //第二代智能提示
                autocompleteMenu1.SetAutocompleteItems(ghc);
                
            }
            else if (smartTip == 2)
            {
                //第一代智能提示
                autocompleteMenu1.SetAutocompleteItems(new ArduinoDynamic(scintilla, dic));
                scintilla.CharAdded += Scintilla_CharAdded;

            }
            SmartTip = smartTip;



        }
        
        private void Scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Char == '.')
            {
                var currentPos = scintilla.CurrentPosition - 1;
                var wordStartPos = scintilla.WordStartPosition(currentPos, true);

                // Display the autocompletion list
                var lenEntered = currentPos - wordStartPos;
                if (lenEntered > 0)
                {
                    string lib = scintilla.GetTextRange(currentPos - lenEntered, lenEntered);
                    Console.WriteLine("输入字符:" + lib + "(" + lenEntered + ")");
                   
                    if (!dic.ContainsKey(lib))
                    {
                        Regex regVarType = new Regex("(\\w+)\\b[\\w,\\t ]*?\\b"+lib+"\\b[\\w,(;]*?", RegexOptions.Compiled);
                        string temp=scintilla.GetTextRange(0, wordStartPos);
                        Match match= regVarType.Match(temp);
                        if (match.Success)
                        {
                           if (match.Groups[1].Value[0] >= 'A' && match.Groups[1].Value[0] <= 'Z')
                            {
                                lib = match.Groups[1].Value;
                            }
                                
                        }
                    }
                    if (dic.ContainsKey(lib))
                    {
                        string parent = "";
                        if (dic[lib].ImageIndex == 0)
                        {
                            parent = dic[lib].ParentStr;
                            if (string.IsNullOrEmpty(parent))
                            {
                                if (dic.ContainsKey(lib))
                                {
                                    parent = lib;
                                }
                            }
                        }
                        else if(lib[0] >= 'A' && lib[0] <= 'Z')
                        {
                            parent = lib;
                        }
                        
                        
                        if (!string.IsNullOrEmpty(parent))
                        {
                            if (dicMethods.ContainsKey(parent))
                            {
                                scintilla.Tag= dicMethods[parent].ToList<ArduinoAutocompleteItem>();
                                //listAutocompleteItem = dicMethods[parent].Concat(autoSystems).ToList<ArduinoAutocompleteItem>();
                                //Console.WriteLine("智能提示更新为:"+listAutocompleteItem.Count);
                                     //dicMethods[parent];
                                

                            }
                        }
                    }
                }
            }
            else if(e.Char == ';'||e.Char=='\n')
            {

            }
        }
        string[] datatypes = new string[] { "array", "bool", "boolean", "byte", "char", "double", "float", "int", "long", "short", "size_t", "string" };
        public string GetLib(string lib)
        {
            string libold = lib;
            if (!dic.ContainsKey(lib))
            {
                Regex regVarType = new Regex("(\\w+)\\b[\\w,\\t ]*?\\b" + lib + "\\b[\\w,(;]*?", RegexOptions.Compiled);
                string temp = scintilla.GetTextRange(0, scintilla.CurrentPosition+lib.Length);
                Match match = regVarType.Match(temp);
                if (match.Success)
                {
                    if (match.Groups[1].Value[0] >= 'A' && match.Groups[1].Value[0] <= 'Z')
                    {
                        lib = match.Groups[1].Value;
                    }
                    if (datatypes.Contains(match.Groups[1].Value))
                    {
                        lib = match.Groups[1].Value;
                    }

                }
            }
            if (dic.ContainsKey(lib))
            {
                string parent = "";
                if (dic[lib].ImageIndex == 0)
                {
                    parent = dic[lib].ParentStr;
                    if (string.IsNullOrEmpty(parent))
                    {
                        if (dic.ContainsKey(lib))
                        {
                            parent = lib;
                        }
                    }
                }


                if (!string.IsNullOrEmpty(parent))
                {
                    if (dicMethods.ContainsKey(parent))
                    {
                        return parent;
                    }
                }
            }
            if (lib!=libold)
            {
                return lib;
            }
            return "";

        }

        static  Regex reg = new Regex("([#\\w]+)\\s+(\\w+)", RegexOptions.Compiled);
        static void ReadKey(string file,string Parent="")
        {
            Dictionary<string, List<MethodAutocompleteItem>> dicMethodsTemp = new Dictionary<string, List<MethodAutocompleteItem>>();
            string[] line = File.ReadAllLines(file);
            foreach (var item in line)
            {
                Match match = reg.Match(item);
                if (match.Success)
                {
                    string tiptype = match.Groups[2].Value;
                    string varstr = match.Groups[1].Value;
                    string inputCode = varstr;
                    int imageIndex = -1;
                    switch (tiptype)
                    {
                        case "KEYWORD1": listKEYWORD1.Add(varstr); imageIndex = 0; break;//类
                        case "KEYWORD2": listKEYWORD2.Add(varstr); imageIndex = 1; break;//func和method
                        case "KEYWORD3": listKEYWORD3.Add(varstr); imageIndex = 1; break;//func和method
                        case "KEYWORD4": listKEYWORD2.Add(varstr); imageIndex = 1; break; //func和method
                        case "LITERAL1": listLITERAL1.Add(varstr); imageIndex = 2; break;//const
                        default:
                            //Console.WriteLine("未知的类型" + tiptype + "   变量:"+ varstr);
                            break;
                    }
                    if (SmartTip==2)
                    {
                        if (!string.IsNullOrEmpty(varstr) && imageIndex == 1)
                        {
                            if (varstr.ToUpper() == varstr)
                            {
                                imageIndex = 5;
                            }
                            else if (varstr[0] >= 'A' && varstr[0] <= 'Z')
                            {
                                imageIndex = 4;
                                if (!dic.ContainsKey(varstr))
                                {
                                    dic.Add(varstr, new ArduinoAutocompleteItem() { Text = varstr, ParentStr = Parent, ImageIndex = imageIndex });
                                }

                            }
                        }
                        if (!string.IsNullOrEmpty(Parent))
                        {
                            if (tiptype == "KEYWORD2" || tiptype == "KEYWORD3" || tiptype == "KEYWORD4")
                            {
                                if (dic.ContainsKey(varstr) && dic[varstr].ImageIndex == 4)
                                {
                                    //已经有声明类存在了
                                }
                                else
                                {
                                    //没有声明类按照Methods
                                    if (!dicMethodsTemp.ContainsKey(Parent))
                                    {
                                        dicMethodsTemp.Add(Parent, new List<MethodAutocompleteItem>());
                                    }

                                    dicMethodsTemp[Parent].Add(new MethodAutocompleteItem(inputCode) { ImageIndex = imageIndex, MenuText = varstr });
                                }

                            }
                            else if (tiptype == "KEYWORD1" || tiptype == "LITERAL1")
                            {
                                if (!dic.ContainsKey(varstr))
                                {
                                    dic.Add(varstr, new ArduinoAutocompleteItem() { Text = inputCode, MenuText = varstr, ParentStr = Parent, ImageIndex = imageIndex });
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            if (!dic.ContainsKey(match.Groups[1].Value))
                            {
                                dic.Add(varstr, new ArduinoAutocompleteItem() { Text = inputCode, MenuText = varstr, ParentStr = Parent, ImageIndex = imageIndex });
                            }
                        }
                    }
                    
                }
            }
            if (SmartTip==2)
            {
                foreach (var itemkey in dicMethodsTemp.Keys)
                {
                    dicMethods[itemkey] = dicMethodsTemp[itemkey].OrderBy(item => item.Text).ToList<MethodAutocompleteItem>();
                }
            }
           
        }
        
    }
}
