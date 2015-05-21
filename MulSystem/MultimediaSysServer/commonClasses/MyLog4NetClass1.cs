using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimediaSysServer.commonClasses
{
    public class MyLog4NetClass1
    {
        string ConfigFile;

        public void LoadLogConfig()
        {
            if (ConfigFile == null)
            {
                return;
            }
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ConfigFile));
        }

        public void LogFactoryBase(string configFile)
        {
            if (Path.IsPathRooted(configFile))
            {
                ConfigFile = configFile;
                return;
            }
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);

            if (File.Exists(filePath))
            {
                ConfigFile = filePath;
                return;
            }

            filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config"), configFile);//这里的config是debug下的子文件夹

            if (File.Exists(filePath))
            {
                ConfigFile = filePath;
                return;
            }
        }
    }

    public class LogHelper
    {
        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            //log.IsErrorEnabled = true;
            log.Info("Info", ex);
        }


        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }
    }

}
