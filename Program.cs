using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace InfoFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // TODO: Remove this when no console debugging is needed

            // TODO: add program loop in the future

            // Init File Manager
            FileManager file = new FileManager();
            file.Open("../../websites.txt");
            if(!file.Validate())
            {
                Console.WriteLine("websites.txt is has wrong content"); // TODO: Change to console independent code in the future
                Environment.Exit(1);
            }
            string url, dir;
            file.Fetch(out url, out dir);

            // init Website content loader
            Website web = new Website();

            while(url != null)
            {
                if(!web.Open(url))
                {
                    Console.WriteLine("Failed to open " + url); // TODO: Change to console independent code in the future
                    continue;
                }

                // TODO: parse here

                file.Fetch(out url, out dir);
            }

            if(Debugger.IsAttached)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
