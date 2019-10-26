using System;
using System.Text;

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
            file.Open("../../websites.txt");
            if(!file.Validate())
            {
                Console.WriteLine("websites.txt is has wrong content"); // TODO: Change to console independent code in the future
                Environment.Exit(1);
            }
            file.Fetch(out string url, out string dir);

            while(url != null)
            {
                data.Update(url);
                if(!web.Open(url))
                {
                    Console.WriteLine("Failed to open " + url); // TODO: Change to console independent code in the future
                    continue;
                }
                findinfo.Read(web, dir, data);
                file.Fetch(out url, out dir);
            }

            data.Update();
        }
    }
}
