using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GacArduinoHelper
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("GacHelp", IsNullable = false)]
    public class GacHelp
    {
        public string FileName { get; set; }
        List<string> _include = new List<string>();
        List<Member> _MemberList = new List<Member>();
        List<ClassInfo> _Class = new List<ClassInfo>();
        List<ExternInfo> _Extern = new List<ExternInfo>();
        List<Struct> _Struct = new List<Struct>();



        [XmlElementAttribute("inlude", IsNullable = true)]
        public List<string> inlude
        {
            get
            {
                return _include;
            }

            set
            {
                _include = value;
            }
        }

        [XmlElementAttribute("class", IsNullable = false)]
        public List<ClassInfo> Class
        {
            get
            {
                return _Class;
            }

            set
            {
                _Class = value;
            }
        }

        

        [System.Xml.Serialization.XmlElementAttribute("member")]
        public List<Member> MemberList
        {
            get
            {
                return _MemberList;
            }

            set
            {
                _MemberList = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("extern")]
        public List<ExternInfo> Extern
        {
            get
            {
                return _Extern;
            }

            set
            {
                _Extern = value;
            }
        }
        List<DefineInfo> _define = new List<DefineInfo>();
        [System.Xml.Serialization.XmlElementAttribute("define")]
        public List<DefineInfo> define
        {
            get
            {
                return _define;
            }

            set
            {
                _define = value;
            }
        }

        List<EnumInfo> _EnumList = new List<EnumInfo>();
        [System.Xml.Serialization.XmlElementAttribute("enums")]
        public List<EnumInfo> Enum
        {
            get
            {
                return _EnumList;
            }

            set
            {
                _EnumList = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("structs")]
        public List<Struct> Struct
        {
            get
            {
                return _Struct;
            }

            set
            {
                _Struct = value;
            }
        }
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("member", IsNullable = false)]
    public class Member
    {
        List<Param> _Param = new List<Param>();
        Return _returns = new Return();
       // string _Class = "";
        string _Name = "";
        bool _Static = false;
        string _summary = "";
        bool _isAttr = false;
        //[XmlAttribute("class")]
        //public string Class
        //{
        //    get
        //    {
        //        return _Class;
        //    }

        //    set
        //    {
        //        _Class = value;
        //    }
        //}

        [XmlAttribute("name")]
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
        [XmlAttribute("static")]
        public bool Static
        {
            get
            {
                return _Static;
            }

            set
            {
                _Static = value;
            }
        }
        [XmlAttribute("isattr")]
        public bool isAttr
        {
            get
            {
                return _isAttr;
            }

            set
            {
                _isAttr = value;
            }
        }
        [XmlElementAttribute("summary", IsNullable = true)]
        public string summary
        {
            get
            {
                return _summary;
            }

            set
            {
                _summary = value;
            }
        }


        [XmlElementAttribute("returns", IsNullable = false)]
        public Return returns
        {
            get
            {
                return _returns;
            }

            set
            {
                _returns = value;
            }
        }

        [XmlElementAttribute("param", IsNullable = false)]

        public List<Param> Param
        {
            get
            {
                return _Param;
            }

            set
            {
                _Param = value;
            }
        }
        public string GetParams
        {
            get {

                string MethodParam = "";
                for (int i = 0; i < Param.Count; i++)
                {
                    if (!string.IsNullOrEmpty(MethodParam))
                    {
                        MethodParam += ",";
                    }
                    MethodParam += (Param[i].DataType == "" ? "" : Param[i].DataType + " ") + Param[i].Name + (Param[i].Default == "" ? "" : " = " + Param[i].Default);
                }
                return MethodParam;
            }
            
        }
    }
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("returns", IsNullable = false)]
    public class Return
    {
        string _DataType = "";
        string _Value = "";
        [XmlAttribute("datatype")]
        public string DataType
        {
            get
            {
                return _DataType;
            }

            set
            {
                _DataType = value.Trim();
            }
        }
        [XmlText]
        public string Desc
        {

            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }

    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("define", IsNullable = false)]
    public class DefineInfo
    {
        string _Value = "";
        string _Desc = "";
        [XmlAttribute("value")]
        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }
        string _Name = "";
        [XmlAttribute("name")]
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
        [XmlText]
        public string Desc
        {

            get
            {
                return _Desc;
            }

            set
            {
                _Desc = value;
            }
        }

    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("class", IsNullable = false)]
    public class ClassInfo
    {


        [XmlAttribute("name")]
        public string Name
        {

            get;
            set;
        }
        string _summary = "";
        [XmlAttribute("summary")]
        public string Summary
        {

            get
            {
                return _summary;
            }

            set
            {
                if (value==null)
                {
                    return;
                }
                _summary = value;
            }
        }
        List<Member> _MemberList = new List<Member>();
        [System.Xml.Serialization.XmlElementAttribute("member")]
        public List<Member> MemberList
        {
            get
            {
                return _MemberList;
            }

            set
            {
                _MemberList = value;
            }
        }




        string[] _Inheritance = new string[] { };
        [System.Xml.Serialization.XmlElementAttribute("inheritance")]
        public string[] Inheritance
        {
            get
            {
                return _Inheritance;
            }

            set
            {
                _Inheritance = value;
            }
        }
        bool _Static = true;
        [XmlAttribute("static")]
        public bool Static
        {
            get
            {
                return _Static;
            }

            set
            {
                _Static = value;
            }
        }

        

    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("param", IsNullable = false)]
    public class Param
    {
        string _DataType = "";
        string _Name = "";
        string _Default = "";
        string _desc = "";
        [XmlAttribute("datatype")]
        public string DataType
        {
            get
            {
                return _DataType;
            }

            set
            {
                _DataType = value.Trim();
            }
        }
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (value=="void")
                {
                    _Name = "";
                }else
                { 
                    _Name = value;
                }
            }
        }
        [XmlAttribute("default")]
        public string Default
        {
            get
            {
                return _Default;
            }

            set
            {
                _Default = value;
            }
        }
        [XmlText]
        public string Desc
        {

            get
            {
                return _desc;
            }

            set
            {
                _desc = value;
            }
        }


    }
    public class ExternInfo
    {
        string _Name = "";
        [XmlAttribute("name")]
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

        string _Class = "";
        [XmlAttribute("class")]
        public string Class
        {
            get
            {
                return _Class;
            }

            set
            {
                _Class = value;
            }
        }
        /// <summary>
        /// Using 直接静态
        /// </summary>
       [XmlAttribute("static")]
        public bool Static
        {
            get
            {
                return _Static;
            }

            set
            {
                _Static = value;
            }
        }

        bool _Static = false;
    }
    public class EnumList
    {
        string _Desc = "";
        [XmlText]
        public string Desc
        {
            get
            {
                return _Desc;
            }

            set
            {
                _Desc = value;
            }
        }

        string _Name = "";
        [XmlAttribute("name")]
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

        string _Value = "";
        [XmlAttribute("value")]
        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }

    }


    public class EnumInfo
    {
        string _Name = "";
        [XmlAttribute("name")]
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
        string _Summary = "";
        [XmlAttribute("summary")]
        public string Summary
        {
            get
            {
                return _Summary;
            }

            set
            {
                _Summary = value;
            }
        }

        

        List<EnumList> _ListEnum = new List<EnumList>();
        [System.Xml.Serialization.XmlElementAttribute("enum")]
        public List<EnumList> ListEnum
        {
            get
            {
                return _ListEnum;
            }

            set
            {
                _ListEnum = value;
            }
        }

       
    }


    public class Struct
    {
        string _Name = "";
        [XmlAttribute("name")]
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
        string _Summary = "";
        [XmlAttribute("summary")]
        public string Summary
        {
            get
            {
                return _Summary;
            }

            set
            {
                _Summary = value;
            }
        }



        List<Param> _Struct = new List<Param>();
        [System.Xml.Serialization.XmlElementAttribute("struct")]
        public List<Param> ListMember
        {
            get
            {
                return _Struct;
            }

            set
            {
                _Struct = value;
            }
        }


    }
}
