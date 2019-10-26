using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace InfoFetch
{
    class Parser
    {
        public Parser() { }

        /// <summary>
        /// Send html to HtmlAgilityPack
        /// Then request the selected sections
        /// At last let database update content
        /// </summary>
        /// <param name="web"></param>
        /// <param name="direction"></param>
        public void Read(Website web, string direction, Database database)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(web.Content);
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes(direction);
            if(htmlNodes == null)
            {
                Console.WriteLine("URL: " + web.Url + "\nSelector: " + direction + "\nNo Match!"); // TODO: Change to console independent code in the future
                return;
            }
            // reverse the order, so that the latest date appears at the end of the database
            for (int i = htmlNodes.Count - 1; i >= 0; i--)
            {
                ParseContent(htmlNodes[i].InnerText, out string newContent, out string newDate);
                database.Compare(web.Url, newContent, newDate);
            }
        }

        /// <summary>
        /// Convert content to single space format
        /// Convert date to mm/dd/yyyy format, if not exist, return empty string
        /// </summary>
        /// <param name="content"></param>
        /// <param name="newContent"></param>
        /// <param name="newDate"></param>
        private void ParseContent(string content, out string newContent, out string newDate)
        {
            // First find the date
            Regex dateReg = new Regex(@".*20\d\d.{1,2}\d{1,2}.{1,2}\d{1,2}.*"); 
            Match dateMatch = dateReg.Match(content);
            if(dateMatch.Success)
            {
                int dateYear = Convert.ToInt32(Regex.Match(dateMatch.Value, @"\d\d\d\d").Value);
                Match dateCollect = Regex.Match(dateMatch.Value.Replace(dateYear.ToString(), ""), @"\d{1,2}");
                int dateMonth = Convert.ToInt32(dateCollect.Value);
                int dateDay = Convert.ToInt32(dateCollect.NextMatch().Value);
                if(dateMonth > 12) // In case that day comes before month
                {
                    int tmp = dateMonth;
                    dateMonth = dateDay;
                    dateDay = tmp;
                }
                newDate = string.Format("{0:d2}/{1:d2}/{2:d4}", dateMonth, dateDay, dateYear);
            }
            else
            {
                newDate = "";
            }
            // Next generate new content
            newContent = content;
            if (dateMatch.Success)
                newContent = newContent.Replace(dateMatch.Value, "");
            newContent = Regex.Replace(newContent, @"\s+", " ");
            newContent = newContent.TrimStart().TrimEnd();
        }
    }
}
