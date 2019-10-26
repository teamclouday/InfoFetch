using System;
using System.Data;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;

namespace InfoFetch
{
    class Database
    {
        /// <summary>
        /// Create connection to the database
        /// If necessary, create a new database
        /// </summary>
        /// <param name="path"></param>
        public Database(string path)
        {
            DATABASE_PATH = path;
            bool firstInit = false;
            if(!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                firstInit = true;
            }
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
            {
                DataSource = DATABASE_PATH,
                FailIfMissing = true
            };
            con = new SQLiteConnection(builder.ConnectionString);
            cmd = new SQLiteCommand(con);

            if(firstInit)
            {
                // TODO: add popup indicating that new database is created
                con.Open();
                cmd.Reset();
                cmd.CommandText = "CREATE TABLE WebsiteData (URL TEXT NOT NULL, MESSAGE TEXT NOT NULL, DATE TEXT NOT NULL);";
                cmd.ExecuteNonQuery();
                con.Close();
            }

            URLS = new List<string>();
        }

        /// <summary>
        /// Just for debugging, should not be used in actual program
        /// </summary>
        public void Browse()
        {
            // Just for debugging
            con.Open();
            cmd.Reset();
            cmd.CommandText = "SELECT * FROM WebsiteData;";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                Console.WriteLine("\n"+reader.GetString(0) + "\n" + reader.GetString(1) + "\n" + reader.GetString(2)+"\n");
            }
            reader.Close();
            con.Close();
        }

        /// <summary>
        /// Simply add new url to the URL list
        /// </summary>
        /// <param name="url"></param>
        public void Update(string url)
        {
            URLS.Add(url);
        }

        /// <summary>
        /// Will execute after a loop of urls are visited
        /// Check all the urls in database and remove the redundant rows
        /// </summary>
        public void Update()
        {
            List<string> to_remove = new List<string>();
            // Check for updates
            con.Open();
            cmd.Reset();
            cmd.CommandText = "SELECT DISTINCT URL FROM WebsiteData;";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                string distinct_url = reader.GetString(0);
                if (!URLS.Contains(distinct_url))
                {
                    to_remove.Add(distinct_url);
                }
            }
            reader.Close();
            // Updates
            foreach (string url in to_remove)
            {
                cmd.Reset();
                cmd.Parameters.Add("@urlname", DbType.String).Value = url;
                cmd.CommandText = "DELETE FROM WebsiteData WHERE URL=@urlname;";
                cmd.ExecuteNonQuery();
            }

            // TODO: Remove redundant content of urls (if > 50 rows)

            con.Close();
        }

        /// <summary>
        /// Compare new extracted data with exisiting data in database
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="date"></param>
        public void Compare(string url, string content, string date)
        {
            con.Open();
            cmd.Reset();
            cmd.Parameters.AddWithValue("@urlname", url);
            cmd.Parameters.AddWithValue("@urlcontent", content);
            cmd.Parameters.AddWithValue("@contentdate", date);
            cmd.CommandText = "SELECT COUNT(*) FROM WebsiteData WHERE URL=@urlname AND DATE=@contentdate AND MESSAGE=@urlcontent;";
            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                Add(url, content, date); // TODO: Invoke a new notification
            con.Close();
        }

        /// <summary>
        /// Create info for new content
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="date"></param>
        private void Add(string url, string content, string date)
        {
            cmd.Reset();
            cmd.Parameters.AddWithValue("@urlname", url);
            cmd.Parameters.AddWithValue("@urlcontent", content);
            cmd.Parameters.AddWithValue("@contentdate", date);
            cmd.CommandText = "INSERT INTO WebsiteData (URL, MESSAGE, DATE) VALUES (@urlname, @urlcontent, @contentdate);";
            cmd.ExecuteNonQuery();
        }

        private string DATABASE_PATH { get; set; }
        private readonly SQLiteConnection con;
        private readonly SQLiteCommand cmd;

        private List<string> URLS;
    }
}
