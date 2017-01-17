using Newtonsoft.Json;
using System;

namespace CSharpHelper
{
    public static class JsonHelper
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static object JsonResult(string url, string strContentType = "application/json")
        {
            string value = HttpClientHelper.Response(url, strContentType);
            return JsonConvert.DeserializeObject(value);
        }

        public static T JsonResult<T>(string url, string strContentType = "application/json")
        {
            string value = HttpClientHelper.Response(url, strContentType);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static object JsonResult(string url, string postData, string strContentType = "application/json")
        {
            string value = HttpClientHelper.Response(url, postData, strContentType);
            return JsonConvert.DeserializeObject(value);
        }

        public static T JsonResult<T>(string url, string postData, string strContentType = "application/json")
        {
            string value = HttpClientHelper.Response(url, postData, strContentType);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
