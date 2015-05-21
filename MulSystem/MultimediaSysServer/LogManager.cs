using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;


namespace MultimediaSysServer
{
    public class LogManager
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogFile
        {
            Trace,
            Warning,
            Error,
            SQL
        }

        private static string logPath = string.Empty;
        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    logPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
                    if (!System.IO.Directory.Exists(logPath))
                        System.IO.Directory.CreateDirectory(logPath);
                }
                return logPath;
            }
            set { logPath = value; }
        }

        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string logFile, string msg)
        {
            FileStream fs = null;
            try
            {
                string file = LogPath + LogFielPrefix + logFile + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                fs = File.Open(file, FileMode.OpenOrCreate);

                if (fs.Length < 10048576)//写文件不能超过1mb
                {
                    fs.Flush();
                    fs.Close();
                    System.IO.StreamWriter sw = System.IO.File.AppendText(file);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    File.Delete(file);
                }
            }
            catch
            { }
            try
            {
                fs.Flush();
                fs.Close();
            }
            catch { }
        }
    }

}
