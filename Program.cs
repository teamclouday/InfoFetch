using System;
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

            Website webManager = new Website();
            webManager.Open("http://www.cninfo.com.cn/new/disclosure/stock?orgId=9900031463&stockCode=300630");
            Console.WriteLine(webManager.Content);

            if(Debugger.IsAttached)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
