using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CSharpHelperLib.Http
{
    public static class HttpCookieHelper
    {
        /// <summary>  
        /// 根据字符生成Cookie列表  
        /// </summary>  
        /// <param name="cookie">Cookie字符串</param>  
        /// <returns></returns>  
        public static List<KeyValuePair<string, string>> GetCookieListFromString(string cookie)
        {
            List<KeyValuePair<string, string>> cookieList = new List<KeyValuePair<string, string>>();
            foreach (string item in cookie.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Regex.IsMatch(item, @"([\s\S]*?)=([\s\S]*?)$"))
                {
                    Match match = Regex.Match(item, @"([\s\S]*?)=([\s\S]*?)$");
                    cookieList.Add(new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[1].Value));
                }
            }
            return cookieList;
        }
        public static string GetCookieStringFromList(List<KeyValuePair<string, string>> cookieList)
        {
            return string.Join(",", cookieList.Select(r => $"{r.Key}={r.Value}"));
        }
        /// <summary>  
        /// 根据Key值得到Cookie值,Key不区分大小写  
        /// </summary>  
        /// <param name="key">key</param>  
        /// <param name="cookie">字符串Cookie</param>  
        /// <returns></returns>  
        public static string GetcookieValue(string key, List<KeyValuePair<string, string>> cookieList)
        {

            foreach (KeyValuePair<string, string> item in cookieList)
            {
                if (item.Key == key)
                {
                    return item.Value;
                }
            }
            return null;
        }
        public static string GetcookieValue(string key, string cookie)
        {
            List<KeyValuePair<string, string>> cookieList = GetCookieListFromString(cookie);
            return GetcookieValue(key, cookieList);
        }
        /// <summary>  
        /// 格式化Cookie为标准格式  
        /// </summary>  
        /// <param name="key">Key值</param>  
        /// <param name="value">Value值</param>  
        /// <returns></returns>  
        public static string FormatCookie(string key, string value)
        {
            return $"{key}={value};";
        }


        /// <summary>  
        /// 清除指定Cookie  
        /// </summary>  
        /// <param name="cookieName">cookieName</param>  
        public static void ClearCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-3);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        /// <summary>  
        /// 获取指定Cookie值  
        /// </summary>  
        /// <param name="cookieName">cookieName</param>  
        /// <returns></returns>  
        public static string GetcookieValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            string str = string.Empty;
            if (cookie != null)
            {
                str = cookie.Value;
            }
            return str;
        }

        /// <summary>  
        /// 添加一个Cookie（24小时过期）  
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>  
        public static void SetCookie(string cookieName, string cookieValue)
        {
            SetCookie(cookieName, cookieValue, DateTime.Now.AddDays(1.0),null);
        }
        /// <summary>  
        /// 添加一个Cookie  
        /// </summary>  
        /// <param name="cookieName">cookie名</param>  
        /// <param name="cookieValue">cookie值</param>  
        /// <param name="expires">过期时间 DateTime</param>  
        public static void SetCookie(string cookieName, string cookieValue, DateTime expires)
        {
           SetCookie(cookieName,cookieValue,expires,null);
        }

        /// <summary>  
        /// 添加一个Cookie  
        /// </summary>  
        /// <param name="cookieName">cookie名</param>  
        /// <param name="cookieValue">cookie值</param>  
        /// <param name="expires">过期时间 DateTime</param>
        /// <param name="domain"></param>  
        public static void SetCookie(string cookieName, string cookieValue, DateTime expires,string domain)
        {
            HttpCookie cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue,
                Expires = expires,
                Domain = domain
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }

}