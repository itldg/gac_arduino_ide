using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ArduinoHelp
{
    
    public class GacHelper
    {
        public Dictionary<string, GacAutoComplete> GacComplete = new Dictionary<string, GacAutoComplete>();
        public GacAutoComplete SystemComplete = new GacAutoComplete();
        public GacHelper()
        {
            string[] files = Directory.GetFiles("help", "*.xml", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            Dictionary<string, List<GacHelp>> dic = GetHelps(files,false);
            Dictionary<string, GacAutoComplete> gac = GetGacAutoComplete(dic);
            foreach (var item in gac.Keys)
            {
                GacComplete.Add(item, gac[item]);
            }
        }

        Regex regArduinoinclude = new Regex("#include\\s*<(\\w+)\\.h>");
        public string[] GetInclude(string code)
        {
            string[] result = new string[] { };
            MatchCollection mcs= regArduinoinclude.Matches(code);
            result = new string[mcs.Count];
            for (int i = 0; i < mcs.Count; i++)
            {
                result[i] = mcs[i].Groups[1].Value;
            }
            return result;
        }
        public void GetHelp(string code)
        {
            string[] include = GetInclude(code);
            Dictionary<string, List<GacHelp>> dic= GetHelps(include);
            Dictionary<string,GacAutoComplete> gac= GetGacAutoComplete(dic);
            foreach (var item in include)
            {
                if (!GacComplete.ContainsKey(item))
                {
                    GacComplete.Add(item, gac[item]);
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
        public Dictionary<string, List<GacHelp>> GetHelps(string[] includes,bool NoSystem=true)
        {
            Dictionary<string, List<GacHelp>> dic = new Dictionary<string, List<GacHelp>>();
            if (includes==null)
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
                string[] files = Directory.GetFiles("help\\", includefile + ".xml", NoSystem?SearchOption.AllDirectories: SearchOption.TopDirectoryOnly);
                if (!NoSystem)
                {
                    includefile = "System";
                    //if (!dic.ContainsKey(includefile))
                    //{
                    //    dic.Add(includefile,new List<GacHelp>());
                    //}
                }
                if (!dic.ContainsKey(includefile) )
                {
                    dic.Add(includefile, new List<GacHelp>());
                }
                foreach (var item in files)
                {
                    GacHelp gh = SearchH(item, "", item);
                    dic[includefile].Add(gh);
                    Dictionary<string, List<GacHelp>> dictemp = new Dictionary<string, List<GacHelp>>();
                    dictemp = GetHelps(gh.inlude);
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

        static Regex reginclude = new Regex("#include \"(\\w+)\\.h\"");
        static Regex regClass = new Regex("^\\s*class (.*?)\\s*(:(.*?))*?\\s*{([\\s\\S]*?)}\\s*;", RegexOptions.Multiline);
        static Regex regClassInheritance = new Regex("public (\\w+)[\\s,]");
        static Regex regexClassPublic = new Regex("public:([\\s\\S]*?)(\\s+\\w+:\\s+|\\};|\\s$)");

        //返回值 方法名 参数 有的会带 const;结尾
        static Regex regMethods = new Regex("\\s*([\\w\\*]+)?\\s+(\\w+)\\((.*)\\)(.*?);?");

        //属性类型 属性名
        static Regex regAttribute = new Regex("^\\s*([\\w\\*\\s]+)?\\s+([\\w\\*]+);",RegexOptions.Multiline);


        static Regex regdefine = new Regex("#define (\\w+)\\s+([-A-Fa-f0-9x]+)");

        //扩展
        static Regex regExtern = new Regex("extern (.*?) (.*?);");
        public static GacHelp SearchH(string filename, string parent,string helpfile="")
        {
            if (string.IsNullOrEmpty(helpfile))
            {

                helpfile = "help\\" + parent + "\\" + Path.GetFileNameWithoutExtension(filename) + ".xml";
                string dir = Path.GetDirectoryName(helpfile); ;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
          
            if (File.Exists(helpfile))
            {
                return null;
                GacHelp gh = (GacHelp)XmlSerializer.LoadFromXml(helpfile, typeof(GacHelp));
                gh.FileName = helpfile;
                return gh;

            }
            GacHelp ch = new GacHelp();
            ch.FileName = helpfile;
            Console.WriteLine("分析文件:" + filename);

            string code = File.ReadAllText(filename);

            //include
            MatchCollection mcsInclude = reginclude.Matches(code);
            if (mcsInclude.Count > 0)
            {
                ch.inlude = new string[mcsInclude.Count];
                for (int i = 0; i < mcsInclude.Count; i++)
                {
                    ch.inlude[i] = mcsInclude[i].Groups[1].Value;
                }
            }

            //extern
            MatchCollection mcsExtern = regExtern.Matches(code);
            if (mcsExtern.Count > 0)
            {

                for (int i = 0; i < mcsExtern.Count; i++)
                {
                    ch.Extern.Add(new ExternInfo() { Name = mcsExtern[i].Groups[2].Value, Class = mcsExtern[i].Groups[1].Value });
                }
            }

            MatchCollection mcsDefine= regdefine.Matches(code);
            if (mcsDefine.Count>0)
            {
                for (int i = 0; i < mcsDefine.Count; i++)
                {
                    ch.define.Add(new DefineInfo() { Value = mcsDefine[i].Groups[2].Value, Desc = mcsDefine[i].Groups[1].Value });
                }
            }

            //class
            MatchCollection mcsClass = regClass.Matches(code);
            if (mcsClass.Count > 0)
            {
                //[mcsInclude.Count];
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
                    MatchCollection mcsPublic = regexClassPublic.Matches(mcsClass[i].Groups[4].Value);
                    for (int i2 = 0; i2 < mcsPublic.Count; i2++)
                    {
                        MatchCollection mcsMethods = regMethods.Matches(mcsPublic[i2].Groups[1].Value);
                        foreach (Match match in mcsMethods)
                        {
                            Member mb = new Member();
                            mb.returns.DataType = match.Groups[1].Value;
                            mb.Name = match.Groups[2].Value;
                            mb.Param = GetParams(match.Groups[3].Value);
                            ci.MemberList.Add(mb);
                            if (mb.Name==ci.Name)
                            {
                                ci.Static = false;
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
                   
                    ch.Class.Add(ci);
                }
            }


            XmlSerializer.SaveToXml(helpfile, ch, typeof(GacHelp), "GacHelp");
            return ch;
        }
        static List<Param> GetParams(string param)
        {
            List<Param> list = new List<Param>();
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
                        if (!dicResult[item.Key].DicMethod.ContainsKey(item3.Name))
                        {
                            dicResult[item.Key].DicMethod.Add(item3.Name, new List<Member>());
                        }
                        //Methods添加提示
                        dicResult[item.Key].DicMethod[item3.Name].AddRange(item3.MemberList);

                        //继承类添加提示

                        GetInfo gi = GetInheritance(dic, item3.Inheritance);
                        dicResult[item.Key].DicMethod[item3.Name].AddRange(gi.ListMember);
                        dicResult[item.Key].Defines.AddRange(gi.ListDefine);


                        //类名添加提示 改调用词典
                        //if (!dicResult[item.Key].List.Contains(item3.Name))
                        //{
                        //    dicResult[item.Key].List.Add(item3.Name);
                        //}



                    }
                    foreach (var item3 in item2.Extern)
                    {
                        if (!dicResult[item.Key].DicMethod.ContainsKey(item3.Name))
                        {
                            if (dicResult[item.Key].DicMethod.ContainsKey(item3.Class))
                            {
                                dicResult[item.Key].DicMethod.Add(item3.Name, new List<Member>());
                                dicResult[item.Key].DicMethod[item3.Name].AddRange(dicResult[item.Key].DicMethod[item3.Class]);
                                //dicResult[item.Key].List.Add(item3.Name);
                            }
                        }
                    }
                    //常量获取
                    dicResult[item.Key].Defines.AddRange(item2.define);
                    //公开函数,主要用于系统类
                    dicResult[item.Key].Methods.AddRange(item2.MemberList);
                }
            }

            return dicResult;
        }
        GetInfo GetInheritance(Dictionary<string, List<GacHelp>> dic,string[] inheritance)
        {
            GetInfo gi = new GetInfo();
            List<Member> list = new List<Member>();
            if (inheritance==null||inheritance.Length==0)
            {
                return gi;
            }
            foreach (var item in dic.Keys)
            {
                foreach (var item2 in dic[item])
                {
                    foreach (var item3 in item2.Class)
                    {

                        if (((IList)inheritance).Contains(item3.Name))//说明此类是被继承的
                        {
                            gi.ListMember.AddRange(item3.MemberList);
                           GetInfo gitemp= GetInheritance(dic, item3.Inheritance);
                            
                            gi.ListMember.AddRange(gitemp.ListMember);
                            gi.ListDefine.AddRange(gitemp.ListDefine);
                            //gi.ListAttribute.AddRange(gitemp.ListAttribute);
                        }
                        
                        
                    }
                    gi.ListDefine.AddRange( item2.define);
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
       

       // List<string> _list = new List<string>();

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

        //public List<string> List
        //{
        //    get
        //    {
        //        return _list;
        //    }

        //    set
        //    {
        //        _list = value;
        //    }
        //}
    }
    public class GetInfo
    {
        List<Member> _listMember = new List<Member>();
        List<DefineInfo> _listDefine = new List<DefineInfo>();

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
    }
}
