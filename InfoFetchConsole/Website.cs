using System.Net;
using OpenQA.Selenium.Chrome;

namespace InfoFetch
{
    class Website
    {
        public Website() { }
        
        /// <summary>
        /// Fetch html content given url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// true if connection is successful
        /// false if connection fails
        /// </returns>
        public bool Open(string url)
        {
            if (driver == null) return false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse responese;
            try
            {
                responese = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Exception)
            {
                return false;
            }

            if (responese.StatusCode != HttpStatusCode.OK)
            {
                responese.Close();
                return false;
            }
            responese.Close();

            driver.Navigate().GoToUrl(url);

            int counter = 0;
            while ((string)driver.ExecuteScript("return document.readyState") != "complete")
            {
                System.Threading.Thread.Sleep(1000);
                counter++;
                if (counter > 20)
                {
                    driver.Close();
                    return false;
                }
            }
            System.Threading.Thread.Sleep(1000);

            Content = driver.FindElementByTagName("HTML").GetAttribute("innerHTML");
            Url = url;

            return true;
        }

        /// <summary>
        /// Start chromedriver
        /// </summary>
        public void StartDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            driver = new ChromeDriver(service, options);
        }

        /// <summary>
        /// Stop chromedriver
        /// </summary>
        public void StopDriver()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
            driver = null;
        }

        public string Content { get; set; }
        public string Url { get; set; }

        private ChromeDriver driver = null;
    }
}
