﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpHelperLib.Ftp
{
    /// <summary/>
    /// FTP处理操作类
    /// 功能：
    /// 下载文件
    /// 上传文件
    /// 上传文件的进度信息
    /// 下载文件的进度信息
    /// 删除文件
    /// 列出文件
    /// 列出目录
    /// 进入子目录
    /// 退出当前目录返回上一层目录
    /// 判断远程文件是否存在
    /// 判断远程文件是否存在
    /// 删除远程文件    
    /// 建立目录
    /// 删除目录
    /// 文件（目录）改名

    /************************************************************************相关类和枚举**************************************************************************/
    #region 文件结构
    public struct FileStruct
    {
        public string Flags;
        public string Owner;
        public string Group;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
    } 
    #endregion
    #region ftp文件列表风格枚举
    /// <summary>
    /// ftp文件列表风格枚举
    /// </summary>
    public enum FileListStyleType
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }
    #endregion
    /************************************************************************委托定义和委托对象**************************************************************************/
    public delegate void DownloadProgressChangedDelegateType(object sender, DownloadProgressChangedEventArgs e);
    public delegate void DownloadDataCompletedDelegateType(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    public delegate void UploadProgressChangedDelegateType(object sender, UploadProgressChangedEventArgs e);
    public delegate void UploadFileCompletedDelegateType(object sender, UploadFileCompletedEventArgs e);
 
    /************************************************************************ftphelper具体类**************************************************************************/
    public class FtpHelper 
    {
        #region 属性信息

        /// <summary>
        /// FTP请求对象
        /// </summary>
        public FtpWebRequest Request { get; private set; }

        /// <summary>
        /// FTP响应对象
        /// </summary>
        public FtpWebResponse Response { get; private set; } 

        /// <summary>
        /// FTP服务器地址
        /// </summary>
        private Uri _uri;
        /// <summary>
        /// FTP服务器地址
        /// </summary>
        public Uri Uri
        {
            get
            {
                if (DirectoryPath == "/")
                {
                    return _uri;
                }
                else
                {
                    string strUri = _uri.ToString();
                    if (strUri.EndsWith("/"))
                    {
                        strUri = strUri.Substring(0, strUri.Length - 1);
                    }
                    return new Uri(strUri + DirectoryPath);
                }
            }
            set
            {
                if (value.Scheme != Uri.UriSchemeFtp)
                {
                    throw new Exception("Ftp 地址格式错误!");
                }
                _uri = new Uri(value.GetLeftPart(UriPartial.Authority));
                DirectoryPath = value.AbsolutePath;
                if (!DirectoryPath.EndsWith("/"))
                {
                    DirectoryPath += "/";
                }
            }
        }

        /// <summary>
        /// 当前工作目录
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// FTP登录用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// FTP登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 连接FTP服务器的代理服务
        /// </summary>
        public WebProxy Proxy { get; set; } 

        /// <summary>
        /// 是否需要删除临时文件
        /// </summary>
        public bool IsDeleteTempFile { get; private set; } 

        /// <summary>
        /// 异步上传所临时生成的文件
        /// </summary>
        public string UploadTempFile { get; private set; } = "";

        #endregion
        #region 事件
        /************************************************************************委托对象**************************************************************************/
        /// <summary>
        /// 异步下载进度发生改变触发的事件
        /// </summary>
        public event DownloadProgressChangedDelegateType DownloadProgressChanged;
        /// <summary>
        /// 异步下载文件完成之后触发的事件
        /// </summary>
        public event DownloadDataCompletedDelegateType DownloadDataCompleted;
        /// <summary>
        /// 异步上传进度发生改变触发的事件
        /// </summary>
        public event UploadProgressChangedDelegateType UploadProgressChanged;
        /// <summary>
        /// 异步上传文件完成之后触发的事件
        /// </summary>
        public event UploadFileCompletedDelegateType UploadFileCompleted;
        #endregion
        /************************************************************************构造析构函数**************************************************************************/
        #region 构造析构函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ftpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>
        public FtpHelper(Uri ftpUri, string strUserName, string strPassword)
        {
           _uri = new Uri(ftpUri.GetLeftPart(UriPartial.Authority));
            DirectoryPath = ftpUri.AbsolutePath;
            if (!DirectoryPath.EndsWith("/"))
            {
                DirectoryPath += "/";
            }
           UserName = strUserName;
           Password = strPassword;
           Proxy = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ftpUri">FTP地址</param>
        /// <param name="strUserName">登录用户名</param>
        /// <param name="strPassword">登录密码</param>
        /// <param name="objProxy">连接代理</param>
        public FtpHelper(Uri ftpUri, string strUserName, string strPassword, WebProxy objProxy)
        {
           _uri = new Uri(ftpUri.GetLeftPart(UriPartial.Authority));
            DirectoryPath = ftpUri.AbsolutePath;
            if (!DirectoryPath.EndsWith("/"))
            {
                DirectoryPath += "/";
            }
            UserName = strUserName;
            Password = strPassword;
            Proxy = objProxy;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public FtpHelper()
        {
           UserName = "anonymous";  //匿名用户
           Password = "@anonymous";
           _uri = null;
           Proxy = null;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FtpHelper()
        {
            if (Response != null)
            {
                Response.Close();
                Response = null;
            }
            if (Request != null)
            {
                Request.Abort();
                Request = null;
            }
        }
        #endregion
        /************************************************************************打开连接**************************************************************************/

        #region 建立连接
        /// <summary>
        /// 建立FTP链接,返回响应对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="ftpMathod">操作命令</param>
        private FtpWebResponse Open(Uri uri, string ftpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = ftpMathod;
                Request.UseBinary = true;
                Request.Credentials = new NetworkCredential(UserName, Password);
                if (Proxy != null)
                {
                    Request.Proxy = Proxy;
                }
                return (FtpWebResponse)Request.GetResponse();
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 建立FTP链接,返回请求对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="ftpMathod">操作命令</param>
        private FtpWebRequest OpenRequest(Uri uri, string ftpMathod)
        {
            try
            {
                Request = (FtpWebRequest)WebRequest.Create(uri);
                Request.Method = ftpMathod;
                Request.UseBinary = true;
                Request.Credentials = new NetworkCredential(UserName, Password);
                if (Proxy != null)
                {
                    Request.Proxy =Proxy;
                }
                return Request;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        /************************************************************************下载文件**************************************************************************/
        #region 下载文件

        /// <summary>
        /// 从FTP服务器下载文件，使用与远程文件同名的文件名来保存文件
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localPath">本地路径</param>

        public bool DownloadFile(string remoteFileName, string localPath)
        {
            return DownloadFile(remoteFileName, localPath, remoteFileName);
        }

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="localFileName">保存本地的文件名</param>
        public bool DownloadFile(string remoteFileName, string localPath, string localFileName)
        {
            byte[] bt;
            try
            {
                if (!IsValidFileChars(remoteFileName) || !IsValidFileChars(localFileName) || !IsValidPathChars(localPath))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (!Directory.Exists(localPath))
                {
                    throw new Exception("本地文件路径不存在!");
                }

                string localFullPath = Path.Combine(localPath, localFileName);
                if (File.Exists(localFullPath))
                {
                    throw new Exception("当前路径下已经存在同名文件！");
                }
                bt = DownloadFile(remoteFileName);
                if (bt != null)
                {
                    FileStream stream = new FileStream(localFullPath, FileMode.Create);
                    stream.Write(bt, 0, bt.Length);
                    stream.Flush();
                    stream.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }

        /// <summary>
        /// 从FTP服务器下载文件，返回文件二进制数据
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        public byte[] DownloadFile(string remoteFileName)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                Response = Open(new Uri(Uri + remoteFileName), WebRequestMethods.Ftp.DownloadFile);
                Stream reader = Response.GetResponseStream();

                MemoryStream mem = new MemoryStream(1024 * 500);
                byte[] buffer = new byte[1024];
                while (true)
                {
                    if (reader != null)
                    {
                        var bytesRead = reader.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        mem.Write(buffer, 0, bytesRead);
                    }
                }
                if (mem.Length > 0)
                {
                    return mem.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        #region 异步下载文件
        /// <summary>
        /// 从FTP服务器异步下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>        
        /// <param name="localPath">保存文件的本地路径,后面带有"\"</param>
        /// <param name="localFileName">保存本地的文件名</param>
        public void DownloadFileAsync(string remoteFileName, string localPath, string localFileName)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName) || !IsValidFileChars(localFileName) || !IsValidPathChars(localPath))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (!Directory.Exists(localPath))
                {
                    throw new Exception("本地文件路径不存在!");
                }

                string localFullPath = Path.Combine(localPath, localFileName);
                if (File.Exists(localFullPath))
                {
                    throw new Exception("当前路径下已经存在同名文件！");
                }
                DownloadFileAsync(remoteFileName, localFullPath);

            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }

        /// <summary>
        /// 从FTP服务器异步下载文件，指定本地完整路径文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFullPath">本地完整路径文件名</param>
        public void DownloadFileAsync(string remoteFileName, string localFullPath)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (File.Exists(localFullPath))
                {
                    throw new Exception("当前路径下已经存在同名文件！");
                }
                MyWebClient client = new MyWebClient();

                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
                client.Credentials = new NetworkCredential(UserName, Password);
                if (Proxy != null)
                {
                    client.Proxy = Proxy;
                }
                client.DownloadFileAsync(new Uri(Uri + remoteFileName), localFullPath);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }

        /// <summary>
        /// 异步下载文件完成之后触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">数据信息对象</param>
        void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            DownloadDataCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// 异步下载进度发生改变触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">进度信息对象</param>
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(sender, e);
        }
        #endregion
        /************************************************************************上传文件**************************************************************************/
        #region 上传文件
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件名</param>
        public bool UploadFile(string localFullPath)
        {
            return UploadFile(localFullPath, Path.GetFileName(localFullPath), false);
        }
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public bool UploadFile(string localFullPath, bool overWriteRemoteFile)
        {
            return UploadFile(localFullPath, Path.GetFileName(localFullPath), overWriteRemoteFile);
        }
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        public bool UploadFile(string localFullPath, string remoteFileName)
        {
            return UploadFile(localFullPath, remoteFileName, false);
        }
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件名</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public bool UploadFile(string localFullPath, string remoteFileName, bool overWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName) || !IsValidFileChars(Path.GetFileName(localFullPath)) || !IsValidPathChars(Path.GetDirectoryName(localFullPath)))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (File.Exists(localFullPath))
                {
                    FileStream stream = new FileStream(localFullPath, FileMode.Open, FileAccess.Read);
                    byte[] bt = new byte[stream.Length];
                    stream.Read(bt, 0, (Int32)stream.Length);   //注意，因为Int32的最大限制，最大上传文件只能是大约2G多一点
                    stream.Close();
                    return UploadFile(bt, remoteFileName, overWriteRemoteFile);
                }
                else
                {
                    throw new Exception("本地文件不存在!");
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="fileBytes">上传的二进制数据</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        public bool UploadFile(byte[] fileBytes, string remoteFileName)
        {
            if (!IsValidFileChars(remoteFileName))
            {
                throw new Exception("非法文件名或目录名!");
            }
            return UploadFile(fileBytes, remoteFileName, false);
        }
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="fileBytes">文件二进制内容</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public bool UploadFile(byte[] fileBytes, string remoteFileName, bool overWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("非法文件名！");
                }
                if (!overWriteRemoteFile && FileExist(remoteFileName))
                {
                    throw new Exception("FTP服务上面已经存在同名文件！");
                }
                Response = Open(new Uri(Uri+ remoteFileName), WebRequestMethods.Ftp.UploadFile);
                Stream requestStream = Request.GetRequestStream();
                MemoryStream mem = new MemoryStream(fileBytes);

                byte[] buffer = new byte[1024];
                while (true)
                {
                    var bytesRead = mem.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Close();
                Response = (FtpWebResponse)Request.GetResponse();
                mem.Close();
                mem.Dispose();
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        #region 异步上传文件
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件名</param>
        public void UploadFileAsync(string localFullPath)
        {
            UploadFileAsync(localFullPath, Path.GetFileName(localFullPath), false);
        }
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public void UploadFileAsync(string localFullPath, bool overWriteRemoteFile)
        {
            UploadFileAsync(localFullPath, Path.GetFileName(localFullPath), overWriteRemoteFile);
        }
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        public void UploadFileAsync(string localFullPath, string remoteFileName)
        {
            UploadFileAsync(localFullPath, remoteFileName, false);
        }
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件名</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public void UploadFileAsync(string localFullPath, string remoteFileName, bool overWriteRemoteFile)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName) || !IsValidFileChars(Path.GetFileName(localFullPath)) || !IsValidPathChars(Path.GetDirectoryName(localFullPath)))
                {
                    throw new Exception("非法文件名或目录名!");
                }
                if (!overWriteRemoteFile && FileExist(remoteFileName))
                {
                    throw new Exception("FTP服务上面已经存在同名文件！");
                }
                if (File.Exists(localFullPath))
                {
                    MyWebClient client = new MyWebClient();

                    client.UploadProgressChanged += client_UploadProgressChanged;
                    client.UploadFileCompleted += client_UploadFileCompleted;
                    client.Credentials = new NetworkCredential(UserName, Password);
                    if (Proxy != null)
                    {
                        client.Proxy = Proxy;
                    }
                    client.UploadFileAsync(new Uri(Uri + remoteFileName), localFullPath);

                }
                else
                {
                    throw new Exception("本地文件不存在!");
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="fileBytes">上传的二进制数据</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        public void UploadFileAsync(byte[] fileBytes, string remoteFileName)
        {
            if (!IsValidFileChars(remoteFileName))
            {
                throw new Exception("非法文件名或目录名!");
            }
            UploadFileAsync(fileBytes, remoteFileName, false);
        }
        /// <summary>
        /// 异步上传文件到FTP服务器
        /// </summary>
        /// <param name="fileBytes">文件二进制内容</param>
        /// <param name="remoteFileName">要在FTP服务器上面保存文件名</param>
        /// <param name="overWriteRemoteFile">是否覆盖远程服务器上面同名的文件</param>
        public void UploadFileAsync(byte[] fileBytes, string remoteFileName, bool overWriteRemoteFile)
        {
            try
            {

                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("非法文件名！");
                }
                if (!overWriteRemoteFile && FileExist(remoteFileName))
                {
                    throw new Exception("FTP服务上面已经存在同名文件！");
                }
                string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                if (!tempPath.EndsWith("\\"))
                {
                    tempPath += "\\";
                }
                string tempFile = tempPath + Path.GetRandomFileName();
                tempFile = Path.ChangeExtension(tempFile, Path.GetExtension(remoteFileName));
                FileStream stream = new FileStream(tempFile, FileMode.CreateNew, FileAccess.Write);
                stream.Write(fileBytes, 0, fileBytes.Length);   //注意，因为Int32的最大限制，最大上传文件只能是大约2G多一点
                stream.Flush();
                stream.Close();
                stream.Dispose();
                IsDeleteTempFile = true;
                UploadTempFile = tempFile;
                UploadFileAsync(tempFile, remoteFileName, overWriteRemoteFile);



            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }

        /// <summary>
        /// 异步上传文件完成之后触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">数据信息对象</param>
        void client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (IsDeleteTempFile)
            {
                if (File.Exists(UploadTempFile))
                {
                    File.SetAttributes(UploadTempFile, FileAttributes.Normal);
                    File.Delete(UploadTempFile);
                }
                IsDeleteTempFile = false;
            }
            UploadFileCompleted?.Invoke(sender, e);
        }

        /// <summary>
        /// 异步上传进度发生改变触发的事件
        /// </summary>
        /// <param name="sender">下载对象</param>
        /// <param name="e">进度信息对象</param>
        void client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (UploadProgressChanged != null)
            {
                UploadProgressChanged(sender, e);
            }
        }
        #endregion
      


        #region 列出目录文件信息
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件和目录
        /// </summary>
        public FileStruct[] ListFilesAndDirectories()
        {
            Response = Open(Uri, WebRequestMethods.Ftp.ListDirectoryDetails);
            if (Response != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                StreamReader stream = new StreamReader(Response.GetResponseStream(), Encoding.Default);
                string datastring = stream.ReadToEnd();
                FileStruct[] list = GetList(datastring);
                return list;
            }
            else
            {
                throw new NullReferenceException("Response");
            }
        }
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件
        /// </summary>
        public FileStruct[] ListFiles()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile.ToArray();
        }

        /// <summary>
        /// 列出FTP服务器上面当前目录的所有的目录
        /// </summary>
        public FileStruct[] ListDirectories()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory.ToArray();
        }
        /************************************************************************存在判断、重命名、删除文件、创建文件、移动、**************************************************************************/
        #region 目录或文件存在的判断
        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="remoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string remoteDirectoryName)
        {
            try
            {
                if (!IsValidPathChars(remoteDirectoryName))
                {
                    throw new Exception("目录名非法！");
                }
                FileStruct[] listDir = ListDirectories();
                foreach (FileStruct dir in listDir)
                {
                    if (dir.Name == remoteDirectoryName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 判断一个远程文件是否存在服务器当前目录下面
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        public bool FileExist(string remoteFileName)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("文件名非法！");
                }
                FileStruct[] listFile = ListFiles();
                foreach (FileStruct file in listFile)
                {
                    if (file.Name == remoteFileName)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        #region 删除文件
        /// <summary>
        /// 从FTP服务器上面删除一个文件
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        public void DeleteFile(string remoteFileName)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName))
                {
                    throw new Exception("文件名非法！");
                }
                Response = Open(new Uri(Uri + remoteFileName), WebRequestMethods.Ftp.DeleteFile);
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        #region 重命名文件
        /// <summary>
        /// 更改一个文件的名称或一个目录的名称
        /// </summary>
        /// <param name="remoteFileName">原始文件或目录名称</param>
        /// <param name="newFileName">新的文件或目录的名称</param>
        public bool ReName(string remoteFileName, string newFileName)
        {
            try
            {
                if (!IsValidFileChars(remoteFileName) || !IsValidFileChars(newFileName))
                {
                    throw new Exception("文件名非法！");
                }
                if (remoteFileName == newFileName)
                {
                    return true;
                }
                if (FileExist(remoteFileName))
                {
                    Request = OpenRequest(new Uri(Uri + remoteFileName), WebRequestMethods.Ftp.Rename);
                    Request.RenameTo = newFileName;
                    Response = (FtpWebResponse)Request.GetResponse();

                }
                else
                {
                    throw new Exception("文件在服务器上不存在！");
                }
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        /// <summary>    
        /// 获取指定文件大小    
        /// </summary>    
        public long GetFileSize(string filename)
        {
            try
            {
                if (!IsValidFileChars(filename))
                {
                    throw new Exception("文件名非法！");
                }
                long size;
                if (FileExist(filename))
                {
                    Request = OpenRequest(new Uri(Uri + filename), WebRequestMethods.Ftp.GetFileSize);
                    Response = (FtpWebResponse)Request.GetResponse();
                    // ReSharper disable once UnusedVariable
                    using (Stream stream = Response.GetResponseStream())
                    {
                        size = Response.ContentLength;
                    }
                }
                else
                {
                    throw new Exception("文件在服务器上不存在！");
                }
                if (size == -1)
                {
                    throw new Exception("获取大小异常！");
                }
                return size;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #region 拷贝、移动文件
        /// <summary>
        /// 把当前目录下面的一个文件拷贝到服务器上面另外的目录中，注意，拷贝文件之后，当前工作目录还是文件原来所在的目录
        /// </summary>
        /// <param name="remoteFile">当前目录下的文件名</param>
        /// <param name="directoryName">新目录名称。
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        /// <returns></returns>
        public bool CopyFileToAnotherDirectory(string remoteFile, string directoryName)
        {
            string currentWorkDir = DirectoryPath;
            try
            {
                byte[] bt = DownloadFile(remoteFile);
                GotoDirectory(directoryName);
                bool success = UploadFile(bt, remoteFile, false);
                DirectoryPath = currentWorkDir;
                return success;
            }
            catch (Exception ep)
            {
                DirectoryPath = currentWorkDir;
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 把当前目录下面的一个文件移动到服务器上面另外的目录中，注意，移动文件之后，当前工作目录还是文件原来所在的目录
        /// </summary>
        /// <param name="remoteFile">当前目录下的文件名</param>
        /// <param name="directoryName">新目录名称。
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ；
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        /// <returns></returns>
        public bool MoveFileToAnotherDirectory(string remoteFile, string directoryName)
        {
            string currentWorkDir = DirectoryPath;
            try
            {
                if (directoryName == "")
                    return false;
                if (!directoryName.StartsWith("/"))
                    directoryName = "/" + directoryName;
                if (!directoryName.EndsWith("/"))
                    directoryName += "/";
                bool success = ReName(remoteFile, directoryName + remoteFile);
                DirectoryPath = currentWorkDir;
                return success;
            }
            catch (Exception ep)
            {
                DirectoryPath = currentWorkDir;
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        #region 建立、删除子目录
        /// <summary>
        /// 在FTP服务器上当前工作目录建立一个子目录
        /// </summary>
        /// <param name="directoryName">子目录名称</param>
        public bool MakeDirectory(string directoryName)
        {
            try
            {
                if (!IsValidPathChars(directoryName))
                {
                    throw new Exception("目录名非法！");
                }
                if (DirectoryExist(directoryName))
                {
                    throw new Exception("服务器上面已经存在同名的文件名或目录名！");
                }
                Response = Open(new Uri(Uri + directoryName), WebRequestMethods.Ftp.MakeDirectory);
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 从当前工作目录中删除一个子目录
        /// </summary>
        /// <param name="directoryName">子目录名称</param>
        public bool RemoveDirectory(string directoryName)
        {
            try
            {
                if (!IsValidPathChars(directoryName))
                {
                    throw new Exception("目录名非法！");
                }
                if (!DirectoryExist(directoryName))
                {
                    throw new Exception("服务器上面不存在指定的文件名或目录名！");
                }
                Response = Open(new Uri(Uri + directoryName), WebRequestMethods.Ftp.RemoveDirectory);
                return true;
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        #endregion
        /************************************************************************目录切换**************************************************************************/
        #region 目录切换操作
        /// <summary>
        /// 进入一个目录
        /// </summary>
        /// <param name="directoryName">
        /// 新目录的名字。 
        /// 说明：如果新目录是当前目录的子目录，则直接指定子目录。如: SubDirectory1/SubDirectory2 ； 
        /// 如果新目录不是当前目录的子目录，则必须从根目录一级一级的指定。如： ./NewDirectory/SubDirectory1/SubDirectory2
        /// </param>
        public bool GotoDirectory(string directoryName)
        {
            string currentWorkPath = DirectoryPath;
            try
            {
                directoryName = directoryName.Replace("\\", "/");
                string[] directoryNames = directoryName.Split(new char[] { '/' });
                if (directoryNames[0] == ".")
                {
                    DirectoryPath = "/";
                    if (directoryNames.Length == 1)
                    {
                        return true;
                    }
                    Array.Clear(directoryNames, 0, 1);
                }
                bool success = false;
                foreach (string dir in directoryNames)
                {
                    if (dir != null)
                    {
                        success = EnterOneSubDirectory(dir);
                        if (!success)
                        {
                            DirectoryPath = currentWorkPath;
                            return false;
                        }
                    }
                }
                return success;

            }
            catch (Exception ep)
            {
                DirectoryPath = currentWorkPath;
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 从当前工作目录进入一个子目录
        /// </summary>
        /// <param name="directoryName">子目录名称</param>
        private bool EnterOneSubDirectory(string directoryName)
        {
            try
            {
                if (directoryName.IndexOf("/", StringComparison.Ordinal) >= 0 || !IsValidPathChars(directoryName))
                {
                    throw new Exception("目录名非法!");
                }
                if (directoryName.Length > 0 && DirectoryExist(directoryName))
                {
                    if (!directoryName.EndsWith("/"))
                    {
                        directoryName += "/";
                    }
                    DirectoryPath += directoryName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ep)
            {
                ErrorMsg = ep.ToString();
                throw;
            }
        }
        /// <summary>
        /// 从当前工作目录往上一级目录
        /// </summary>
        public bool ComeOutDirectory()
        {
            if (DirectoryPath == "/")
            {
                ErrorMsg = "当前目录已经是根目录！";
                throw new Exception("当前目录已经是根目录！");
            }
            char[] sp = new char[] { '/' };

            string[] strDir = DirectoryPath.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            if (strDir.Length == 1)
            {
                DirectoryPath = "/";
            }
            else
            {
                DirectoryPath = String.Join("/", strDir, 0, strDir.Length - 1);
            }
            return true;

        }


        #endregion

        /************************************************************************文件名和目录名有效性判断**************************************************************************/

        #region 文件、目录名称有效性判断
        /// <summary>
        /// 判断目录名中字符是否合法
        /// </summary>
        /// <param name="directoryName">目录名称</param>
        public bool IsValidPathChars(string directoryName)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            char[] dirChar = directoryName.ToCharArray();
            foreach (char c in dirChar)
            {
                if (Array.BinarySearch(invalidPathChars, c) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断文件名中字符是否合法
        /// </summary>
        /// <param name="fileName">文件名称</param>
        public bool IsValidFileChars(string fileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            char[] nameChar = fileName.ToCharArray();
            foreach (char c in nameChar)
            {
                if (Array.BinarySearch(invalidFileChars, c) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        /************************************************************************内部私有方法**************************************************************************/


        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private FileStruct[] GetList(string datastring)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            FileListStyleType directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (directoryListStyle != FileListStyleType.Unknown && s != "")
                {
                    FileStruct f = new FileStruct();
                    f.Name = "..";
                    switch (directoryListStyle)
                    {
                        case FileListStyleType.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break;
                        case FileListStyleType.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s);
                            break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray.ToArray();
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="record">文件信息</param>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string record)
        {
            FileStruct f = new FileStruct();
            string processstr = record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDtfi = new CultureInfo("en-US", false).DateTimeFormat;
            myDtfi.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDtfi);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }


        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="recordList">文件信息列表</param>
        private FileListStyleType GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10
                 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyleType.UnixStyle;
                }
                else if (s.Length > 8
                 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyleType.WindowsStyle;
                }
            }
            return FileListStyleType.Unknown;
        }

        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string record)
        {
            FileStruct f = new FileStruct();
            string processstr = record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":", StringComparison.Ordinal) >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            return f;
        }

        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion
        #region 重载WebClient，支持FTP进度
        internal class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                FtpWebRequest req = (FtpWebRequest)base.GetWebRequest(address);
                if (req != null)
                {
                    req.UsePassive = false;
                    return req;
                }
                else
                {
                     throw  new Exception("req is null");
                }
            }
        }
        #endregion
    }
}
