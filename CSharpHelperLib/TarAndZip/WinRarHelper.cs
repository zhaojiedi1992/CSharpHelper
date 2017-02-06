using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;

namespace CSharpHelperLib.TarAndZip
{

    /// <summary>
    /// 需要安装WinRar软件,使用WinRar.exe完成压缩，解压工作
    /// 
    /// </summary>

    public static class WinRarHelper
    {
        public static string WinRarExePath { get; set; }

        /// <summary>
        /// 将目录和文件压缩为rar格式并保存到指定的目录
        /// </summary>
        /// <param name="soruceDir">要压缩的文件夹目录</param>
        /// <param name="rarFileName">压缩后的rar保存路径</param>
        public static void CompressRar(string soruceDir, string rarFileName)
        {
            if (WinRarExePath == null)
            {
                GetWinRarExePathFromRegistryTable();
            }
            if (WinRarExePath == null)
            {
                throw new Exception("cant find Winrar.exe path");
            }

            string winrarDir = System.IO.Path.GetDirectoryName(WinRarExePath);
            String commandOptions = $"a {rarFileName} {soruceDir} -r";

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = System.IO.Path.Combine(winrarDir, "rar.exe");
            processStartInfo.Arguments = commandOptions;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
        }

        public static void CompressRar(string soruceDir, string rarFileName ,out ProcessResultInfo.ProcessResultInfo resultInfo)
        {

            if (WinRarExePath == null)
            {
                GetWinRarExePathFromRegistryTable();
            }
            if (WinRarExePath == null)
            {
                throw new Exception("cant find Winrar.exe path");
            }
            FileInfo fileInfo = new FileInfo(rarFileName);
            if (!fileInfo.Exists)
            {
                throw new Exception("rar file not found ");
            }
            resultInfo = new ProcessResultInfo.ProcessResultInfo();
            String commandOptions = $" a \"{soruceDir}\" \"{rarFileName}\" -y";
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = $"\"{WinRarExePath}\"",
                    Arguments = commandOptions,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
                resultInfo.OutMessage = process.StandardOutput.ReadToEnd();
                resultInfo.ErrorMessage = process.StandardError.ReadToEnd();
                resultInfo.ExitCode = process.ExitCode;
                resultInfo.TotalProcessorTime = process.TotalProcessorTime;
                process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 将格式为rar的压缩文件解压到指定的目录
        /// </summary>
        /// <param name="rarFileName">要解压rar文件的路径</param>
        /// <param name="saveDir">解压后要保存到的目录</param>
        /// <param name="resultInfo">进程执行的结果信息类</param>
        public static void DeCompressRar(string rarFileName, string saveDir,out ProcessResultInfo.ProcessResultInfo resultInfo)
        {
            
            if (WinRarExePath == null)
            {
                GetWinRarExePathFromRegistryTable();
            }
            if (WinRarExePath == null)
            {
                throw  new Exception("cant find Winrar.exe path");
            }
            FileInfo fileInfo = new FileInfo(rarFileName);
            if(!fileInfo.Exists)
            {
                throw  new Exception("rar file not found ");
            }
            saveDir = saveDir.EndsWith("\\") ? saveDir : saveDir + "\\";
            resultInfo = new ProcessResultInfo.ProcessResultInfo();
            String commandOptions = $" x \"{rarFileName}\" \"{saveDir}\" -y";
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = "\"" + WinRarExePath + "\"",
                    Arguments = commandOptions,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
                resultInfo.OutMessage = process.StandardOutput.ReadToEnd();
                resultInfo.ErrorMessage = process.StandardError.ReadToEnd();
                resultInfo.ExitCode = process.ExitCode;
                resultInfo.TotalProcessorTime = process.TotalProcessorTime;
                process.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string ParseWinRaraExitCode(int exitCode)
        {
            string defaultInfo = "执行结果未知";
            Dictionary<int, string> info = new Dictionary<int, string>
            {
                {0, "成功操作"},
                {1, "警告发生非致命错误"},
                {2, "发生致命错误"},
                {3, "无效校验和数据损坏"},
                {4, "尝试修改一个锁定的压缩文件"},
                {5, "写错误"},
                {6, "文件打开错误"},
                {7, "错误命令行选项"},
                {8, "内存不足"},
                {9, "文件创建错误"},
                {10, "没有找到与指定的掩码和选项匹配的文件"},
                {11, "密码错误"},
                {255, "用户中断"}
            };
            try
            {
                defaultInfo = info.First(r => r.Key == exitCode).Value;
            }
            catch (Exception ex)
            {
                //todo nothing 
            }
            return defaultInfo;

        }
        private static void GetWinRarExePathFromRegistryTable()
        {
            string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regKey);
            if (registryKey != null)
            {
                WinRarExePath = registryKey.GetValue("").ToString();
                registryKey.Close();
            }
            else
            {
                throw new Exception("can't fand winrar.exe in RegistryTable,you can set WinRarExePath");
            }
        }
    } 
}