using DotNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LdgArduinoIde
{
    public class IdeConfigInfo
    {
        string _IdePath = "";
        string _ThemeName = "VS";
        string _Lately = "";
        string _COM = "";
        string _Boards = "";
        string _Package = "";
        string _Arch = "";
        int _DebugModule = 0;
        int _SmartTip = 1;
        bool _UploadVerbose = false;
        bool _BuildVerbose = false;
        int _CodeStyle = 1;
        bool _DeleteEmptyLines=true;
        bool _UnpadParen = true;
        bool _IndentCol1Comments = true;

        public string IdePath
        {
            get
            {
                return _IdePath;
            }

            set
            {
                _IdePath = value;
            }
        }

        public string ThemeName
        {
            get
            {
                return _ThemeName;
            }

            set
            {
                _ThemeName = value;
            }
        }

        public int DebugModule
        {
            get
            {
                return _DebugModule;
            }

            set
            {
                _DebugModule = value;
            }
        }
        /// <summary>
        /// 最近打开的文件
        /// </summary>
        public string Lately
        {
            get
            {
                return _Lately;
            }

            set
            {
                _Lately = value;
            }
        }

        public bool UploadVerbose
        {
            get
            {
                return _UploadVerbose;
            }

            set
            {
                _UploadVerbose = value;
            }
        }

        public bool BuildVerbose
        {
            get
            {
                return _BuildVerbose;
            }

            set
            {
                _BuildVerbose = value;
            }
        }

        /// <summary>
        /// Com口
        /// </summary>

        public string COM
        {
            get
            {
                return _COM;
            }

            set
            {
                _COM = value;
            }
        }
        /// <summary>
        /// board是要使用的实际电路板，如 所选架构文件夹中所包含的boards.txt中所定义。例如， Arduino的：AVR：UNO的Arduino的乌诺， Arduino的：AVR：diecimila为Arduino的Duemilanove或Diecimila，或Arduino的：AVR：大型的Arduino的兆。
        /// </summary>
        public string Boards
        {
            get
            {
                return _Boards;
            }

            set
            {
                _Boards = value;
            }
        }
        /// <summary>
        /// 包是供应商的标识符（硬件目录中的第一级文件夹）。默认的arduino板使用arduino。
        /// </summary>
        public string Package
        {
            get
            {
                return _Package;
            }

            set
            {
                _Package = value;
            }
        }
        /// <summary>
        /// 架构是主板的架构（硬件目录内的第二级文件夹）。默认的arduino板将arduino：avr用于所有基于AVR的板（例如Uno，Mega或Leonardo），或arduino：sam用于32位基于SAM的板（例如Arduino Due）。
        /// </summary>
        public string Arch
        {
            get
            {
                return _Arch;
            }

            set
            {
                _Arch = value;
            }
        }
        /// <summary>
        /// 智能提示方式 0无提示 1智能提示 2仅提示名称
        /// </summary>
        public int SmartTip
        {
            get
            {
                return _SmartTip;
            }

            set
            {
                _SmartTip = value;
            }
        }

        public int CodeStyle
        {
            get
            {
                return _CodeStyle;
            }

            set
            {
                _CodeStyle = value;
            }
        }

        public bool DeleteEmptyLines
        {
            get
            {
                return _DeleteEmptyLines;
            }

            set
            {
                _DeleteEmptyLines = value;
            }
        }

        public bool UnpadParen
        {
            get
            {
                return _UnpadParen;
            }

            set
            {
                _UnpadParen = value;
            }
        }

        public bool IndentCol1Comments
        {
            get
            {
                return _IndentCol1Comments;
            }

            set
            {
                _IndentCol1Comments = value;
            }
        }
    }
    public class IdeConfig
    {
        public static IdeConfigInfo ReadConfig()
        {
            //throw new Exception(AppDomain.CurrentDomain.BaseDirectory + "Config.ini");
            IdeConfigInfo config = new IdeConfigInfo();
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Config.txt"))
            {
                INIFileHelper ini = new INIFileHelper(AppDomain.CurrentDomain.BaseDirectory + "Config.txt");
                config.IdePath = ini.IniReadValue("Config", "IdePath", config.IdePath);
                config.ThemeName = ini.IniReadValue("Config", "ThemeName", config.ThemeName);
                config.DebugModule = ini.IniReadValueInt("Config", "DebugModule",config.DebugModule);
                config.SmartTip = ini.IniReadValueInt("Config", "SmartTip", config.SmartTip);
                config.Lately= LdgSetting.GetAppSetting( "Lately",config.Lately);
                config.COM = ini.IniReadValue("Config", "COM", ArduinoIde.GetValue("serial.port"));
                config.Boards = ini.IniReadValue("Config", "Boards", ArduinoIde.GetValue("board"));
                config.Package = ini.IniReadValue("Config", "Package", ArduinoIde.GetValue("target_package"));
                config.Arch = ini.IniReadValue("Config", "Arch", ArduinoIde.GetValue("target_platform"));

                config.CodeStyle = ini.IniReadValueInt("Config", "CodeStyle", config.CodeStyle);
                try
                {
                    config.BuildVerbose=Convert.ToBoolean( ini.IniReadValue("Config", "BuildVerbose", config.BuildVerbose.ToString()));
                    config.UploadVerbose = Convert.ToBoolean(ini.IniReadValue("Config", "UploadVerbose", config.UploadVerbose.ToString()));

                    
                    config.DeleteEmptyLines = Convert.ToBoolean(ini.IniReadValue("Config", "DeleteEmptyLines", config.DeleteEmptyLines.ToString()));
                    config.UnpadParen = Convert.ToBoolean(ini.IniReadValue("Config", "UnpadParen", config.UnpadParen.ToString()));
                    config.IndentCol1Comments = Convert.ToBoolean(ini.IniReadValue("Config", "IndentCol1Comments", config.IndentCol1Comments.ToString()));
                }
                catch (Exception)
                {

                }
            }
            return config;
        }
        public static void SaveConfig(IdeConfigInfo config)
        {
            INIFileHelper ini = new INIFileHelper(AppDomain.CurrentDomain.BaseDirectory + "\\Config.txt");
            ini.IniWriteValue("Config", "IdePath", config.IdePath);
            ini.IniWriteValue("Config", "ThemeName", config.ThemeName);
            ini.IniWriteValue("Config", "DebugModule", config.DebugModule);
            //ini.IniWriteValue("Config", "Lately", config.Lately);
            LdgSetting.SetAppSetting("Lately", config.Lately);
            ini.IniWriteValue("Config", "BuildVerbose", config.BuildVerbose.ToString());
            ini.IniWriteValue("Config", "UploadVerbose", config.UploadVerbose.ToString());
            ini.IniWriteValue("Config", "COM", config.COM);
            ini.IniWriteValue("Config", "Boards", config.Boards);
            ini.IniWriteValue("Config", "Package", config.Package);
            ini.IniWriteValue("Config", "Arch", config.Arch);
            ini.IniWriteValue("Config", "SmartTip", config.SmartTip);

            ini.IniWriteValue("Config", "CodeStyle", config.CodeStyle);
            ini.IniWriteValue("Config", "DeleteEmptyLines", config.DeleteEmptyLines.ToString());
            ini.IniWriteValue("Config", "UnpadParen", config.UnpadParen.ToString());
            ini.IniWriteValue("Config", "IndentCol1Comments", config.IndentCol1Comments.ToString());
        }
    }
}
