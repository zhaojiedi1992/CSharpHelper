using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System
{
   public static class ByteArrayExt
    {
        /// <summary>
        /// 扩展byte[]方法，支持byte[]转string
        /// </summary>
        /// <param name="bytes">输入byte[[]</param>
        /// <returns></returns>
        public static string ToStringFromBytes (this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }
        /// <summary>
        /// 扩展byte[]方法，支持byte[]转string
        /// </summary>
        /// <param name="bytes">输入byte[]</param>
        /// <param name="encoding">指定编码</param>
        /// <returns></returns>
        public static string ToStringFromBytes(this byte[] bytes, Encoding encoding)
        {
            return Encoding.Default.GetString(bytes);
        }
    }
}
