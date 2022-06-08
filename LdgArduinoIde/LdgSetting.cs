using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LdgArduinoIde
{
    public class LdgSetting
    {

        //向配置文件中添加键值对，有则修改，无则添加
        public static void SetAppSetting(string key, string value)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add(key, value);
                config.Save();
                //return;
            }
            else
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings[key].Value = value;
                cfa.Save();
            }
        }
        public static string GetAppSetting(string key,  string defaultValue="")
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key].ToString();
            }
            return defaultValue;
        }
        public static int GetAppSetting(string key, int defaultValue = 0)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString());
                }
                catch (Exception)
                {
                }
                
            }
            return defaultValue;
        }
    }
}
