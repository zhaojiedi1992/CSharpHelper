﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace CSharpHelperLib.Http
{
    /// <summary>  
    /// Http连接操作帮助类  
    /// </summary>  
    public class HttpHelper
    {
        #region 预定义变量  
        //默认的编码  
        public Encoding Encoding { get; private set; } = Encoding.Default;
        //Post数据编码  
        public Encoding PostEncoding { get; private set; } = Encoding.Default;
        //HttpWebRequest对象用来发起请求  
        public HttpWebRequest Request { get; private set; }
        //获取影响流的数据对象  
        public HttpWebResponse Response { get; private set; } 
        //设置本地的出口ip和端口  
        public IPEndPoint IpEndPoint { get; private set; }

        #endregion

        #region Public  

        /// <summary>  
        /// 根据相传入的数据，得到相应页面数据  
        /// </summary>  
        /// <param name="item">参数类对象</param>  
        /// <returns>返回HttpResult类型</returns>  
        public HttpResult GetHtml(HttpItem item)
        {
            //返回参数  
            HttpResult result = new HttpResult();
            try
            {
                //准备参数  
                SetRequest(item);
            }
            catch (Exception ex)
            {
                //配置参数时出错  
                return new HttpResult() { Cookie = string.Empty, Header = null, Html = ex.Message, StatusDescription = "配置参数时出错：" + ex.Message };
            }
            try
            {
                //请求数据  
                using (Response = (HttpWebResponse)Request.GetResponse())
                {
                    GetData(item, result);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (Response = (HttpWebResponse)ex.Response)
                    {
                        GetData(item, result);
                    }
                }
                else
                {
                    result.Html = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }
            if (item.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }
        #endregion

        #region GetData  

        /// <summary>  
        /// 获取数据的并解析的方法  
        /// </summary>  
        /// <param name="item"></param>  
        /// <param name="result"></param>  
        private void GetData(HttpItem item, HttpResult result)
        {
            if (Response == null)
            {
                return;
            }
            #region base  
            //获取StatusCode  
            result.StatusCode = Response.StatusCode;
            //获取StatusDescription  
            result.StatusDescription = Response.StatusDescription;
            //获取Headers  
            result.Header = Response.Headers;
            //获取最后访问的URl  
            result.ResponseUri = Response.ResponseUri.ToString();
            //获取CookieCollection  
            if (Response.Cookies != null) result.CookieCollection = Response.Cookies;
            //获取set-cookie  
            if (Response.Headers["set-cookie"] != null) result.Cookie = Response.Headers["set-cookie"];
            #endregion

            #region byte  
            //处理网页Byte  
            byte[] responseByte = GetByte();
            #endregion

            #region Html  
            if (responseByte != null && responseByte.Length > 0)
            {
                //设置编码  
                SetEncoding(item, result, responseByte);
                //得到返回的HTML  
                result.Html = Encoding.GetString(responseByte);
            }
            else
            {
                //没有返回任何Html代码  
                result.Html = string.Empty;
            }
            #endregion
        }
        /// <summary>  
        /// 设置编码  
        /// </summary>  
        /// <param name="item">HttpItem</param>  
        /// <param name="result">HttpResult</param>  
        /// <param name="responseByte">byte[]</param>  
        private void SetEncoding(HttpItem item, HttpResult result, byte[] responseByte)
        {
            //是否返回Byte类型数据  
            if (item.ResultType == ResultType.Byte) result.ResultByte = responseByte;
            //从这里开始我们要无视编码了  
            if (Encoding == null)
            {
                Match meta = Regex.Match(Encoding.Default.GetString(responseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                string c = string.Empty;
                if (meta.Groups.Count > 0)
                {
                    c = meta.Groups[1].Value.ToLower().Trim();
                }
                if (c.Length > 2)
                {
                    try
                    {
                        Encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
                    }
                    catch
                    {
                        if (string.IsNullOrEmpty(Response.CharacterSet))
                        {
                            Encoding = Encoding.UTF8;
                        }
                        else
                        {
                            Encoding = Encoding.GetEncoding(Response.CharacterSet);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Response.CharacterSet))
                    {
                        Encoding = Encoding.UTF8;
                    }
                    else
                    {
                        Encoding = Encoding.GetEncoding(Response.CharacterSet);
                    }
                }
            }
        }
        /// <summary>  
        /// 提取网页Byte  
        /// </summary>  
        /// <returns></returns>  
        private byte[] GetByte()
        {
            byte[] responseByte;
            using (MemoryStream stream = new MemoryStream())
            {
                //GZIIP处理  
                if (Response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    //开始读取流并设置编码方式  
                    Stream responseStream= Response.GetResponseStream();
                    if (responseStream != null)
                    {
                        new GZipStream(responseStream, CompressionMode.Decompress).CopyTo(stream, 10240);
                    }
                }
                else
                {
                    //开始读取流并设置编码方式  
                    var responseStream = Response.GetResponseStream();
                    responseStream?.CopyTo(stream, 10240);
                }
                //获取Byte  
                responseByte = stream.ToArray();
            }
            return responseByte;
        }


        #endregion

        #region SetRequest  

        /// <summary>  
        /// 为请求准备参数  
        /// </summary>  
        ///<param name="item">参数列表</param>  
        private void SetRequest(HttpItem item)
        {

            // 验证证书  
            SetCertificate(item);
            if (item.IpEndPoint != null)
            {
                IpEndPoint = item.IpEndPoint;
                //设置本地的出口ip和端口  
                Request.ServicePoint.BindIPEndPointDelegate = BindIpEndPointCallback;
            }
            //设置Header参数  
            if (item.Header != null && item.Header.Count > 0)
            {
                foreach (string key in item.Header.AllKeys)
                {
                    Request.Headers.Add(key, item.Header[key]);
                }
            }
            // 设置代理  
            SetProxy(item);
            if (item.ProtocolVersion != null) Request.ProtocolVersion = item.ProtocolVersion;
            Request.ServicePoint.Expect100Continue = item.Expect100Continue;
            //请求方式Get或者Post  
            Request.Method = item.Method;
            Request.Timeout = item.Timeout;
            Request.KeepAlive = item.KeepAlive;
            Request.ReadWriteTimeout = item.ReadWriteTimeout;
            if (!string.IsNullOrWhiteSpace(item.Host))
            {
                Request.Host = item.Host;
            }
            if (item.IfModifiedSince != null) Request.IfModifiedSince = Convert.ToDateTime(item.IfModifiedSince);
            //Accept  
            Request.Accept = item.Accept;
            //ContentType返回类型  
            Request.ContentType = item.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息  
            Request.UserAgent = item.UserAgent;
            // 编码  
            Encoding = item.Encoding;
            //设置安全凭证  
            Request.Credentials = item.Credentials;
            //设置Cookie  
            SetCookie(item);
            //来源地址  
            Request.Referer = item.Referer;
            //是否执行跳转功能  
            Request.AllowAutoRedirect = item.AllowAutoRedirect;
            if (item.MaximumAutomaticRedirections > 0)
            {
                Request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections;
            }
            //设置Post数据  
            SetPostData(item);
            //设置最大连接  
            if (item.Connectionlimit > 0) Request.ServicePoint.ConnectionLimit = item.Connectionlimit;
        }
        /// <summary>  
        /// 设置证书  
        /// </summary>  
        /// <param name="item"></param>  
        private void SetCertificate(HttpItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.CertificaterPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。  
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                //初始化对像，并设置请求的URL地址  
                Request = (HttpWebRequest)WebRequest.Create(item.Url);
                SetCertificateList(item);
                //将证书添加到请求里  
                Request.ClientCertificates.Add(new X509Certificate(item.CertificaterPath));
            }
            else
            {
                //初始化对像，并设置请求的URL地址  
                Request = (HttpWebRequest)WebRequest.Create(item.Url);
                SetCertificateList(item);
            }
        }
        /// <summary>  
        /// 设置多个证书  
        /// </summary>  
        /// <param name="item"></param>  
        private void SetCertificateList(HttpItem item)
        {
            if (item.ClentCertificates != null && item.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate c in item.ClentCertificates)
                {
                    Request.ClientCertificates.Add(c);
                }
            }
        }
        /// <summary>  
        /// 设置Cookie  
        /// </summary>  
        /// <param name="item">Http参数</param>  
        private void SetCookie(HttpItem item)
        {
            if (!string.IsNullOrEmpty(item.Cookie)) Request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
            //设置CookieCollection  
            if (item.ResultCookieType == ResultCookieType.CookieCollection)
            {
                Request.CookieContainer = new CookieContainer();
                if (item.CookieCollection != null && item.CookieCollection.Count > 0)
                    Request.CookieContainer.Add(item.CookieCollection);
            }
        }
        /// <summary>  
        /// 设置Post数据  
        /// </summary>  
        /// <param name="item">Http参数</param>  
        private void SetPostData(HttpItem item)
        {
            //验证在得到结果时是否有传入数据  
            if (!Request.Method.Trim().ToLower().Contains("get"))
            {
                if (item.PostEncoding != null)
                {
                    PostEncoding = item.PostEncoding;
                }
                byte[] buffer = null;
                //写入Byte类型  
                if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
                {
                    //验证在得到结果时是否有传入数据  
                    buffer = item.PostdataByte;
                }//写入文件  
                else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrWhiteSpace(item.Postdata))
                {
                    StreamReader r = new StreamReader(item.Postdata, PostEncoding);
                    buffer = PostEncoding.GetBytes(r.ReadToEnd());
                    r.Close();
                } //写入字符串  
                else if (!string.IsNullOrWhiteSpace(item.Postdata))
                {
                    buffer = PostEncoding.GetBytes(item.Postdata);
                }
                if (buffer != null)
                {
                    Request.ContentLength = buffer.Length;
                    Request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
                else
                {
                    Request.ContentLength = 0;
                }
            }
        }
        /// <summary>  
        /// 设置代理  
        /// </summary>  
        /// <param name="item">参数对象</param>  
        private void SetProxy(HttpItem item)
        {
            bool isIeProxy = false;
            if (!string.IsNullOrWhiteSpace(item.ProxyIp))
            {
                isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
            }
            if (!string.IsNullOrWhiteSpace(item.ProxyIp) && !isIeProxy)
            {
                //设置代理服务器  
                if (item.ProxyIp.Contains(":"))
                {
                    string[] plist = item.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()))
                    {
                        Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd)
                    };
                    //建议连接  
                    //给当前请求对象  
                    Request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(item.ProxyIp, false);
                    //建议连接  
                    myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
                    //给当前请求对象  
                    Request.Proxy = myProxy;
                }
            }
            else if (isIeProxy)
            {
                //设置为IE代理  
            }
            else
            {
                Request.Proxy = item.WebProxy;
            }
        }


        #endregion

        #region private main  
        /// <summary>  
        /// 回调验证证书问题  
        /// </summary>  
        /// <param name="sender">流对象</param>  
        /// <param name="certificate">证书</param>  
        /// <param name="chain">X509Chain</param>  
        /// <param name="errors">SslPolicyErrors</param>  
        /// <returns>bool</returns>  
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }

        /// <summary>  
        /// 通过设置这个属性，可以在发出连接的时候绑定客户端发出连接所使用的IP地址。   
        /// </summary>  
        /// <param name="servicePoint"></param>  
        /// <param name="remoteEndPoint"></param>  
        /// <param name="retryCount"></param>  
        /// <returns></returns>  
        private IPEndPoint BindIpEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
        {
            return IpEndPoint;//端口号  
        }
        #endregion
    }

    #region public calss  
    /// <summary>  
    /// Http请求参考类  
    /// </summary>  
    public class HttpItem
    {
        /// <summary>  
        /// 请求URL必须填写  
        /// </summary>  
        public string Url { get; set; }

        /// <summary>  
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值  
        /// </summary>  
        public string Method { get; set; } = "GET";

        /// <summary>  
        /// 默认请求超时时间  
        /// </summary>  
        public int Timeout { get; set; } = 100000;

        /// <summary>  
        /// 默认写入Post数据超时间  
        /// </summary>  
        public int ReadWriteTimeout { get; set; } = 30000;

        /// <summary>  
        /// 设置Host的标头信息  
        /// </summary>  
        public string Host { get; set; }

        /// <summary>  
        ///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。  
        /// </summary>  
        public bool KeepAlive { get; set; } = true;

        /// <summary>  
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*  
        /// </summary>  
        public string Accept { get; set; } = "text/html, application/xhtml+xml, */*";

        /// <summary>  
        /// 请求返回类型默认 text/html  
        /// </summary>  
        public string ContentType { get; set; } = "text/html";

        /// <summary>  
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)  
        /// </summary>  
        public string UserAgent { get; set; } = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";

        /// <summary>  
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312  
        /// </summary>  
        public Encoding Encoding { get; set; }

        /// <summary>  
        /// Post的数据类型  
        /// </summary>  
        public PostDataType PostDataType { get; set; } = PostDataType.String;

        /// <summary>  
        /// Post请求时要发送的字符串Post数据  
        /// </summary>  
        public string Postdata { get; set; }
        /// <summary>  
        /// Post请求时要发送的Byte类型的Post数据  
        /// </summary>  
        public byte[] PostdataByte { get; set; }
        /// <summary>  
        /// Cookie对象集合  
        /// </summary>  
        public CookieCollection CookieCollection { get; set; }
        /// <summary>  
        /// 请求时的Cookie  
        /// </summary>  
        public string Cookie { get; set; }
        /// <summary>  
        /// 来源地址，上次访问地址  
        /// </summary>  
        public string Referer { get; set; }
        /// <summary>  
        /// 证书绝对路径  
        /// </summary>  
        public string CertificaterPath { get; set; }
        /// <summary>  
        /// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp  
        /// </summary>  
        public WebProxy WebProxy { get; set; }

        /// <summary>  
        /// 是否设置为全文小写，默认为不转化  
        /// </summary>  
        public Boolean IsToLower { get; set; } = false;

        /// <summary>  
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转  
        /// </summary>  
        public Boolean AllowAutoRedirect { get; set; } = false;

        /// <summary>  
        /// 最大连接数  
        /// </summary>  
        public int Connectionlimit { get; set; } = 1024;

        /// <summary>  
        /// 代理Proxy 服务器用户名  
        /// </summary>  
        public string ProxyUserName { get; set; }
        /// <summary>  
        /// 代理 服务器密码  
        /// </summary>  
        public string ProxyPwd { get; set; }
        /// <summary>  
        /// 代理 服务IP,如果要使用IE代理就设置为ieproxy  
        /// </summary>  
        public string ProxyIp { get; set; }

        /// <summary>  
        /// 设置返回类型String和Byte  
        /// </summary>  
        public ResultType ResultType { get; set; } = ResultType.String;

        /// <summary>  
        /// header对象  
        /// </summary>  
        public WebHeaderCollection Header { get; set; } = new WebHeaderCollection();

        /// <summary>  
        //     获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。  
        /// </summary>  
        public Version ProtocolVersion { get; set; }

        /// <summary>  
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。  
        /// </summary>  
        public Boolean Expect100Continue { get; set; } = false;

        /// <summary>  
        /// 设置509证书集合  
        /// </summary>  
        public X509CertificateCollection ClentCertificates { get; set; }
        /// <summary>  
        /// 设置或获取Post参数编码,默认的为Default编码  
        /// </summary>  
        public Encoding PostEncoding { get; set; }

        /// <summary>  
        /// Cookie返回类型,默认的是只返回字符串类型  
        /// </summary>  
        public ResultCookieType ResultCookieType { get; set; } = ResultCookieType.String;

        /// <summary>  
        /// 获取或设置请求的身份验证信息。  
        /// </summary>  
        public ICredentials Credentials { get; set; } = CredentialCache.DefaultCredentials;

        /// <summary>  
        /// 设置请求将跟随的重定向的最大数目  
        /// </summary>  
        public int MaximumAutomaticRedirections { get; set; }

        /// <summary>  
        /// 获取和设置IfModifiedSince，默认为当前日期和时间  
        /// </summary>  
        public DateTime? IfModifiedSince { get; set; } = null;

        #region ip-port  

        /// <summary>  
        /// 设置本地的出口ip和端口  
        /// </summary>]  
        /// <example>  
        ///item.IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"),80);  
        /// </example>  
        public IPEndPoint IpEndPoint { get; set; } = null;

        #endregion
    }
    /// <summary>  
    /// Http返回参数类  
    /// </summary>  
    public class HttpResult
    {
        /// <summary>  
        /// Http请求返回的Cookie  
        /// </summary>  
        public string Cookie { get; set; }
        /// <summary>  
        /// Cookie对象集合  
        /// </summary>  
        public CookieCollection CookieCollection { get; set; }

        /// <summary>  
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空  
        /// </summary>  
        public string Html { get; set; } = string.Empty;

        /// <summary>  
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空  
        /// </summary>  
        public byte[] ResultByte { get; set; }
        /// <summary>  
        /// header对象  
        /// </summary>  
        public WebHeaderCollection Header { get; set; }
        /// <summary>  
        /// 返回状态说明  
        /// </summary>  
        public string StatusDescription { get; set; }
        /// <summary>  
        /// 返回状态码,默认为OK  
        /// </summary>  
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>  
        /// 最后访问的URl  
        /// </summary>  
        public string ResponseUri { get; set; }
        /// <summary>  
        /// 获取重定向的URl  
        /// </summary>  
        public string RedirectUrl
        {
            get
            {
                try
                {
                    if (Header != null && Header.Count > 0)
                    {
                        if (Header.AllKeys.Any(k => k.ToLower().Contains("location")))
                        {
                            string baseurl = Header["location"].Trim();
                            string locationurl = baseurl.ToLower();
                            if (!string.IsNullOrWhiteSpace(locationurl))
                            {
                                bool b = locationurl.StartsWith("http://") || locationurl.StartsWith("https://");
                                if (!b)
                                {
                                    baseurl = new Uri(new Uri(ResponseUri), baseurl).AbsoluteUri;
                                }
                            }
                            return baseurl;
                        }
                    }
                }
                catch
                {
                    // ignored
                }
                return string.Empty;
            }
        }
    }
    /// <summary>  
    /// 返回类型  
    /// </summary>  
    public enum ResultType
    {
        /// <summary>  
        /// 表示只返回字符串 只有Html有数据  
        /// </summary>  
        String,
        /// <summary>  
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回  
        /// </summary>  
        Byte
    }
    /// <summary>  
    /// Post的数据格式默认为string  
    /// </summary>  
    public enum PostDataType
    {
        /// <summary>  
        /// 字符串类型，这时编码Encoding可不设置  
        /// </summary>  
        String,
        /// <summary>  
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空  
        /// </summary>  
        Byte,
        /// <summary>  
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值  
        /// </summary>  
        FilePath
    }
    /// <summary>  
    /// Cookie返回类型  
    /// </summary>  
    public enum ResultCookieType
    {
        /// <summary>  
        /// 只返回字符串类型的Cookie  
        /// </summary>  
        String,
        /// <summary>  
        /// CookieCollection格式的Cookie集合同时也返回String类型的cookie  
        /// </summary>  
        CookieCollection
    }
    #endregion
}
