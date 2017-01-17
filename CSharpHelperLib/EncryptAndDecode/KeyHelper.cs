using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CSharpHelperLib.EncryptAndDecode
{
    public static class KeyHelper
    {
        public static byte[] MD5(this byte[] data)
        {
            return System.Security.Cryptography.MD5.Create().ComputeHash(data);
        }

        public static string MD5(this string strData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strData);
            return System.Security.Cryptography.MD5.Create().ComputeHash(bytes).ToStringByte("");
        }

        public static byte[] SHA1(this byte[] data)
        {
            return System.Security.Cryptography.SHA1.Create().ComputeHash(data);
        }

        public static string SHA1(this string strData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            return System.Security.Cryptography.SHA1.Create().ComputeHash(bytes).ToStringByte("");
        }

        public static byte[] SHA256(this byte[] data)
        {
            return System.Security.Cryptography.SHA256.Create().ComputeHash(data);
        }

        public static string SHA256(this string strData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            return System.Security.Cryptography.SHA256.Create().ComputeHash(bytes).ToStringByte("");
        }

        public static byte[] SHA384(this byte[] data)
        {
            return System.Security.Cryptography.SHA384.Create().ComputeHash(data);
        }

        public static string SHA384(this string strData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            return System.Security.Cryptography.SHA384.Create().ComputeHash(bytes).ToStringByte("");
        }

        public static byte[] SHA512(this byte[] data)
        {
            return System.Security.Cryptography.SHA512.Create().ComputeHash(data);
        }

        public static string SHA512(this string strData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            return System.Security.Cryptography.SHA512.Create().ComputeHash(bytes).ToStringByte("");
        }

        public static string EnDES(this string data, byte[] key, byte[] iv)
        {
            DES dES = DES.Create();
            byte[] array = data.ToByte();
            ICryptoTransform transform = dES.CreateEncryptor(key, iv);
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
            dES.Clear();
            return Convert.ToBase64String(inArray);
        }

        public static string DeDES(this string data, byte[] key, byte[] iv)
        {
            string result = string.Empty;
            byte[] buffer = Convert.FromBase64String(data);
            DES dES = DES.Create();
            ICryptoTransform transform = dES.CreateDecryptor(key, iv);
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

        public static string EnTripleDES(this string data, byte[] key, byte[] iv)
        {
            byte[] inArray = null;
            byte[] array = data.ToByte();
            TripleDES tripleDES = TripleDES.Create();
            ICryptoTransform transform = tripleDES.CreateEncryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(cryptoStream);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }
                inArray = memoryStream.ToArray();
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DeTripleDES(this string data, byte[] key, byte[] iv)
        {
            byte[] buffer = Convert.FromBase64String(data);
            string result = string.Empty;
            TripleDES tripleDES = TripleDES.Create();
            ICryptoTransform transform = tripleDES.CreateDecryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    StreamReader streamReader = new StreamReader(cryptoStream);
                    result = streamReader.ReadLine();
                }
            }
            tripleDES.Clear();
            return result;
        }

        public static string EnAES(this string data, byte[] key, byte[] iv)
        {
            Aes aes = Aes.Create();
            byte[] inArray = null;
            ICryptoTransform transform = aes.CreateEncryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(cryptoStream);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                inArray = memoryStream.ToArray();
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DeAES(this string data, byte[] key, byte[] iv)
        {
            string result = string.Empty;
            Aes aes = Aes.Create();
            ICryptoTransform transform = aes.CreateDecryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    StreamReader streamReader = new StreamReader(cryptoStream);
                    result = streamReader.ReadLine();
                    streamReader.Close();
                }
            }
            aes.Clear();
            return result;
        }

        public static string EnRijndael(this string data, byte[] key, byte[] iv)
        {
            Rijndael rijndael = Rijndael.Create();
            byte[] inArray = null;
            ICryptoTransform transform = rijndael.CreateEncryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(cryptoStream);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }
                inArray = memoryStream.ToArray();
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DeRijndael(this string data, byte[] key, byte[] iv)
        {
            string result = string.Empty;
            Rijndael rijndael = Rijndael.Create();
            ICryptoTransform transform = rijndael.CreateDecryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    StreamReader streamReader = new StreamReader(cryptoStream);
                    result = streamReader.ReadLine();
                    streamReader.Close();
                }
            }
            return result;
        }

        public static string EnRSA(this string data, string publickey)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.FromXmlString(publickey);
            byte[] inArray = rSACryptoServiceProvider.Encrypt(data.ToByte(), false);
            return Convert.ToBase64String(inArray);
        }

        public static string DeRSA(this string data, string publickey)
        {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
            rSACryptoServiceProvider.FromXmlString(publickey);
            byte[] bytes = rSACryptoServiceProvider.Decrypt(Convert.FromBase64String(data), false);
            return Encoding.Default.GetString(bytes);
        }

        public static string EnDSA(this string data, string publickey)
        {
            DSA dSA = DSA.Create();
            dSA.FromXmlString(publickey);
            SHA1 sHA = System.Security.Cryptography.SHA1.Create();
            byte[] inArray = dSA.CreateSignature(sHA.ComputeHash(Convert.FromBase64String(data)));
            return Convert.ToBase64String(inArray);
        }

        public static bool DeDSA(this string data, string privatekey, string originalData)
        {
            DSA dSA = DSA.Create();
            dSA.FromXmlString(privatekey);
            SHA1 sHA = System.Security.Cryptography.SHA1.Create();
            return dSA.VerifySignature(sHA.ComputeHash(Convert.FromBase64String(originalData)), Convert.FromBase64String(data));
        }

        public static string EnCDsa(this string data, CngKey key)
        {
            ECDsa eCDsa = new ECDsaCng(key);
            SHA1 sHA = System.Security.Cryptography.SHA1.Create();
            byte[] inArray = eCDsa.SignHash(sHA.ComputeHash(Convert.FromBase64String(data)));
            return Convert.ToBase64String(inArray);
        }

        public static bool DeCDsa(this string data, CngKey key, string originalData)
        {
            ECDsaCng eCDsaCng = new ECDsaCng(key);
            SHA1 sHA = System.Security.Cryptography.SHA1.Create();
            return eCDsaCng.VerifyHash(sHA.ComputeHash(Convert.FromBase64String(originalData)), Convert.FromBase64String(data));
        }

        private static string ToStringByte(this byte[] data, string strFg = "")
        {
            return string.Join(strFg, (from t in data
                                       select t.ToString()).ToArray<string>());
        }

        private static byte[] ToByte(this string data)
        {
            return Encoding.Default.GetBytes(data);
        }
    }
}
