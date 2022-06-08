using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GacArduinoHelper
{
    
    public class GacHelper
    {
        public Dictionary<string, GacAutoComplete> GacComplete = new Dictionary<string, GacAutoComplete>();
        public GacAutoComplete SystemComplete = new GacAutoComplete();
        public static int MaxLevel = 3;//最大文件层次,层次越多,提示越多,处理越慢
        
        public GacHelper()
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "\\help\\ArduinoSystem", "*.xml", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            List<string> listfiles = files.ToList<string>();
            Dictionary<string, List<GacHelp>> dic = GetHelps(listfiles, false,0);
            Dictionary<string, GacAutoComplete> gac = GetGacAutoComplete(dic);
            foreach (var item in gac.Keys)
            {
                GacComplete.Add(item, gac[item]);
            }
        }
        public static void InitSystem(string ArduinoIde)
        {
            string arduinopath = Path.GetDirectoryName(ArduinoIde)+ @"\hardware\arduino\avr\cores\arduino";
            if (!Directory.Exists(arduinopath))
            {
                return;
            }
            string[] files = Directory.GetFiles(arduinopath, "*.h", SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
                SearchH(item, "ArduinoSystem",0);
            }

        }
        Regex regArduinoinclude = new Regex("#include\\s*<(\\w+)\\.h>");
        public List<string> GetInclude(string code)
        {
            List<string> list = new List<string>();
            MatchCollection mcs= regArduinoinclude.Matches(code);
            for (int i = 0; i < mcs.Count; i++)
            {
                list.Add( mcs[i].Groups[1].Value);
            }
            return list;
        }
        public void GetHelp(string code)
        {
            RemoveRemarks(ref code);
            List<string> include = GetInclude(code);
            Dictionary<string, List<GacHelp>> dic= GetHelps(include,true,0);
           Dictionary<string,GacAutoComplete> gac= GetGacAutoComplete(dic);

            foreach (var item in include)
            {
                if (!GacComplete.ContainsKey(item))
                {
                    
                    GacComplete.Add(item, gac[item]);
                    //for (int i = 0; i < gac[item].listClass.Count; i++)
                    //{
                    //    var classinfo = gac[item].listClass[i];
                    //    HashSet<string> hs = new HashSet<string>();
                    //    foreach (var member in classinfo.MemberList)
                    //    {
                    //        string pars = member.Name + member.GetParams;
                    //        if (!hs.Contains(pars))
                    //        {
                    //            if (member.Name== "beginSmartConfig")
                    //            {

                    //            }
                    //            hs.Add(pars);
                    //        }
                            
                    //    }
                    //    foreach (var classinfo2 in gac[item].listClass)
                    //    {
                    //        if (classinfo != null && classinfo.Inheritance != null)
                    //        {
                    //            if (classinfo.Inheritance.Contains(classinfo2.Name))
                    //            {
                    //                foreach (var member in classinfo2.MemberList)
                    //                {
                                       
                    //                    string pars = member.Name + member.GetParams;
                    //                    if (!hs.Contains(pars))
                    //                    {
                    //                        if (member.Name == "beginSmartConfig")
                    //                        {

                    //                        }
                    //                        hs.Add(pars);
                    //                        GacComplete[item].listClass[i].MemberList.Add(member);
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }
                   //}
                }
            }
            int dicCount = dic.Keys.Count;
            String[] strKey = new String[dicCount];
            dic.Keys.CopyTo(strKey, 0);//支持.net2.0
            foreach (var item in strKey)
            {
                if (item!="System"&&!((IList)include).Contains(item))
                {
                    GacComplete.Remove(item);
                }
            }
        }
        static Regex regLineRemark = new Regex("\\s*//(.*?)$", RegexOptions.Multiline);
        static Regex regMultiLineRemark = new Regex("\\s*/\\*[\\s\\S]*?\\*/");
        static  void RemoveRemarks(ref string Code)
        {
            Code=regLineRemark.Replace(Code, "");
            Code = regMultiLineRemark.Replace(Code, "");
        }
        string userdata = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"/Arduino15/";
        public Dictionary<string, List<GacHelp>> GetHelps(List<string> includes,bool NoSystem,int Level)
        {

            Dictionary<string, List<GacHelp>> dic = new Dictionary<string, List<GacHelp>>();
            if (includes==null|| includes.Count==0|| Level> MaxLevel)
            {
                return dic;
            }
          
            foreach (var include in includes)
            {
                string includefile = include;
                if (GacComplete.ContainsKey(includefile) && NoSystem)
                {
                    continue;
                }

                string[] files = Directory.GetFiles(Application.StartupPath + "\\help\\"+( NoSystem ?"":"ArduinoSystem"), includefile + ".xml", NoSystem?SearchOption.AllDirectories: SearchOption.TopDirectoryOnly);
                if (files.Length==0)//没有找到任何帮助,可能输入错误,也可能不是在类库里
                {
                    files = Directory.GetFiles(userdata, includefile + ".h", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        string parent = "";
                        string[] dirs= item.Split(new string[] { @"/packages\" }, StringSplitOptions.RemoveEmptyEntries);
                        if (dirs.Length > 1)
                        {
                            dirs = dirs[1].Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
                            parent = dirs[0];
                        }
                        else
                        {
                            parent = "gacUserHistory";
                        }
                        SearchH(item, parent, 0);
                    }
                    files = Directory.GetFiles(Application.StartupPath + "\\help\\", includefile + ".xml", NoSystem ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                }
                if (!NoSystem)
                {
                    includefile = "System";
                }
                if (!dic.ContainsKey(includefile) )
                {
                    dic.Add(includefile, new List<GacHelp>());
                }
                foreach (var item in files)
                {
                    GacHelp gh = SearchH(item, "", Level, item);

                    //dic[includefile].Add(gh);
                    CheckAdd( dic[includefile], gh);
                    Dictionary<string, List<GacHelp>> dictemp = new Dictionary<string, List<GacHelp>>();
                    //if (gh.inlude!=null&& gh.inlude.Length>0)
                    //{
                    //    Console.WriteLine("读取" + item + "的关联帮助文件");
                    //}

                    dictemp = GetHelps(gh.inlude,NoSystem,Level+1);
                    foreach (string key in dictemp.Keys)
                    {
                        foreach (GacHelp g in dictemp[key])
                        {

                            dic[includefile].Add(g);
                        }
                    }
                }
            }
            return dic;
        }
        void CheckAdd(List<GacHelp> list,GacHelp ch)
        {
            foreach (var item in list)
            {
                if (item.FileName == ch.FileName)
                {
                    return;
                }
            }
            list.Add(ch);
        }

        static Regex reginclude = new Regex("#include [\"<](.*?/)?(\\w+)\\.h[\">]");
        //static Regex regClass = new Regex("^\\s*class (.*?)\\s*(:(.*?))*?\\s*{([\\s\\S]*?)}\\s*;", RegexOptions.Multiline);

        //类名 不要 继承 根据匹配的字符,查找字符串然后找结尾位置
        static Regex regClass = new Regex("\\s*class (.*?)\\s*(:(.*?))*?\\s*{", RegexOptions.Multiline);
        static Regex regClassInheritance = new Regex("public (\\w+)");
        static Regex regexClassPublic = new Regex("public:([\\s\\S]*?)(\\s+\\w+:\\s+|\\s$)");//|\\};

        //static 返回值 方法名 参数和其他信息,参数需要额外取
        static Regex regMethods = new Regex("^\\s*(static)?\\s*([\\w \\*\\&]+)?\\s+(\\w+)\\s*(.+)$", RegexOptions.Multiline);

        //属性类型 属性名
        static Regex regAttribute = new Regex("^\\s*([\\w\\*\\s]+)?\\s+([\\w\\*]+);",RegexOptions.Multiline);


        static Regex regdefine = new Regex("#define (\\w+)[^\\S\\r\\n]+([-A-Fa-f0-9x]+)");

        static Regex regtypedef = new Regex("typedef (\\w+) (\\w+);");

        //扩展
        //extern ([\\w\\* ]+) ([\\w\\*]*?);
        static Regex regExtern = new Regex("extern ((\\w+::)?[\\w\\* :]*?) ([\\w\\*]*?)[ ;]");

        //Using
        //using\\s*((.*?)\\s*=)?.*?::(\\w+)
        static Regex regUsing = new Regex("using\\s*(.*?)\\s*=.*?::(\\w+)");

        /// <summary>
        /// 枚举大块 如果3不为空 那么3是名字,否则1是名字 2是内容部分,需要再次匹配
        /// </summary>
        static Regex regEnum = new Regex("enum (\\w+)?\\s*\\{([^}]*?)\\}\\s*?(\\w+)?;");
        /// <summary>
        /// 枚举内容获取 1是枚举的名字 3是枚举的值
        /// </summary>
        static Regex regEnumInfo = new Regex("\\s*(\\w+)\\s*(=\\s*(\\d+))?\\s*,?");

        static Regex regRemoveDKH = new Regex("\\{[\\s\\S]*?\\}");

        //struct
        static Regex regStruct = new Regex("struct\\s*(\\w*)\\s*\\{([\\s\\S]*?)\\}\\s*(\\w*);");
        static Regex regStructInfo = new Regex("\\s*([\\w \\*\\&]+)?\\s+([\\w\\[\\]]+)\\s*(.+)$", RegexOptions.Multiline);
        static Dictionary<string, GacHelp> dicHelpCache = new Dictionary<string, GacHelp>();
        public static GacHelp SearchH(string filename, string parent,int Level,string helpfile="")
        {
            if (Level> MaxLevel)//分析到第3层
            {
                return new GacHelp();
            }
            string thisfile = Path.GetFileNameWithoutExtension(filename);
            string dirhelp = "";
            if (string.IsNullOrEmpty(helpfile))
            {

                helpfile = Application.StartupPath + "\\help\\" + parent + "\\" + thisfile + ".xml";
                dirhelp = Path.GetDirectoryName(helpfile); ;
                if (!Directory.Exists(dirhelp))
                {
                    Directory.CreateDirectory(dirhelp);
                }
            }
            if (dicHelpCache.ContainsKey(helpfile))
            {
                return dicHelpCache[helpfile];
            }
            Console.WriteLine("读取帮助文件:" + helpfile);
            Application.DoEvents();
            if (File.Exists(helpfile))
            {
                try
                {
                    GacHelp gh = (GacHelp)XmlSerializer.LoadFromXml(helpfile, typeof(GacHelp));
                    gh.FileName = helpfile;
                    if (!dicHelpCache.ContainsKey(helpfile))
                    {
                        dicHelpCache.Add(helpfile, gh);
                    }
                    return gh;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("错误的帮助文件："+helpfile+"\r\n" + ex.Message);
                    Console.WriteLine("错误的帮助文件"+ex.Message);
                    return new GacHelp();
                }
               

            }
            Console.WriteLine("分析新的帮助文件:" + filename);
            //File.WriteAllText(helpfile, "帮助文件正在生成");
            GacHelp ch = new GacHelp();
            ch.FileName = helpfile.Replace(Application.StartupPath,"");//仅保留help开头的帮助文件
            

            string code = File.ReadAllText(filename);
            RemoveRemarks(ref code);
            //include
            MatchCollection mcsInclude = reginclude.Matches(code);
            if (mcsInclude.Count > 0)
            {
                
                for (int i = 0; i < mcsInclude.Count; i++)
                {
                    if (mcsInclude[i].Groups[2].Value!= thisfile)
                    {
                        ch.inlude.Add(mcsInclude[i].Groups[2].Value);
                        if (!File.Exists(dirhelp + "\\" + mcsInclude[i].Groups[2].Value + ".xml"))
                        {
                            string dir = Path.GetDirectoryName(filename);
                            string[] files = Directory.GetFiles(dir, mcsInclude[i].Groups[2].Value + ".h", SearchOption.AllDirectories);
                            foreach (var item in files)
                            {
                                SearchH(item, parent, Level+1);
                            }
                        }
                    }
                }
            }

            //extern
            MatchCollection mcsExtern = regExtern.Matches(code);
            if (mcsExtern.Count > 0)
            {

                for (int i = 0; i < mcsExtern.Count; i++)
                {
                    ExternInfo ei = new ExternInfo() { Name = mcsExtern[i].Groups[3].Value, Class = mcsExtern[i].Groups[1].Value, Static = true };
                    if (!string.IsNullOrEmpty(mcsExtern[i].Groups[2].Value))
                    {
                        //ei.Static = true;
                        ei.Class = ei.Class.Replace(mcsExtern[i].Groups[2].Value, "");
                    }
                    ch.Extern.Add(ei);
                }
            }


            //using
            MatchCollection mcsUsing = regUsing.Matches(code);
            if (mcsUsing.Count > 0)
            {

                for (int i = 0; i < mcsUsing.Count; i++)
                {
                    ExternInfo ei = new ExternInfo() { Name = mcsUsing[i].Groups[1].Value, Class = mcsUsing[i].Groups[2].Value };
                    if (ei.Name!=ei.Class)
                    {
                        ch.Extern.Add(ei);
                    }
                }
            }


            //enum
            MatchCollection mcsEnum = regEnum.Matches(code);
            if (mcsEnum.Count > 0)
            {
                for (int i = 0; i < mcsEnum.Count; i++)
                {
                    string enumname = mcsEnum[i].Groups[1].Value;
                    if (!string.IsNullOrEmpty(mcsEnum[i].Groups[3].Value))
                    {
                        enumname = mcsEnum[i].Groups[3].Value;
                    }
                    ch.Enum.Add(new EnumInfo() {  Name=enumname});
                    MatchCollection mcsEnumInfo= regEnumInfo.Matches(mcsEnum[i].Groups[2].Value);
                    foreach (Match matchEnum in mcsEnumInfo)
                    {
                        ch.Enum[ch.Enum.Count - 1].ListEnum.Add(new EnumList() {  Name=matchEnum.Groups[1].Value, Value=matchEnum.Groups[3].Value});
                    }
                    
                }
            }
            //Define
            MatchCollection mcsDefine = regdefine.Matches(code);
            if (mcsDefine.Count>0)
            {
                for (int i = 0; i < mcsDefine.Count; i++)
                {
                    ch.define.Add(new DefineInfo() { Value = mcsDefine[i].Groups[2].Value, Name = mcsDefine[i].Groups[1].Value });
                }
            }

            //class
            MatchCollection mcsClass = regClass.Matches(code);
            if (mcsClass.Count > 0)
            {
                ch.Class = new List<ClassInfo>();
                for (int i = 0; i < mcsClass.Count; i++)
                {
                    MatchCollection mcsClassInheritance = regClassInheritance.Matches(mcsClass[i].Groups[3].Value);
                    string[] classInheritance = new string[mcsClassInheritance.Count];
                    for (int i2 = 0; i2 < mcsClassInheritance.Count; i2++)
                    {
                        classInheritance[i2] = mcsClassInheritance[i2].Groups[1].Value;
                    }
                    ClassInfo ci = new ClassInfo() { Name = mcsClass[i].Groups[1].Value, Inheritance = classInheritance };
                    string classcode = GetClass(code, mcsClass[i].Value);
                    if (!string.IsNullOrEmpty(classcode))
                    {
                        string publicode = regRemoveDKH.Replace(classcode, "");
                        MatchCollection mcsPublic = regexClassPublic.Matches(publicode);//mcsClass[i].Groups[4].Value
                        for (int i2 = 0; i2 < mcsPublic.Count; i2++)
                        {

                            MatchCollection mcsMethods = regMethods.Matches(mcsPublic[i2].Groups[1].Value);
                            foreach (Match match in mcsMethods)
                            {
                                Member mb = new Member();
                                mb.Static = !string.IsNullOrEmpty(match.Groups[1].Value);
                                mb.returns.DataType = match.Groups[2].Value;
                                mb.Name = match.Groups[3].Value;
                                mb.Param = GetParams(match.Groups[4].Value);
                                if (mb.returns.DataType.StartsWith("virtual ")|| mb.returns.DataType=="using")
                                {
                                    continue;
                                }
                                if (mb.Name == ci.Name)
                                {
                                    ci.Static = false;
                                    ch.MemberList.Add(mb);
                                }
                                else
                                {
                                    ci.MemberList.Add(mb);
                                }
                            }

                            MatchCollection mcsattr = regAttribute.Matches(mcsPublic[i2].Groups[1].Value);
                            foreach (Match match in mcsattr)
                            {

                                Member mb = new Member();
                                mb.returns.DataType = match.Groups[1].Value;
                                mb.Name = match.Groups[2].Value;
                                mb.isAttr = true;
                                ci.MemberList.Add(mb);
                            }

                        }
                    }
                   
                   
                    ch.Class.Add(ci);
                }
            }

            //Struct
            MatchCollection mcsStruct = regStruct.Matches(code);
            if (mcsStruct.Count > 0)
            {
                for (int i = 0; i < mcsStruct.Count; i++)
                {
                    MatchCollection mcsStructInfo = regStructInfo.Matches(mcsStruct[i].Groups[2].Value);
                    if (mcsStructInfo.Count>0)
                    {
                        Struct s = new Struct();
                        if (!string.IsNullOrEmpty(mcsStruct[i].Groups[3].Value))
                        {
                            s.Name = mcsStruct[i].Groups[3].Value;
                        }
                        else
                        {
                            s.Name = mcsStruct[i].Groups[1].Value;
                        }

                        for (int i2 = 0; i2 < mcsStructInfo.Count; i2++)
                        {
                            Param p = new Param();
                            p.DataType = mcsStructInfo[i2].Groups[1].Value;
                            p.Name = mcsStructInfo[i2].Groups[2].Value;
                            s.ListMember.Add(p);
                        }
                        ch.Struct.Add(s);
                    }
                    
                }
            }
            //typedef
            MatchCollection mcstypedef = regtypedef.Matches(code);
            if (mcstypedef.Count > 0)
            {
                for (int i = 0; i < mcstypedef.Count; i++)
                {
                    ch.Extern.Add(new ExternInfo() {  Class = mcstypedef[i].Groups[1].Value, Name = mcstypedef[i].Groups[2].Value });
                }
            }

            XmlSerializer.SaveToXml(helpfile, ch, typeof(GacHelp), "GacHelp");
           
            return ch;
        }
        static List<Param> GetParams(string param)
        {
            List<Param> list = new List<Param>();
            param = GetInBrackets(param);
            if (param.Trim().Length<=0)
            {
                return list;
            }

            string[] strparams = param.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in strparams)
            {
                Param p = new Param();

                string[] info = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)//有默认值
                {
                    p.Default = info[1].Trim();
                }
                info = info[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                p.Name = info[info.Length - 1];
                for (int i = 0; i < info.Length - 1; i++)
                {
                    if (p.DataType.Length > 0)
                    {
                        p.DataType += " ";
                    }
                    p.DataType += info[i];
                }
                list.Add(p);
            }
            return list;

        }
        static string GetInBrackets(string param)
        {
            int i = 0;
            int index = -1;
            int start = 0;
            while ( (index=param.IndexOfAny(new char[] { '(', ')' },index+1))>=0)
            {
                if (param[index] == '(')
                {
                    if (i==0)
                    {
                        start = index+1;
                    }
                    i ++;
                }
                else
                {
                    i--;
                    if (i==0)
                    {
                        return param.Substring(start, index-start);
                    }
                }
            }
            //没有括号的应该是错误了,返回空,不给参数
            //return param;
            return "";
        }
        static string GetClass(string code,string beginstr)
        {
            int index = code.IndexOf(beginstr);
            if (index>=0)
            {
                int i = 1;
                index += beginstr.Length-1;
                int start = index+1;
                while ((index = code.IndexOfAny(new char[] { '{', '}' }, index + 1)) >= 0)
                {
                    if (code[index] == '{')
                    {
                        i++;
                    }
                    else
                    {
                        i--;
                        if (i == 0)
                        {
                            return code.Substring(start, index - start);
                        }
                    }
                }
            }
            return "";
        }

        Dictionary<string, GacAutoComplete> GetGacAutoComplete(Dictionary<string, List<GacHelp>> dic)
        {
            
            Dictionary<string, GacAutoComplete> dicResult = new Dictionary<string, GacAutoComplete>();
            foreach (var item in dic)
            {
                if(!dicResult.ContainsKey(item.Key))
                {
                    dicResult.Add(item.Key, new GacAutoComplete());
                }
                foreach (var item2 in item.Value)
                {
                    foreach (var item3 in item2.Class)
                    {
                        if (item3.Name=="SH1106"||item3.Name== "SH1106Wire")
                        {

                        }
                        if (!dicResult[item.Key].DicMethod.ContainsKey(item3.Name))
                        {
                            dicResult[item.Key].DicMethod.Add(item3.Name, new List<Member>());
                        }
                        //Methods添加提示
                        dicResult[item.Key].DicMethod[item3.Name].AddRange(item3.MemberList);

                        //继承类添加提示
                        if (item3.Inheritance!=null)
                        {
                            GetInfo gi = GetInheritance(dic, item3.Inheritance);

                            HashSet<string> hs2 = new HashSet<string>();
                            foreach (var itemmember in item3.MemberList)
                            {
                                if (!hs2.Contains(itemmember.Name + itemmember.GetParams))
                                {
                                    hs2.Add(itemmember.Name + itemmember.GetParams);
                                }
                            }
                            foreach (var itemmember in gi.ListMember)
                            {
                                if (!hs2.Contains(itemmember.Name + itemmember.GetParams))
                                {
                                    hs2.Add(itemmember.Name + itemmember.GetParams);
                                    item3.MemberList.Add(itemmember);
                                }
                            }
                            //item3.MemberList.AddRange(gi.ListMember);

                            dicResult[item.Key].DicMethod[item3.Name].AddRange(gi.ListMember);
                            dicResult[item.Key].Defines.AddRange(gi.ListDefine);
                            dicResult[item.Key].listEnum.AddRange(gi.listEnum);
                            dicResult[item.Key].ListStruct.AddRange(gi.ListStruct);
                        }
                       



                        //类名添加提示 改调用词典
                        //if (!dicResult[item.Key].List.Contains(item3.Name))
                        //{
                        //    dicResult[item.Key].List.Add(item3.Name);
                        //}



                    }
                    foreach (var item3 in item2.Extern)
                    {
                        //if (!dicResult[item.Key].DicMethod.ContainsKey(item3.Name))
                        //{
                        //    if (dicResult[item.Key].DicMethod.ContainsKey(item3.Class))
                        //    {
                        //        dicResult[item.Key].DicMethod.Add(item3.Name, new List<Member>());
                        //        dicResult[item.Key].DicMethod[item3.Name].AddRange(dicResult[item.Key].DicMethod[item3.Class]);
                        //        //dicResult[item.Key].List.Add(item3.Name);
                        //    }
                        //}
                        //dicResult[item.Key].DicExtern.Add(item3.Name, item3.Class);
                        foreach (var itemAll in dic)
                        {
                            foreach (var itemdic in itemAll.Value)
                            {
                                for (int i = 0; i < itemdic.Class.Count; i++)
                                {
                                   
                                    if (itemdic.Class[i].Name == item3.Class)
                                    {
                                        //暴漏的类添加到类中
                                        dicResult[item.Key].listClass.Add(new ClassInfo() { Name = item3.Name, MemberList = itemdic.Class[i].MemberList, Static = item3.Static ? true : itemdic.Class[i].Static, Summary = itemdic.Class[i].Summary });
                                    }
                                }
                            }
                            
                        }
                           
                    }

                    //常量获取
                    dicResult[item.Key].Defines.AddRange(item2.define);

                    //结构
                    dicResult[item.Key].ListStruct.AddRange(item2.Struct);

                    //公开函数,主要用于系统类
                    dicResult[item.Key].Methods.AddRange(item2.MemberList);

                    //所有类
                    dicResult[item.Key].listClass.AddRange(item2.Class);
                }
            }
            Dictionary<string, GacAutoComplete> dicNewResult = new Dictionary<string, GacAutoComplete>();
            foreach (var item in dicResult)
            {
                dicNewResult.Add(item.Key, new GacAutoComplete());

                HashSet<string> hs = new HashSet<string>();
                foreach (var item2 in item.Value.Defines)
                {
                    if (!hs.Contains(item2.Name+item2.Value))
                    {
                        hs.Add(item2.Name + item2.Value);
                        dicNewResult[item.Key].Defines.Add(item2);
                    }
                }

                hs = new HashSet<string>();
                foreach (var item2 in item.Value.listEnum)
                {
                    if (!hs.Contains(item2.Name + item2.ListEnum.Count))
                    {
                        hs.Add(item2.Name + item2.ListEnum.Count);
                        dicNewResult[item.Key].listEnum.Add(item2);
                    }
                }

                hs = new HashSet<string>();

                foreach (var item2 in item.Value.Methods)
                {
                    if (!hs.Contains(item2.Name + item2.GetParams))
                    {
                        hs.Add(item2.Name + item2.GetParams);
                        dicNewResult[item.Key].Methods.Add(item2);
                    }
                }


                hs = new HashSet<string>();
                if (item.Key == "SH1106" || item.Key == "SH1106Wire")
                {

                }
                foreach (var item2 in item.Value.listClass)
                {
                    if (!hs.Contains(item2.Name+item2.MemberList.Count))
                    {
                        hs.Add(item2.Name + item2.MemberList.Count);
                        dicNewResult[item.Key].listClass.Add(item2);
                    }
                }


                hs = new HashSet<string>();
                foreach (var item2 in item.Value.ListStruct)
                {
                    if (!hs.Contains(item2.Name + item2.ListMember.Count))
                    {
                        hs.Add(item2.Name + item2.ListMember.Count);
                        dicNewResult[item.Key].ListStruct.Add(item2);
                    }
                }
            }
            return dicNewResult;
        }
        GetInfo GetInheritance(Dictionary<string, List<GacHelp>> dic,string[] inheritance)
        {
            GetInfo gi = new GetInfo();
            List<Member> list = new List<Member>();
            if (inheritance.Length==0)
            {
                return gi;
            }
            HashSet<string> hs = new HashSet<string>();
            //原本是从当前信息继承,后改成全部
            foreach (var item in GacComplete.Keys)
            {

                foreach (var item3 in GacComplete[item].listClass)
                {

                    if (((IList)inheritance).Contains(item3.Name) && !hs.Contains(item3.Name))//说明此类是被继承的
                    {
                        hs.Add(item3.Name);
                        gi.ListMember.AddRange(item3.MemberList);
                        if (item3.Inheritance != null)
                        {
                            GetInfo gitemp = GetInheritance(dic, item3.Inheritance);
                            HashSet<string> hs2 = new HashSet<string>();
                            foreach (var itemmember in gi.ListMember)
                            {
                                if (!hs2.Contains(itemmember.Name + itemmember.GetParams))
                                {
                                    hs2.Add(itemmember.Name + itemmember.GetParams);
                                }
                            }
                            foreach (var itemmember in gitemp.ListMember)
                            {
                                if (!hs2.Contains(itemmember.Name+itemmember.GetParams))
                                {
                                    hs2.Add(itemmember.Name + itemmember.GetParams);
                                    gi.ListMember.Add(itemmember);
                                }
                            }
                            //gi.ListMember.AddRange(gitemp.ListMember);
                            gi.ListDefine.AddRange(gitemp.ListDefine);
                        }

                        //gi.ListAttribute.AddRange(gitemp.ListAttribute);
                    }


                }
                gi.listEnum.AddRange(GacComplete[item].listEnum);
                gi.ListDefine.AddRange(GacComplete[item].Defines);
                gi.ListStruct.AddRange(GacComplete[item].ListStruct);
                


            }

            foreach (var item in dic.Keys)
            {
                foreach (var item2 in dic[item])
                {
                    foreach (var item3 in item2.Class)
                    {

                        if (((IList)inheritance).Contains(item3.Name)&&! hs.Contains(item3.Name))//说明此类是被继承的
                        {
                            hs.Add(item3.Name);
                            gi.ListMember.AddRange(item3.MemberList);
                            if (item3.Inheritance != null)
                            {
                                GetInfo gitemp = GetInheritance(dic, item3.Inheritance);
                                HashSet<string> hs2 = new HashSet<string>();
                                foreach (var itemmember in gi.ListMember)
                                {
                                    if (!hs2.Contains(itemmember.Name + itemmember.GetParams))
                                    {
                                        hs2.Add(itemmember.Name + itemmember.GetParams);
                                    }
                                }
                                foreach (var itemmember in gitemp.ListMember)
                                {
                                    if (!hs2.Contains(itemmember.Name + itemmember.GetParams))
                                    {
                                        hs2.Add(itemmember.Name + itemmember.GetParams);
                                        gi.ListMember.Add(itemmember);
                                    }
                                }
                                //gi.ListMember.AddRange(gitemp.ListMember);
                                gi.ListDefine.AddRange(gitemp.ListDefine);
                            }
                        }
                        
                        
                    }
                    gi.listEnum.AddRange(item2.Enum);
                    gi.ListDefine.AddRange( item2.define);
                    gi.ListStruct.AddRange(item2.Struct);
                }
            }


            return gi;
        }

    }
    public class GacAutoComplete
    {
        /// <summary>
        /// 暴露的类名
        /// </summary>
        Dictionary<string, List<Member>> _dicMethod = new Dictionary<string, List<Member>>();
        public Dictionary<string, List<Member>> DicMethod
        {
            get
            {
                return _dicMethod;
            }

            set
            {
                _dicMethod = value;
            }
        }

        Dictionary<string, string> _DicExtern = new Dictionary<string, string>();
        public Dictionary<string, string> DicExtern
        {
            get
            {
                return _DicExtern;
            }

            set
            {
                _DicExtern = value;
            }
        }
        List<DefineInfo> _listDefine = new List<DefineInfo>();
        public List<DefineInfo> Defines
        {
            get
            {
                return _listDefine;
            }

            set
            {
                _listDefine = value;
            }
        }
        List<Member> _listMethods = new List<Member>();
        public List<Member> Methods
        {
            get
            {
                return _listMethods;
            }

            set
            {
                _listMethods = value;
            }
        }

       List<ClassInfo> _listClass = new List<ClassInfo>();

        public  List< ClassInfo> listClass
        {
            get
            {
                return _listClass;
            }

            set
            {
                _listClass = value;
            }
        }
        List<EnumInfo> _listEnum = new List<EnumInfo>();

        public List<EnumInfo> listEnum
        {
            get
            {
                return _listEnum;
            }

            set
            {
                _listEnum = value;
            }
        }

        List<Struct> _listStruct = new List<Struct>();
        public List<Struct> ListStruct
        {
            get
            {
                return _listStruct;
            }

            set
            {
                _listStruct = value;
            }
        }

        
    }
    public class GetInfo
    {
        List<Member> _listMember = new List<Member>();
        List<DefineInfo> _listDefine = new List<DefineInfo>();
        List<Struct> _listStruct = new List<Struct>();

        public List<Member> ListMember
        {
            get
            {
                return _listMember;
            }

            set
            {
                _listMember = value;
            }
        }


        public List<DefineInfo> ListDefine
        {
            get
            {
                return _listDefine;
            }

            set
            {
                _listDefine = value;
            }
        }
        List<EnumInfo> _listEnum = new List<EnumInfo>();

        public List<EnumInfo> listEnum
        {
            get
            {
                return _listEnum;
            }

            set
            {
                _listEnum = value;
            }
        }

        public List<Struct> ListStruct
        {
            get
            {
                return _listStruct;
            }

            set
            {
                _listStruct = value;
            }
        }
    }
}
