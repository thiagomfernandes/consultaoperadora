using System.IO;
using System.Net;
using System.Text;

namespace ConsultaOperadora.Models
{
    public static class WebAccess
    {
        public static string GetStringFromUrlExternal(string url, ref CookieContainer _cookies, string method = "GET", string data = null, string contentType = "application/x-www-form-urlencoded", string referer = "", int timeout = 0)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                request.CookieContainer = _cookies;
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; Touch; rv:11.0) like Gecko";


                request.AllowAutoRedirect = true;
                request.Method = method;
                request.Headers.Add("Accept-Language:pt-BR");
                request.Headers.Add("Accept-Encoding: gzip, deflate");

                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (!string.IsNullOrEmpty(referer))
                    request.Referer = referer;

                ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                request.Timeout = timeout > 0 ? timeout : 1000;

                if (!string.IsNullOrEmpty(data))
                {
                    request.ContentType = contentType;
                    byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(data);
                    request.ContentLength = bytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                var response = request.GetResponse();

                try
                {
                    using (var receiveStream = response.GetResponseStream())
                    {
                        if (receiveStream != null)
                        {
                            var readStream = new StreamReader(receiveStream, Encoding.GetEncoding("ISO-8859-1"));
                            return readStream.ReadToEnd();
                        }
                    }
                }
                catch { }
                finally
                {
                    response.Close();
                }
            }
            catch { }
            
            return string.Empty;
        }
    }
}
