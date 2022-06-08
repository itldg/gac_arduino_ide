using AutocompleteMenuNS;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GacArduinoHelper;

namespace LdgArduinoIde
{

    internal class GacHelpComplete : IEnumerable<AutocompleteItem>
    {
        private Scintilla scintilla;

        string[] snippets = { "if(^)\n{\n}", "if(^)\n{\n}\nelse\n{\n}", "for(^;;)\n{\n}", "while(^)\n{\n}", "do${\n^}while();", "switch(^)\n{\n\tcase : break;\n}" };


        GacHelper gh = new GacHelper();
        string lastClass = "";
       public  ClassInfo classInfo;
        public  EnumInfo enuminfo;
        public Struct structinfo;
        public List<Member> memberinfo = new List<Member>();
        public GacHelpComplete(Scintilla scintilla)
        {
            
            this.scintilla = scintilla;
            scintilla.Insert += Scintilla_Insert;
            scintilla.CharAdded += Scintilla_CharAdded;
            InitDataType();
            gh.GetHelp(scintilla.Text);
        }

        private void Scintilla_Insert(object sender, ModificationEventArgs e)
        {
            Console.WriteLine("更新智能提示前输入文本" + e.Text);
            if (e.Text.Contains("#include") || e.Text.EndsWith(">"))
            {

                //更新智能提示
                Console.WriteLine("更新智能提示");
                gh.GetHelp(scintilla.Text);
            }
        }

        Dictionary<string, string> dicDataTypes = new Dictionary<string, string>();
        void InitDataType()
        {
            dicDataTypes.Add("array", "数组");
            dicDataTypes.Add("bool", "bool拥有两个值之一，true即false。（每个bool变量占用一个字节的内存。）");
            dicDataTypes.Add("boolean", "boolean是boolArduino定义的非标准类型别名。建议改用bool相同的标准类型");
            dicDataTypes.Add("byte", "字节型");
            dicDataTypes.Add("char", "字符型");
            dicDataTypes.Add("double", "双精度浮点型");
            dicDataTypes.Add("float", "浮点型");
            dicDataTypes.Add("int", "整型");
            dicDataTypes.Add("long", "长整型");
            dicDataTypes.Add("short", "短整型");
            dicDataTypes.Add("String", "字符串型");
            dicDataTypes.Add("size_t", "size_t是一种数据类型，能够以字节为单位表示任何对象的大小。使用的例子的size_t是返回类型sizeof()和Serial.print()。");
            dicDataTypes.Add("unsigned char", "无符号字符型");
            dicDataTypes.Add("unsigned int", "无符号整数");
            dicDataTypes.Add("unsigned long", "无符号长整型");
            dicDataTypes.Add("void", "空");
            dicDataTypes.Add("word", "字型");
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
                    if (lastClass==lib)
                    {
                        return;
                    }
                    GetCurrClassOrEnum(lib, wordStartPos);


                 }
            }
            else if (e.Char == ';' || e.Char == '\n')
            {

            }
        }
        public void GetCurrClassOrEnum(string lib,int wordStartPos)
        {
            bool IsHas = false;
            //string hasClass = lib;
            //检查所有公开静态类
            foreach (var item in gh.GacComplete)
            {
                //foreach (var Extern in item.Value.DicExtern)
                //{
                //    if (Extern.Key == hasClass)
                //    {
                //        hasClass = Extern.Value; break;
                //    }
                //}
                foreach (var classinfo in item.Value.listClass)
                {
                    if (classinfo.Name == lib)
                    {
                        if (classinfo.Static == true)
                        {
                            enuminfo = null;
                            lastClass = lib;
                            classInfo = classinfo;
                            memberinfo.Clear();
                            structinfo = null;
                            IsHas = true;
                            break;
                        }
                        else
                        {
                            foreach (var member in classinfo.MemberList)
                            {
                                if (member.Static==true)
                                {
                                    memberinfo.Add(member);
                                }
                            }
                        }
                        
                    }
                }
                if (IsHas == true) { break; }
                foreach (var enuminfo in item.Value.listEnum)
                {
                    if (enuminfo.Name == lib)
                    {
                        this.enuminfo = enuminfo;
                        classInfo = null;
                        IsHas = true;
                        memberinfo.Clear();
                        structinfo = null;
                        break;
                    }
                }
                if (IsHas == true) { break; }
            }
            //如果静态类没找到,检查变量所属类别,然后检查声明类
            if (!IsHas)
            {
                Regex regVarType = new Regex("(\\w+)\\b[\\w,\\t ]*?\\b" + lib + "\\b[\\w,(;]*?", RegexOptions.Compiled);
                string temp = scintilla.GetTextRange(0, wordStartPos);
                Match match = regVarType.Match(temp);
                if (match.Success)
                {

                    string templib = match.Groups[1].Value;
                    foreach (var item in gh.GacComplete)
                    {

                        foreach (var classinfo in item.Value.listClass)
                        {
                            if (classinfo.Name == templib && classinfo.Static == false)
                            {
                                lastClass = lib;
                                classInfo = classinfo;
                                enuminfo = null;
                                memberinfo.Clear();
                                structinfo = null;
                                IsHas = true;
                                break;
                            }
                        }
                        foreach (var structinfo in item.Value.ListStruct)
                        {
                            if (structinfo.Name == templib )
                            {
                                lastClass = lib;
                                this.structinfo = structinfo;
                                classInfo = null;
                                enuminfo = null;
                                memberinfo.Clear();
                                IsHas = true;
                                break;
                            }
                        }
                        if (IsHas == true){break;}
                    }

                }
            }
            //最终什么都没找到的话清理旧得数据
            if (!IsHas)
            {
                lastClass = lib;
                classInfo = null;
                enuminfo = null;
                structinfo = null;
                memberinfo.Clear();
            }
        }


        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            IEnumerator < AutocompleteItem > r= BuildList().GetEnumerator(); ;
            return r;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            HashSet<string> hs = new HashSet<string>();
            HashSet<string> hs2 = new HashSet<string>();
            List<AutocompleteItem> list = new List<AutocompleteItem>();

            Console.WriteLine("开始读取智能提示");
            foreach (var item in gh.GacComplete)
            {
                //读取常量
                foreach (var define in item.Value.Defines)
                {
                    if (!hs.Contains(define.Name+":"+define.Value))
                    {
                        hs.Add(define.Name + ":" + define.Value);
                        list.Add(new AutocompleteItem(define));
                    }
                    if (!hs2.Contains(define.Name))
                    {
                        hs2.Add(define.Name);
                    }

                }
                //读取公开方法
                foreach (var method in item.Value.Methods)
                {
                    string methodstr = method.Name + ":" + method.GetParams;
                    if (!hs.Contains(methodstr))
                    {
                        hs.Add(methodstr);
                        list.Add(new AutocompleteItem(method));
                    }
                    if (!hs2.Contains(method.Name))
                    {
                        hs2.Add(method.Name);
                    }
                    //yield return new AutocompleteItem(method);
                }
                //读取静态类名
                foreach (var classinfo in item.Value.listClass)
                {
                    if (!hs.Contains("class:" + classinfo.Name))
                    {
                        hs.Add("class:" + classinfo.Name);
                        if (classinfo.Static)
                        {
                            list.Add(new AutocompleteItem(classinfo.Name, 0) { ToolTipTitle = classinfo.Summary});
                        }
                        else
                        {
                            list.Add(new AutocompleteItem(classinfo.Name, 4) { ToolTipTitle = classinfo.Summary });
                        }
                        //yield return new AutocompleteItem(classinfo.Name, 0);
                    }
                    if (!hs2.Contains(classinfo.Name))
                    {
                        hs2.Add(classinfo.Name);
                    }
                }
                //读取枚举Enum
                foreach (var enuminfo in item.Value.listEnum)
                {
                    if (!hs.Contains("enum:" + enuminfo.Name))
                    {
                        hs.Add("enum:" + enuminfo.Name);
                        list.Add(new AutocompleteItem(enuminfo.Name,6) { ToolTipTitle = "枚举名称：" + enuminfo.Name+"\r\n枚举数量：" +enuminfo.ListEnum.Count+"\r\n"+enuminfo.Summary});
                        //yield return new AutocompleteItem(enuminfo.Name, 0);
                        if (!hs2.Contains(enuminfo.Name))
                        {
                            hs2.Add(enuminfo.Name);
                        }
                        foreach (var enuminfo2 in enuminfo.ListEnum)
                        {
                            list.Add(new AutocompleteItem(enuminfo2, enuminfo.Name));
                            if (!hs2.Contains(enuminfo2.Name))
                            {
                                hs2.Add(enuminfo2.Name);
                            }
                            //yield return new AutocompleteItem(enuminfo2,enuminfo.Name);
                        }
                    }
                }
                //读取结构Struct
                foreach (var structinfo in item.Value.ListStruct)
                {
                    if (!hs.Contains("struct:" + structinfo.Name))
                    {
                        hs.Add("struct:" + structinfo.Name);
                        list.Add(new AutocompleteItem(structinfo.Name, 6) { ToolTipTitle = "结构名称：" + structinfo.Name + "\r\n选项数量：" + structinfo.ListMember.Count + "\r\n" + structinfo.Summary });
                        //yield return new AutocompleteItem(enuminfo.Name, 0);
                        if (!hs2.Contains(structinfo.Name))
                        {
                            hs2.Add(structinfo.Name);
                        }
                    }
                }

            }
            //如果存在静态类得方法
            if (classInfo != null)
            {
                foreach (var item in classInfo.MemberList)
                {
                    if (classInfo.Static||item.Static==false)
                    {
                        list.Add(new GacMethod(item));
                    }
                    
                    //yield return (AutocompleteItem)new GacMethod(item);
                }

            }
            //如果存在枚举
            if (enuminfo != null)
            {
                foreach (var item in enuminfo.ListEnum)
                {
                    list.Add(new GacEnum(item,enuminfo.Name));
                    //yield return (AutocompleteItem)new GacMethod(item);
                }

            }

            //如果存在结构
            if (structinfo != null)
            {
                foreach (var item in structinfo.ListMember)
                {
                    list.Add(new GacMethod(item, enuminfo.Name));
                    //yield return (AutocompleteItem)new GacMethod(item);
                }

            }

            //如果存在声明类有静态方法
            if (memberinfo.Count>0)
            {
                foreach (var item in memberinfo)
                {
                    list.Add(new GacMethod(item));
                }

            }

            //find all words of the text
            var words = new Dictionary<string, string>();
            foreach (Match m in Regex.Matches(scintilla.Text, @"\b\w+\b"))
                words[m.Value] = m.Value;

            //return autocomplete items
            foreach (var word in words.Keys)
            {
                if (!hs2.Contains(word))
                {
                    list.Add(new ArduinoAutocompleteItem(word) { ToolTipTitle = word + "\r\n来自用户输入记录", ImageIndex = 7 });
                }
            }
            foreach (var word in snippets)
                list.Add(new SnippetAutocompleteItem(word) {ToolTipTitle="来自常用语法\r\n"+word, ImageIndex=1 });
            foreach (var item in dicDataTypes)
                list.Add(new AutocompleteItem(item.Key) { ToolTipTitle = "数据类型："+ item .Key+ "\r\n" + item.Value, ImageIndex = 3 });

            return list.OrderBy(item => item.Text).ToList<AutocompleteItem>();




        }

    }

    /// <summary>
    /// This autocomplete item appears after dot
    /// </summary>
    public class GacMethod : AutocompleteItem
    {
        string firstPart;
        string lowercaseText;
        public GacMethod(Member member)
             : base(member)
        {
            lowercaseText = member.Name.ToLower();
            
        }
        public GacMethod(EnumList Eunm,string EnumName)
            : base(Eunm, EnumName)
        {
            lowercaseText = Eunm.Name.ToLower();

        }
        public GacMethod(Param paraminfo, string Parent)
    : base(paraminfo, Parent)
        {
            lowercaseText = paraminfo.Name.ToLower();

        }

        public override CompareResult Compare(string fragmentText)
        {
            int i = fragmentText.LastIndexOf('.');
            if (i < 0)
                return CompareResult.Hidden;
            string lastPart = fragmentText.Substring(i + 1);
            firstPart = fragmentText.Substring(0, i);

            if (lastPart == "") return CompareResult.Visible;
            if (Text.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase))
                return CompareResult.VisibleAndSelected;
            if (lowercaseText.Contains(lastPart.ToLower()))
                return CompareResult.Visible;

            return CompareResult.Hidden;
        }

        public override string GetTextForReplace()
        {
            return firstPart + "." + Text;
        }
    }
    /// <summary>
    /// This autocomplete item appears after dot
    /// </summary>
    public class GacEnum : AutocompleteItem
    {
        string firstPart;
        string lowercaseText;
        public GacEnum(EnumList Eunm, string EnumName)
            : base(Eunm, EnumName)
        {
            lowercaseText = Eunm.Name.ToLower();

        }


        public override CompareResult Compare(string fragmentText)
        {
            int i = fragmentText.LastIndexOf('.');
            if (i < 0)
                return CompareResult.Hidden;
            string lastPart = fragmentText.Substring(i + 1);
            firstPart = fragmentText.Substring(0, i);

            if (lastPart == "") return CompareResult.Visible;
            if (Text.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase))
                return CompareResult.VisibleAndSelected;
            if (lowercaseText.Contains(lastPart.ToLower()))
                return CompareResult.Visible;

            return CompareResult.Hidden;
        }

        public override string GetTextForReplace()
        {
            return  Text;
        }
    }
}
