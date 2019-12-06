using InfoFetch;
using Microsoft.Win32;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Windows.Forms;

namespace InfoFetchConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckLocalization();

            MyNotification.Push(localeM.GetString("ProgramStart"), localeM.GetString("Wait"));

            if (!Driver.CheckChromeVersion())
            {
                MessageBox.Show(localeM.GetString("UnknownChromeVersion"), localeM.GetString("InfoFetchError"), MessageBoxButtons.OK);
                System.Environment.Exit(1);
            }
            else
            {
                if (!Driver.CheckChromeDriver())
                {
                    MessageBox.Show(localeM.GetString("ChromedriverDownloadFail"), localeM.GetString("InfoFetchError"), MessageBoxButtons.OK);
                    System.Environment.Exit(1);
                }
                else
                {
                    Driver.UpdateLocalHtml();
                }
            }

            MyNotification.Push(localeM.GetString("ProgramStarted"), string.Format(localeM.GetString("SystemTrayManage"), (UpdateInterval / 3600000)));

            webManager = new Website();
            fileManager = new FileManager();
            parser = new Parser();
#if DEBUG
            databasePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "webdata.sqlite");
#else
            databasePath = "webdata.sqlite";
#endif
            database = new Database(databasePath);
#if DEBUG
            websitesPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "websites.txt");
            iconPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())), "icon.ico");
#else
            websitesPath = "websites.txt";
            iconPath = "icon.ico";
#endif

            notifyThread = new System.Threading.Thread(
            delegate()
            {
                var menuItem1 = new MenuItem();
                menuItem1.Index = 0;
                menuItem1.Text = localeM.GetString("OpenLocation");
                menuItem1.Click += new System.EventHandler(IconMenuClickEvent1);

                var menuItem2 = new MenuItem();
                menuItem2.Index = 1;
                menuItem2.Text = localeM.GetString("EditWebsites");
                menuItem2.Click += new System.EventHandler(IconMenuClickEvent2);

                var menuItem5 = new MenuItem();
                menuItem5.Index = 2;
                menuItem5.Text = localeM.GetString("ChangeInterval");
                menuItem5.Click += new System.EventHandler(IconMenuClickEvent5);

                var menuItem4 = new MenuItem();
                menuItem4.Index = 3;
                menuItem4.Text = localeM.GetString("ToggleAutostart");
                menuItem4.Click += new System.EventHandler(IconMenuClickEvent4);

                var menuItem3 = new MenuItem();
                menuItem3.Index = 4;
                menuItem3.Text = localeM.GetString("Exit");
                menuItem3.Click += new System.EventHandler(IconMenuClickEvent3);

                var contextMenu = new ContextMenu();
                contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem1, menuItem2, menuItem5, menuItem4, menuItem3 });

                trayIcon = new NotifyIcon();
                trayIcon.Visible = true;
                trayIcon.Icon = new System.Drawing.Icon(iconPath);
                trayIcon.DoubleClick += new System.EventHandler(IconDoubleClickEvent);
                trayIcon.Text = localeM.GetString("IconText");
                trayIcon.ContextMenu = contextMenu;

                Application.Run();
            }
            );
            notifyThread.Start();

            myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimeEvent);
            myTimer.AutoReset = false;
            myTimer.Start();

            while (myTimer.Enabled)
            {
                System.Threading.Thread.Sleep(20);
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
            System.Console.WriteLine("A Job Begins");
            if(JobRunning)
            {
                // to avoid conflict, return this run
                return;
            }

            if(!myTimer.AutoReset)
            {
                myTimer.Interval = UpdateInterval;
                myTimer.AutoReset = true;
            }

            JobRunning = true;

            // Check website data
            if (!File.Exists(websitesPath))
            {
                MessageBox.Show(localeM.GetString("TxtNotFound"), localeM.GetString("InfoFetchError"), MessageBoxButtons.OK);
                myTimer.Stop();
                notifyThread.Abort();
                return;
            }
            fileManager.Open(websitesPath);
            if (!fileManager.Validate())
            {
                MessageBox.Show(localeM.GetString("TxtWrongFormat"), localeM.GetString("InfoFetchError"), MessageBoxButtons.OK);
                myTimer.Stop();
                notifyThread.Abort();
                return;
            }

            fileManager.Fetch(out string url, out string dir);

            webManager.StartDriver();

            while (url != null)
            {
                database.Update(url);
                if (!webManager.Open(url))
                {
                    MyNotification.Push(localeM.GetString("Network"), localeM.GetString("CannotConnect") + url);
                }
                else
                {
                    parser.Read(webManager, dir, database);
                }
                fileManager.Fetch(out url, out dir);
            }

            webManager.StopDriver();

            database.Update();
            database.Reset();

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
#if DEBUG
            System.Diagnostics.Process.Start(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
#else
            System.Diagnostics.Process.Start(Directory.GetCurrentDirectory());
#endif
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
            if(JobRunning)
            {
                MyNotification.Push(localeM.GetString("WaitBackground"), localeM.GetString("WaitExit"));
            }
            while(JobRunning)
            {
                System.Threading.Thread.Sleep(50);
            }
            webManager.StopDriver();
            Application.Exit();
        }

        /// <summary>
        /// MenuItem4 click event: will toggle autostart with system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconMenuClickEvent4(object sender, System.EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if(key == null)
            {
                Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            }
            if(key.GetValue(AppID) == null)
            {
#if DEBUG
                string startPath = Application.ExecutablePath.ToString();
#else
                string startPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Programs), "Sida Zhu", "Teamclouday", "InfoFetch.appref-ms");
#endif
                key.SetValue(AppID, startPath, RegistryValueKind.String);
                MyNotification.Push(localeM.GetString("AutoStart"), localeM.GetString("Activated"));
            }
            else
            {
                key.DeleteValue(AppID, false);
                MyNotification.Push(localeM.GetString("AutoStart"), localeM.GetString("Deactivated"));
            }
        }

        /// <summary>
        /// MenuItem5 click event: will show a input box to update a new interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IconMenuClickEvent5(object sender, System.EventArgs e)
        {
            if(JobRunning)
            {
                MyNotification.Push(localeM.GetString("WaitBackground"), localeM.GetString("IntervalWillShow"));
            }
            myTimer.AutoReset = false;
            while(JobRunning)
            {
                System.Threading.Thread.Sleep(50);
            }
            if(MyUI.IntervalInput(UpdateInterval, out long newInterval) == DialogResult.OK)
            {
                UpdateInterval = newInterval;
            }
            myTimer.Interval = UpdateInterval;
            myTimer.Start();
        }

        /// <summary>
        /// Set Resource Manager based on locale on Computer
        /// </summary>
        /// <returns></returns>
        private static void CheckLocalization()
        {
            //CultureInfo[] current = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
            //foreach(CultureInfo ci in current)
            //{
            //    System.Console.WriteLine(ci.EnglishName);
            //}

            string lan = "en-US";

            foreach (InputLanguage c in InputLanguage.InstalledInputLanguages)
            {
                if(c.Culture.Name.Substring(0, 2) == "zh")
                {
                    lan = "zh-CN";
                    break;
                }
            }

            localeInfo = CultureInfo.GetCultureInfo(lan);
            CultureInfo.DefaultThreadCurrentCulture = localeInfo;
            CultureInfo.DefaultThreadCurrentUICulture = localeInfo;

            localeM = new ResourceManager("language", System.Reflection.Assembly.GetExecutingAssembly());
        }

        private static Website webManager;
        private static FileManager fileManager;
        private static Parser parser;
        private static Database database;
        private static System.Timers.Timer myTimer;
        private static NotifyIcon trayIcon;
        private static System.Threading.Thread notifyThread;

        public static string databasePath;
        public static string websitesPath;
        private static string iconPath;

        public static bool JobRunning = false;

        public const string AppID = @"InfoFetch";
        private static long UpdateInterval = 28800000;

        public static ResourceManager localeM;
        public static CultureInfo localeInfo;
    }
}