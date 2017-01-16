using System;
using System.Configuration;
using System.IO;

namespace RemoteSenseCommLib.Tools
{
    public  static class LogHelper
    {
        private static string _logBasePath = null;
        private static string _logFilePrefix = null;
        private static string _logFileSuffix = null;

        public  static string LogBasePath
        {
            get
            {
                if (_logBasePath != null)
                {
                    //先看设置获取
                    return _logBasePath;
                }
                if (_logBasePath == null)
                {   
                    //先配置文件获取
                    _logBasePath = ConfigurationManager.AppSettings["logBasePath"];
                }
                if(_logBasePath ==null)
                {
                    //在使用应用程序启动位置
                    _logBasePath = AppDomain.CurrentDomain.BaseDirectory;
                }
                return _logBasePath;
            }
            set { _logBasePath = value; }
       
        }
        public static string LogSubPath { get; set; }

        private static string LogFilePrefix
        {
            get { return _logFilePrefix ?? (_logFilePrefix = "Log_"); }
            set { _logFilePrefix = value; }
        }

        public static string LogFileSuffix
        {
            get { return _logFileSuffix ?? (_logFileSuffix = ".log"); }
            set { _logFileSuffix = value; }
        }

        public static string LogFullFileName
        {
            get
            {
                string result = string.Empty;
                if (string.IsNullOrEmpty(LogSubPath))
                {
                    result = Path.Combine(LogBasePath,
                        LogFilePrefix + "_" + DateTime.Now.ToString("yyyyMMdd") + LogFileSuffix);
                }
                else
                {
                    result = Path.Combine(LogBasePath, LogSubPath,
                        LogFilePrefix + "_" + DateTime.Now.ToString("yyyyMMdd") + LogFileSuffix);
                }
                return result;
            }
        }
        public static void WriteLog(LogInfoType logType, string msg)
        {
            StreamWriter sw = StreamWriter.Null;
            try
            {
                var directoryInfo = new FileInfo(LogFullFileName).Directory;
                if (directoryInfo != null && !directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                var fileInfo = new FileInfo(LogFullFileName);
                if (!fileInfo.Exists)
                {
                    fileInfo.Create();
                }
                sw = System.IO.File.AppendText(LogFullFileName);
                sw.WriteLine(logType + "#" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
            }
            catch (Exception ex)
            {
                // ignored
            }
            finally
            {
                if (sw != StreamWriter.Null)
                {
                    sw.Dispose();
                }

            }
        }


    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogInfoType
    {
        Trace,  //堆栈跟踪信息
        Warning,//警告信息
        Error,  //错误信息应该包含对象名、发生错误点所在的方法名称、具体错误信息
        SQL,    //与数据库相关的信息
        Message //普通信息
    }
}
