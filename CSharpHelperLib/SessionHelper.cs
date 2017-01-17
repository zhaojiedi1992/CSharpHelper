using System;
using System.Collections.Generic;
using System.Web;

namespace CSharpHelper
{
    public class SessionHelper
    {
        public void SetSession(Dictionary<string, string> Values, int iExpires = 60)
        {
            foreach (string current in Values.Keys)
            {
                HttpContext.Current.Session[current] = Values[current];
            }
            HttpContext.Current.Session.Timeout = iExpires;
        }

        public string GetSession(string name)
        {
            string result;
            if (HttpContext.Current.Session[name] == null)
            {
                result = "";
            }
            else
            {
                result = HttpContext.Current.Session[name].ToString();
            }
            return result;
        }

        public void ClearSession(string name)
        {
            HttpContext.Current.Session[name] = null;
        }

        public bool IsCreate(string name)
        {
            return HttpContext.Current.Session[name] != null;
        }
    }
}
