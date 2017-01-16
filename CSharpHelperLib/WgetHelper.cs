using System;
using System.Diagnostics;
using System.IO;

namespace RemoteSenseCommLib.Tools
{
    public class WgetHelper
    {
        
        //private string exe = ConfigurationManager.AppSettings["wgetPath"];
        public WgetHelper()
        {
            
        }
        public bool DownFileFromUrl(string url,string log,string localFile)
        {
            var logDirectoryInfo = new FileInfo(log).Directory;
            if (logDirectoryInfo != null && !logDirectoryInfo.Exists)
            {
                logDirectoryInfo.Create();
            }
            var dataDirectoryInfo = new FileInfo(localFile).Directory;
            if (dataDirectoryInfo != null && !dataDirectoryInfo.Exists)
            {
                dataDirectoryInfo.Create();
            }
            string argument = string.Format(" -o {0} -O {1} -c -N {2}", log, localFile, url);
            //创建进程进行处理
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = "wget.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = argument,
                    CreateNoWindow = true
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception)
            {
                return false;
            }
            return process.ExitCode==0;
        }
        public bool DownFileFromTxt(string needDownTxt, string log, string localPath)
        {
            //C:\wgetwin-1_5_3_1-binary\wget.exe  -o e:\test\downlog2.txt -r  -np -k -P e:\test ftp://acdisc.gsfc.nasa.gov -I OMTO3e.003*
            string cmd = string.Format("wget.exe -o {0} - P {1}-i {2}", log, localPath, needDownTxt);
            return true;
        }
    }
}
