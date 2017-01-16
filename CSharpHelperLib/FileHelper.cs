using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteSenseCommLib.Tools
{
    public static class FileHelper
    {
        /// <summary>
        /// 获取指定文件的最后几行
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string GetLastNumberLines(string filename, int lines)
        {
            string result = string.Empty;
            List<string> listFile = new List<string>();
            StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                listFile.Add(sr.ReadLine());
            }
            if (listFile.Count > lines)
            {
                for (int i = listFile.Count - lines; i < listFile.Count - 1; i++)
                {
                    result += listFile[i] + Environment.NewLine;
                }
            }
            else
            {
                for (int i = 0; i < listFile.Count; i++)
                {
                    result += listFile[i] + Environment.NewLine;
                }
            }
            return result + listFile[listFile.Count - 1];
        }

        #region 创建删除相关
        public static bool CreateDir(string path )
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool DeleteDir(string path, bool recursion=false)
        {
            try
            {
                Directory.Delete(path,recursion);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool CreateFile(string path)
        {
            try
            {
                //如果文件不存在则创建该文件
                File.Create(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool DeleteFile(string file)
        {
            try
            {
              File.Delete(file);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
        public static string[] GetDir(string path)
        {
            string[] dirs = null;
            try
            {
               dirs= Directory.GetDirectories(path);
            }
            catch (Exception)
            {
               return dirs;
            }
            return dirs;
        }
        public static string[] GetFiles(string path)
        {
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception)
            {
                return files;
            }
            return files;
        }
        #region 文件大小和行数相关
        public static long GetLineCount(string path)
        {
            //将文本文件的各行读到一个字符串数组中
            long lineCount = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.ReadLine() !=null)
                {
                    lineCount++;
                }
            }
            return lineCount;
        }
        public static int GetFileSize(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);

            //获取文件的大小
            return (int) fi.Length;
        }
        public static double GetFileSizeByK(string filePath)
        {
            return (double)GetFileSize(filePath) / 1024;
        }
        public static double GetFileSizeByM(string filePath)
        {
            return GetFileSizeByK(filePath) / 1024;
        }
        #endregion
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                if (directoryInfo.GetDirectories().Length>0)
                {
                    return false;
                }
                if (directoryInfo.GetFiles().Length >0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                throw new Exception("IsEmptyDirectory函数异常"+"参数" + directoryPath);
            }
        }
      
        public static void ClearFile(string filePath)
        {
            File.Delete(filePath);
            CreateFile(filePath);
        }


    }
}
