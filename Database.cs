using System;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace InfoFetch
{
    class Database
    {
        public Database(string path)
        {
            DATABASE_PATH = path;
            bool firstInit = false;
            if(!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                firstInit = true;
            }
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = DATABASE_PATH;
            builder.FailIfMissing = true;
            con = new SQLiteConnection(builder.ConnectionString);
            cmd = new SQLiteCommand(con);

            if(firstInit)
            {
                con.Open();
                cmd.Reset();
                cmd.CommandText = "CREATE TABLE WebsiteData (URL TEXT NOT NULL, MESSAGE TEXT NOT NULL, DATE CHAR(30));";
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void Browse()
        {
            // Just for debugging
            con.Open();
            cmd.Reset();
            cmd.CommandText = "SELECT * FROM WebsiteData";
            SQLiteDataReader reader = cmd.ExecuteReader();
            Console.WriteLine(reader.FieldCount);
            con.Close();
        }

        private string DATABASE_PATH { get; set; }
        private SQLiteConnection con;
        private SQLiteCommand cmd;
    }
}
