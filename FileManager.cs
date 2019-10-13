using System;
using System.IO;
using System.Collections.Generic;

namespace InfoFetch
{
    class FileManager
    {
        public FileManager() 
        {
            URL = new Queue<string>();
            Direction = new Queue<string>();
        }

        /// <summary>
        /// Open a file path, read URL and Selector Direction
        /// Rule: First line URL, then next line should be selector(XPath)
        /// If commented with "#", then the line is skipped
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path)
        {
            URL.Clear();
            Direction.Clear();
            StreamReader myReader = new StreamReader(path);

            string line;
            while((line = myReader.ReadLine()) != null)
            {
                if (line.Length == 0 || line[0] == '#')
                    continue;
                if (URL.Count == Direction.Count)
                    URL.Enqueue(line);
                else
                    Direction.Enqueue(line);
            }

            myReader.Close();
        }

        /// <summary>
        /// Validate the URL in the file, and Direction number
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if(URL.Count != Direction.Count || URL.Count == 0)
                return false;
            string[] urls = URL.ToArray();
            for(int i = 0; i < urls.Length; i++)
            {
                Uri result;
                if(!Uri.TryCreate(urls[i], UriKind.Absolute, out result) || !(result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Fetch one url and one direction from the file content
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dir"></param>
        public void Fetch(out string url, out string dir)
        {
            if(URL.Count == 0 || Direction.Count == 0)
            {
                url = null;
                dir = null;
            }
            else
            {
                url = URL.Dequeue();
                dir = Direction.Dequeue();
            }
        }

        public Queue<string> URL;
        public Queue<string> Direction;
    }
}
