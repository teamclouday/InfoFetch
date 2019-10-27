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

            // init all
            Website web = new Website();
            Parser findinfo = new Parser();
            Database data = new Database("../../webdata.sqlite");
            data.Browse();

            // init File Manager
            FileManager file = new FileManager();
            if(!File.Exists(".. / .. / websites.txt"))
            {
                MessageBox.Show(@"未找到文件websites.txt", @"InfoFetch Error", MessageBoxButtons.OK);
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
                findinfo.Read(web, dir, data);
                file.Fetch(out url, out dir);
            }

            data.Update();
        }
    }
}
