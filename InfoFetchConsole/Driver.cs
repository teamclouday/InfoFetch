using System.IO;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using OpenQA.Selenium.Chrome;

namespace InfoFetch
{
    public static class Driver
    {
        /// <summary>
        /// check local chrome version
        /// </summary>
        /// <returns>
        /// true if chrome version is found
        /// false if chrome version is unknown
        /// </returns>
        public static bool CheckChromeVersion()
        {
            var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path != null)
            {
                localChromeVersion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
                return true;
            }
            else
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                if (path != null)
                {
                    localChromeVersion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check local chromedriver
        /// </summary>
        /// <returns>
        /// true if chromedriver is found and has correct version
        /// false if not found
        /// </returns>
        public static bool CheckChromeDriver()
        {
            bool driverOK = true;
            if(!File.Exists("chromedriver.exe"))
            {
                MyNotification.Push(@"Chrome驱动", "未找到chromedriver");
                driverOK = false;
            }
            else if(!CheckChromeDriverVersion())
            {
                MyNotification.Push(@"Chrome驱动", "chromedriver版本不正确");
                driverOK = false;
            }
            if(!driverOK)
            {
                return DownloadChromeDriver();
            }

            return driverOK;
        }

        /// <summary>
        /// download the correct chromedriver and remove the one with wrong version
        /// </summary>
        /// <returns>
        /// true if download succeeded
        /// false if download failed
        /// </returns>
        private static bool DownloadChromeDriver()
        {
            MyNotification.Push(@"Chrome驱动", @"正在下载chromedriver");

            string webContent;
            if(File.Exists(localBackupFile))
            {
                StreamReader sr = new StreamReader(localBackupFile);
                webContent = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                return false;
            }
            if (string.IsNullOrEmpty(webContent)) return false;

            Regex versionReg = new Regex(@"\d+.\d+.\d+.\d+");
            Match versionMatch = versionReg.Match(webContent);
            string targetVersion = string.Empty;
            string[] chromeV = localChromeVersion.Split('.');
            while(versionMatch.Success)
            {
                string version = versionMatch.Value;
                string[] driverV = version.Split('.');
                if(chromeV[0] == driverV[0] && chromeV[1] == driverV[1] && chromeV[2] == driverV[2])
                {
                    targetVersion = version;
                    break;
                }
                versionMatch = versionMatch.NextMatch();
            }
            if (string.IsNullOrEmpty(targetVersion)) return false;

            string targetURL = downloadURL + targetVersion + "/chromedriver_win32.zip";

            if(File.Exists("chromedriver_win32.zip"))
            {
                File.Delete("chromedriver_win32.zip");
            }

            using(var downloader = new WebClient())
            {
                downloader.DownloadFile(targetURL, "chromedriver_win32.zip");
            }

            if (!File.Exists("chromedriver_win32.zip")) return false;

            if(File.Exists("chromedriver.exe"))
            {
                File.Delete("chromedriver.exe");
            }

            ZipFile.ExtractToDirectory("chromedriver_win32.zip", ".");

            if (File.Exists("chromedriver.exe"))
            {
                File.Delete("chromedriver_win32.zip");
            }
            else
                return false;

            return true;
            
        }

        /// <summary>
        /// check local chromedriver version
        /// </summary>
        /// <returns>
        /// true if match with chrome version
        /// false if not match
        /// </returns>
        private static bool CheckChromeDriverVersion()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "chromedriver.exe";
            p.StartInfo.Arguments = "-v";
            p.Start();
            string driveroutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Dispose();

            Regex versionReg = new Regex(@"\d+.\d+.\d+.\d+");
            Match versionMatch = versionReg.Match(driveroutput);
            string driverVersion;
            if (versionMatch.Success)
            {
                driverVersion = versionMatch.Value;
            }
            else
            {
                MyNotification.Push(@"Chrome驱动", @"未知chromedriver版本");
                return false;
            }

            if (string.IsNullOrEmpty(localChromeVersion)) return false;

            string[] chromeV = localChromeVersion.Split('.');
            string[] driverV = driverVersion.Split('.');

            if(chromeV[0] == driverV[0] && chromeV[1] == driverV[1] && chromeV[2] == driverV[2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// after driver is correct, update local html
        /// </summary>
        public static void UpdateLocalHtml()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            ChromeDriver driver = new ChromeDriver(service, options);

            driver.Navigate().GoToUrl(indexURL);

            int counter = 0;
            while((string)driver.ExecuteScript("return document.readyState") != "complete")
            {
                System.Threading.Thread.Sleep(1000);
                counter++;
                if(counter > 20)
                {
                    driver.Close();
                    return;
                }
            }
            System.Threading.Thread.Sleep(1000);
            string content = driver.FindElementByTagName("HTML").GetAttribute("innerHTML");

            File.WriteAllText(localBackupFile, content);

            driver.Quit();
            driver.Dispose();
        }

        public static string localChromeVersion = string.Empty;
        public const string downloadURL = @"https://chromedriver.storage.googleapis.com/";
        public const string indexURL = @"https://chromedriver.storage.googleapis.com/index.html";
        public const string localBackupFile = @"chromedriver.txt";
    }
}
