using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace uCheck
{
    class WebRequest_Wrapper
    {
        public static CookieContainer Cookies = new CookieContainer();

        #region " UserAgent."
        // Default
        private static string _UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
        public static string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        #endregion

        #region " Method."
        // Default
        private static string _Method = "GET";
        public static string Method
        {
            get { return _Method; }
            set { _Method = value.ToUpperInvariant(); }
        }
        #endregion

        #region " AllowAutoRedirect."
        // Default
        private static bool _AllowAutoRedirect = true;
        public static bool AllowAutoRedirect
        {
            get { return _AllowAutoRedirect; }
            set { _AllowAutoRedirect = value; }
        }
        #endregion

        #region " KeepAlive."
        // Default
        private static bool _KeepAlive = true;
        public static bool KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        #endregion

        // Modified version of my WebRequest Wrapper        
        public static object Request(checkConfig.checkConf Config, String Name)
        {
            String Host = Config.URL.Replace("%user%", Name);
            String Referer = Config.URL;
            String POSTData = null;
            String SuccessString = null;
            String FailString = null;
            if (Config.POSTData == null)
            {
                _Method = "GET";
            }
            else
            {
                _Method = "POST";
                POSTData = Config.POSTData.Replace("%user%", Name);
            }
            if(Config.SuccessString != null)
            {
                SuccessString = Config.SuccessString.Replace("%user%", Name);
            }
            if (Config.FailString != null)
            {
                FailString = Config.FailString.Replace("%user%", Name);
            }
            
            try
            {
                HttpWebRequest WebR = (HttpWebRequest)WebRequest.Create(Host);

                WebR.Method = _Method;
                WebR.CookieContainer = Cookies;
                WebR.AllowAutoRedirect = _AllowAutoRedirect;
                WebR.KeepAlive = _KeepAlive;
                WebR.UserAgent = _UserAgent;
                WebR.ContentType = "application/x-www-form-urlencoded";
                WebR.Referer = Referer;

                if ((_Method == "POST"))
                {
                    byte[] _PostData = null;
                    _PostData = System.Text.Encoding.Default.GetBytes(POSTData);
                    WebR.ContentLength = _PostData.Length;

                    System.IO.Stream StreamWriter = WebR.GetRequestStream();
                    StreamWriter.Write(_PostData, 0, POSTData.Length);
                    StreamWriter.Dispose();
                }

                HttpWebResponse WebResponse;
                string PageHTML; 

                try
                {
                   WebResponse =  (HttpWebResponse)WebR.GetResponse();
                   Cookies.Add(WebResponse.Cookies);
                   System.IO.StreamReader StreamReader = new System.IO.StreamReader(WebResponse.GetResponseStream());
                   PageHTML = StreamReader.ReadToEnd();
                }
                catch (WebException e)
                {
                    if (Config.successOn404)
                    {
                        return true;
                    }
                    WebResponse response = e.Response;
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        PageHTML = reader.ReadToEnd();
                    }
                }

                if (SuccessString != null)
                {
                    if (PageHTML.ToLower().Contains(SuccessString.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (FailString != null)
                {
                    if (!PageHTML.ToLower().Contains(FailString.ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "WebRequest Wrapper", ex.StackTrace, ex.TargetSite));
                return Config.successOn404;
            }
        }

    }
}
