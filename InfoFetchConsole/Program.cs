using InfoFetch;
using System.IO;
using System.Windows.Forms;

namespace InfoFetchConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            webManager = new Website();
            fileManager = new FileManager();
            parser = new Parser();
            databasePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "webdata.sqlite");
            database = new Database(databasePath);
            websitesPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "websites.txt");
            iconPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "icon.ico");

            System.Threading.Thread notifyThread = new System.Threading.Thread(
            delegate()
            {
                var menuItem1 = new MenuItem();
                menuItem1.Index = 0;
                menuItem1.Text = "Open Program Location";
                menuItem1.Click += new System.EventHandler(IconMenuClickEvent1);

                var menuItem2 = new MenuItem();
                menuItem2.Index = 1;
                menuItem2.Text = "Edit Websites.txt";
                menuItem2.Click += new System.EventHandler(IconMenuClickEvent2);

                var menuItem3 = new MenuItem();
                menuItem3.Index = 2;
                menuItem3.Text = "Exit";
                menuItem3.Click += new System.EventHandler(IconMenuClickEvent3);

                var contextMenu = new ContextMenu();
                contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem1, menuItem2, menuItem3 });

                trayIcon = new NotifyIcon();
                trayIcon.Visible = true;
                trayIcon.Icon = new System.Drawing.Icon(iconPath);
                trayIcon.DoubleClick += new System.EventHandler(IconDoubleClickEvent);
                trayIcon.Text = "InfoFetch Program";
                trayIcon.ContextMenu = contextMenu;

                Application.Run();
            }
            );
            notifyThread.Start();

            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimeEvent);
            myTimer.Enabled = true;
            myTimer.AutoReset = false;
            myTimer.Start();

            while (myTimer.Enabled)
            {
                System.Threading.Thread.Sleep(50);
            }
            notifyThread.Join();

            myTimer.Stop();
        }

        /// <summary>
        /// The main task on each run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(JobRunning)
            {
                // to avoid conflict, return this run
                return;
            }

            if(!myTimer.AutoReset)
            {
                myTimer.Stop();
                myTimer.Interval = 5000;
                myTimer.AutoReset = true;
                myTimer.Start();
            }

            JobRunning = true;

            // Check website data
            if (!File.Exists(websitesPath))
            {
                MessageBox.Show(@"未找到文件websites.txt，程序已退出", @"InfoFetch Error", MessageBoxButtons.OK);
                myTimer.Stop();
                return;
            }
            fileManager.Open(websitesPath);
            if (!fileManager.Validate())
            {
                MessageBox.Show(@"websites.txt的格式错误，程序已退出", @"InfoFetch Error", MessageBoxButtons.OK);
                myTimer.Stop();
                return;
            }

            fileManager.Fetch(out string url, out string dir);

            while (url != null)
            {
                database.Update(url);
                if (!webManager.Open(url))
                {
                    MyNotification.Push("网络", "无法连接网页: " + url);
                    continue;
                }
                parser.Read(webManager, dir, database);
                fileManager.Fetch(out url, out dir);
            }

            database.Update();
            JobRunning = false;
        }

        /// <summary>
        /// Double click event: will launch the github repo url of this project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconDoubleClickEvent(object sender, System.EventArgs e)
        {
            string githuburl = @"https://github.com/teamclouday/InfoFetch";
            System.Diagnostics.Process.Start(githuburl);
        }

        /// <summary>
        /// MenuItem1 click event: will open the program location in file explorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconMenuClickEvent1(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
        }

        /// <summary>
        /// MenuItem2 click event: will open the websites.txt file in text editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconMenuClickEvent2(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(websitesPath);
        }

        /// <summary>
        /// MenuItem3 click event: will exit the whole program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconMenuClickEvent3(object sender, System.EventArgs e)
        {
            myTimer.Stop();
            Application.Exit();
        }

        private static Website webManager;
        private static FileManager fileManager;
        private static Parser parser;
        private static Database database;
        private static System.Timers.Timer myTimer;
        private static NotifyIcon trayIcon;

        public static string databasePath;
        public static string websitesPath;
        private static string iconPath;

        public static bool JobRunning = false;
    }
}
