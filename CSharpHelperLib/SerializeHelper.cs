using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

namespace CSharpHelper
{
    public class SerializeHelper
    {
        public static bool Serialize<T>(T model, SerializeType type, string strUrl) where T : class, new()
        {
            bool result;
            try
            {
                using (FileStream fileStream = new FileStream(strUrl, FileMode.Create))
                {
                    switch (type)
                    {
                        case SerializeType.BinaryFormatter:
                            {
                                BinaryFormatter binaryFormatter = new BinaryFormatter();
                                binaryFormatter.Serialize(fileStream, model);
                                break;
                            }
                        case SerializeType.Soap:
                            {
                                SoapFormatter soapFormatter = new SoapFormatter();
                                soapFormatter.Serialize(fileStream, model);
                                break;
                            }
                        case SerializeType.Xml:
                            {
                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                                xmlSerializer.Serialize(fileStream, model);
                                break;
                            }
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string Serialize<T>(object t)
        {
            string result;
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, t);
                result = stringWriter.ToString();
            }
            return result;
        }

        public static T DeSerialize<T>(SerializeType type, string strUrl) where T : class, new()
        {
            T result;
            using (FileStream fileStream = new FileStream(strUrl, FileMode.Open))
            {
                T t = Activator.CreateInstance<T>();
                switch (type)
                {
                    case SerializeType.BinaryFormatter:
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            t = (T)((object)binaryFormatter.Deserialize(fileStream));
                            break;
                        }
                    case SerializeType.Soap:
                        {
                            SoapFormatter soapFormatter = new SoapFormatter();
                            t = (T)((object)soapFormatter.Deserialize(fileStream));
                            break;
                        }
                    case SerializeType.Xml:
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                            t = (T)((object)xmlSerializer.Deserialize(fileStream));
                            break;
                        }
                }
                result = t;
            }
            return result;
        }

        public static object Deserialize<T>(string xml) where T : class, new()
        {
            object result;
            using (StringReader stringReader = new StringReader(xml))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                result = xmlSerializer.Deserialize(stringReader);
            }
            return result;
        }
    }
}


namespace CSharpHelper
{
    public enum SerializeType
    {
        BinaryFormatter,
        Soap,
        Xml
    }
}

