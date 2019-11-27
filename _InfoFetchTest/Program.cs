using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InfoFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // TODO: Remove this when no console debugging is needed

            // TODO: add program loop in the future

            if(!Driver.CheckChromeVersion())
            {
                MessageBox.Show(@"未知Chrome版本，程序将退出", @"InfoFetch Error", MessageBoxButtons.OK);
                Environment.Exit(1);
            }
            else
            {
                if(!Driver.CheckChromeDriver())
                {
                    MessageBox.Show(@"chromedriver下载失败，程序将退出", @"InfoFetch Error", MessageBoxButtons.OK);
                    Environment.Exit(1);
                }
                else
                {
                    Driver.UpdateLocalHtml();
                }
            }


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

            web.StartDriver();

            while(url != null)
            {
                data.Update(url);
                if(!web.Open(url))
                {
                    Notification.Push("网络", "无法连接网页: " + url);
                    file.Fetch(out url, out dir);
                    continue;
                }
                Console.WriteLine(web.Content);
                findinfo.Read(web, dir, data);
                file.Fetch(out url, out dir);
            }

            web.StopDriver();

            data.Update();
        }
    }
}
