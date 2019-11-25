using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using OpenQA.Selenium.Chrome;

namespace InfoFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // TODO: Remove this when no console debugging is needed

            // TODO: add program loop in the future

            var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path != null)
            {
                Console.WriteLine(FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion);
            }
            else
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                if(path != null)
                    Console.WriteLine(FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion);
            }

            ChromeDriver driver = new ChromeDriver();
            Console.WriteLine(driver.Capabilities["browserVersion"].ToString());
            driver.Close();

            // init all
            Website web = new Website();
            Parser findinfo = new Parser();
            Database data = new Database("../../webdata.sqlite");
            // data.Browse();

            // init File Manager
            FileManager file = new FileManager();
            if(!File.Exists("../../websites.txt"))
            {
                MessageBox.Show(@"未找到文件websites.txt，程序已退出", @"InfoFetch Error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
            file.Open("../../websites.txt");
            if(!file.Validate())
            {
                MessageBox.Show(@"websites.txt的格式错误，程序已退出", @"InfoFetch Error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
            file.Fetch(out string url, out string dir);

            while(url != null)
            {
                data.Update(url);
                if(!web.Open(url))
                {
                    Notification.Push("网络", "无法连接网页: " + url);
                    continue;
                }
                Console.WriteLine(web.Content);
                findinfo.Read(web, dir, data);
                file.Fetch(out url, out dir);
            }

            data.Update();
        }
    }
}
