using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ArduinoHelp
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("GacHelp", IsNullable = false)]
    public class GacHelp
    {
        public string FileName { get; set; }
        string[] _include = new string[] { };
        List<Member> _MemberList = new List<Member>();
        List<ClassInfo> _Class = new List<ClassInfo>();
        List<ExternInfo> _Extern = new List<ExternInfo>();
        
        

        [XmlElementAttribute("inlude", IsNullable = true)]
        public string[] inlude
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
        string _summary = "";
        string _desc = "";
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
                _DataType = value;
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
                _DataType = value;
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
                _Name = value;
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
    }
}
