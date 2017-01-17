using System;
using System.IO;
using System.Security.Cryptography;
namespace CSharpHelperLib.EncryptAndDecode
{
  public  static class Decode 
    {
        private static readonly byte[] KeyBytes = new byte[] { 42, 16, 93, 156, 78, 4, 218, 32 };
        private static readonly byte[] IvBytes = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3 };
        public static string DecodeDes2String(this string data, byte[] key, byte[] iv)
        {
            string result;
            byte[] buffer = Convert.FromBase64String(data);
            DES des = DES.Create();
            ICryptoTransform transform = des.CreateDecryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    StreamReader streamReader = new StreamReader(cryptoStream);
                    result = streamReader.ReadLine();
                }
            }
            return result;
        }
        public static string DecodeDes2String( string data)
        {
            return DecodeDes2String(data,KeyBytes,IvBytes);
        }
    }

}
