using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System
{
   public static class StringExt
    {
        /// <summary>
        /// 扩展string方法，支持string转byte[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytesFromString(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }
        /// <summary>
        /// 扩展string方法，支持string转byte[]
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] ToBytesFromString(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }
    }
}
