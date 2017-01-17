using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CSharpHelper
{
    public static class HttpClientHelper
    {
        public static string Response(string url, string strContentType = "application/json")
        {
            if (url.StartsWith("https"))
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorWriteLog(ex.Message);
                }
            }
            HttpResponseMessage result = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue(strContentType)
                    }
                }
            }.GetAsync(url).Result;
            string result3;
            if (result.IsSuccessStatusCode)
            {
                string result2 = result.Content.ReadAsStringAsync().Result;
                result3 = result2;
            }
            else
            {
                result3 = "";
            }
            return result3;
        }

        public static string Response(string url, string postData, string strContentType = "application/json")
        {
            if (url.StartsWith("https"))
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorWriteLog(ex.Message);
                }
            }
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(strContentType);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = httpClient.PostAsync(url, httpContent).Result;
            string result3;
            if (result.IsSuccessStatusCode)
            {
                string result2 = result.Content.ReadAsStringAsync().Result;
                result3 = result2;
            }
            else
            {
                result3 = "";
            }
            return result3;
        }
    }
}
