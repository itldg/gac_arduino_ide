using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LdgArduinoIde
{
    [Serializable]
    public class ArduinoReferences
    {
        string _Name = "";
        int _Type = 0;
        string _DataType = "";
        string _ExampleCode = "";
        string _Description = "";
        string _NotesAndWarnings = "";
        string _Syntax = "";
        string _Parameters = "";
        string _Returns = "";
        string _UseLibrary="";
        string _Closes = "";
        string _Parents = "";
        /// <summary>
        /// 名字
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = Regex.Replace(value, ".*? - ", "");
                //_Name = value;
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
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
        /// <summary>
        /// 示例代码
        /// </summary>
        public string ExampleCode
        {
            get
            {
                return _ExampleCode;
            }

            set
            {
                _ExampleCode = value;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get
            {
                return _Description;
            }

            set
            {
                _Description = value;
            }
        }
        /// <summary>
        /// 说明和警告
        /// </summary>
        public string NotesAndWarnings
        {
            get
            {
                return _NotesAndWarnings;
            }

            set
            {
                _NotesAndWarnings = value;
            }
        }
        /// <summary>
        /// 语法示例
        /// </summary>
        public string Syntax
        {
            get
            {
                return _Syntax;
            }

            set
            {
                _Syntax = value;
            }
        }
        /// <summary>
        /// 参数
        /// </summary>
        public string Parameters
        {
            get
            {
                return _Parameters;
            }

            set
            {
                _Parameters = value;
            }
        }
        /// <summary>
        /// 返回
        /// </summary>
        public string Returns
        {
            get
            {
                return _Returns;
            }

            set
            {
                _Returns = value;
            }
        }
        /// <summary>
        /// 类型 0方法 1属性 2类 3常量 4变量类型 5控制分支 6需要声明的类
        /// </summary>
        public int Type
        {
            get
            {
                return _Type;
            }

            set
            {
                _Type = value;
            }
        }
        /// <summary>
        /// 该类引用说明
        /// </summary>
        public string UseLibrary
        {
            get
            {
                return _UseLibrary;
            }

            set
            {
                _UseLibrary = value;
            }
        }
        /// <summary>
        /// 祖宗节点 所属
        /// </summary>
        public string Closes
        {
            get
            {
                return _Closes;
            }

            set
            {
                _Closes = value;
            }
        }
        /// <summary>
        /// 所属节点
        /// </summary>
        public string Parents
        {
            get
            {
                return _Parents;
            }

            set
            {
                _Parents = value;
            }
        }
    }
    
}
