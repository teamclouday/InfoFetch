using System;
using HtmlAgilityPack;

namespace InfoFetch
{
    class Parser
    {
        public Parser() { }

        /// <summary>
        /// Send html to HtmlAgilityPack
        /// Then request the selected sections
        /// </summary>
        /// <param name="web"></param>
        /// <param name="direction"></param>
        public void Read(Website web, string direction)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(web.Content);
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes(direction);
            if(htmlNodes == null)
            {
                Console.WriteLine("URL: " + web.Url + "\nSelector: " + direction + "\nNo Match!"); // TODO: Change to console independent code in the future
                return;
            }
            for (int i = 0; i < htmlNodes.Count; i++)
                Console.WriteLine(htmlNodes[i].InnerText);
        }
    }
}
