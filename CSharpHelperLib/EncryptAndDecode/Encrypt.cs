using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSharpHelperLib.EncryptAndDecode
{
  public  static class Encrypt
    {
        private static readonly byte[] KeyBytes = new byte[] { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static readonly byte[] IvBytes = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3 };
        public static byte[] Encrypt2Bytes( byte[] data, CryptographyType cryptographyType)
        {
            switch (cryptographyType)
            {
                case CryptographyType.MD5:return MD5.Create().ComputeHash(data);
                case CryptographyType.SHA1:return SHA1.Create().ComputeHash(data);
                case CryptographyType.SHA256: return SHA256.Create().ComputeHash(data);
                case CryptographyType.SHA384: return SHA384.Create().ComputeHash(data);
                case CryptographyType.SHA512: return SHA512.Create().ComputeHash(data);
                case CryptographyType.DES:return UseDesEncrypt2String(data).ToBytesFromString();
                default: throw new ArgumentOutOfRangeException();
            }
        }
        public static string Encrypt2String(byte[] data, CryptographyType cryptographyType)
        {
            switch (cryptographyType)
            {
                case CryptographyType.MD5: return MD5.Create().ComputeHash(data).ToStringFromBytes();
                case CryptographyType.SHA1: return SHA1.Create().ComputeHash(data).ToStringFromBytes();
                case CryptographyType.SHA256: return SHA256.Create().ComputeHash(data).ToStringFromBytes();
                case CryptographyType.SHA384: return SHA384.Create().ComputeHash(data).ToStringFromBytes();
                case CryptographyType.SHA512: return SHA512.Create().ComputeHash(data).ToStringFromBytes();
                case CryptographyType.DES: return UseDesEncrypt2String(data);
                default: throw new ArgumentOutOfRangeException();
            }
        }
        public static string UseDesEncrypt2String( string data, byte[] key, byte[] iv)
        {
            DES des = DES.Create();
            byte[] array = data.ToBytesFromString();
            ICryptoTransform transform = des.CreateEncryptor(key, iv);
            byte[] inArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                    }
                }
                inArray = memoryStream.ToArray();
            }
            des.Clear();
            return Convert.ToBase64String(inArray);
        }
        public static string UseDesEncrypt2String(byte[] data, byte[] key, byte[] iv)
        {
            return UseDesEncrypt2String(data.ToStringFromBytes(),key,iv);
        }
        public static string UseDesEncrypt2String(byte[] data)
        {
            return UseDesEncrypt2String(data.ToStringFromBytes(), KeyBytes, IvBytes);
        }
       

    }

}
