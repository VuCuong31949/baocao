using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace QLTapChi.Models
{
    public static class CookieHelper
    {
        public static void create(string name, string value, DateTime expires)
        {

            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = HttpUtility.UrlEncode(value);
            //cookie.Value = value;
            cookie.Expires = expires;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public static string GetCookie(string name)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];

            if (cookie != null)
            {
                return HttpUtility.UrlDecode(cookie.Value);
            }
            else
            {
                return "";
            }
        }
        public static bool VerifySHA(string name, string value)
        {
            string hashedValue = Hashing.ToSHA256(value);
            string storedCookie = GetCookie(name);
            return hashedValue == storedCookie;
        }
        public static void delete(string key)
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                HttpCookie cookie = new HttpCookie(key);
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}