using System.Net;
using System.IO;
using System.Text;
// using System.Windows.Forms;

namespace InfoFetch
{
    class Website
    {
        public Website() { }
        
        /// <summary>
        /// Fetch decoded html content given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// true if successfully open and decoded
        /// false if connection fails
        /// </returns>
        public bool Open(string url)
        {
            // Reset website content for new url
            Content = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse responese = (HttpWebResponse)request.GetResponse();
            if (responese.StatusCode == HttpStatusCode.OK)
            {
                byte[] rawData = null;
                // read response stream to a memorystream
                using (Stream recieveStream = responese.GetResponseStream())
                using (MemoryStream stream = new MemoryStream())
                {
                    int count;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = recieveStream.Read(buf, 0, 1024);
                        stream.Write(buf, 0, count);
                    } while (recieveStream.CanRead && count > 0);
                    rawData = stream.ToArray();
                }
                DecodeWeb(rawData); // TODO: push notification if the website may not be successfully decoded
                Url = url;
            }
            else
            {
                return false; // TODO: push notification if the url cannot be connected
            }
            responese.Close();
            return true;

            //WebBrowser wb = new WebBrowser();
            //wb.AllowNavigation = true;
            //wb.ScrollBarsEnabled = false;
            //wb.ScriptErrorsSuppressed = true;
            //wb.Navigate(url);
            //while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }
            //byte[] rawData = null;
            //using (Stream recieveStream = wb.DocumentStream)
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    int count;
            //    do
            //    {
            //        byte[] buf = new byte[1024];
            //        count = recieveStream.Read(buf, 0, 1024);
            //        stream.Write(buf, 0, count);
            //    } while (recieveStream.CanRead && count > 0);
            //    rawData = stream.ToArray();
            //}
            //Content = Encoding.GetEncoding(wb.Document.Encoding).GetString(rawData);
            //Url = url;
            //return true;
        }

        /// <summary>
        /// First decode web content with UTF-8, to get its charset
        /// Then decode the web content using the real encoding
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private bool DecodeWeb(byte[] rawData)
        {
            string rawDataStr = Encoding.UTF8.GetString(rawData); // decode with UTF-8 first
            if (rawDataStr.Contains(DECODESTR))
            {
                int pos = rawDataStr.IndexOf(DECODESTR, 0) + DECODESTR.Length;
                string encoding = "";
                for (; pos < rawDataStr.Length; pos++)
                {
                    if (rawDataStr[pos] == ' ')
                        continue;
                    else if (rawDataStr[pos] == '=')
                        continue;
                    else if (rawDataStr[pos] == '"')
                    {
                        if (encoding == "")
                            continue;
                        else
                            break;
                    }
                    encoding += rawDataStr[pos];
                }
                Content = Encoding.GetEncoding(encoding).GetString(rawData); // decode with real charset
            }
            else
            {
                Content = rawDataStr; // if charset not defined, use UTF-8 as default
                return false;
            }
            return true;
        }

        public string Content { get; set; }
        public string Url { get; set; }
        private const string DECODESTR = "charset";
    }
}
