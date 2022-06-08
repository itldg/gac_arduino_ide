using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace LdgArduinoIde
{
    public class ArduinoIde
    {
        static string preferences = "";
        public static string GetValue(string name,string defaultvalue="")
        {
            if (string.IsNullOrEmpty(preferences))
            {
                preferences = File.ReadAllText(Environment.GetEnvironmentVariable("LOCALAPPDATA")+"/Arduino15/preferences.txt");
            }
            Regex reg = new Regex("^"+Regex.Escape(name)+"=(.+)$",RegexOptions.Multiline);
            Match match= reg.Match(preferences);
            if (match.Success)
            {
                return match.Groups[1].Value.TrimEnd();
            }
            return defaultvalue;
        }
        public static string GetValue(string filename,string name, string defaultvalue = "")
        {
            
            string str = File.ReadAllText(filename);
            Regex reg = new Regex("^" + Regex.Escape(name) + "=(.+)$", RegexOptions.Multiline);
            Match match = reg.Match(str);
            if (match.Success)
            {
                return match.Groups[1].Value.TrimEnd();
            }
            return defaultvalue;
        }
    }
}
