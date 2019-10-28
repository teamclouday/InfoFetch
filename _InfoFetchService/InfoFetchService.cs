using InfoFetch;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InfoFetchService
{
    public partial class InfoFetchService : ServiceBase
    {
        // Set Service State
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        // Set Service Status
        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }

        /// <summary>
        /// Constructor init eventlog1 and backgroundWork1
        /// </summary>
        /// <param name="args"></param>
        public InfoFetchService(string[] args)
        {
            InitializeComponent();

            DatabasePath = args[0];
            WebsiteDataPath = args[1];

            // init InfoFetch Content
            _website = new Website();
            _parser = new Parser();
            _database = new Database(DatabasePath);
            _fileManager = new FileManager();

            string eventSourceName = "InfoFetchEventSource";
            string logName = "InfoFetchEventLog";
            eventLog1 = new EventLog();
            if(!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        /// <summary>
        /// OnStart, service will init a timer to trigger backgroundWork at certain intervals
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Service Started");

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 30000; // 7200000; // TODO: Should set to 2 hours in Release
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            timer.Start();

            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 120000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            if (!File.Exists(WebsiteDataPath))
            {
                MessageBox.Show(@"未找到文件websites.txt: " + WebsiteDataPath + "\n程序已退出", @"InfoFetch Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                base.OnStop();
            }
            _fileManager.Open(WebsiteDataPath);
            if (!_fileManager.Validate())
            {
                MessageBox.Show(@"文本格式错误: " + WebsiteDataPath + "\n程序已退出", @"InfoFetch Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                base.OnStop();
            }
        }

        /// <summary>
        /// OnStop, stop the service
        /// </summary>
        protected override void OnStop()
        {
            eventLog1.WriteEntry("Service Stopped");

            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_STOP_PENDING,
                dwWaitHint = 120000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// Implementation of timer event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            string eventText = "InfoCheck Running";
            if(!InfoCheck())
            {
                eventText += " (Internet Error Occurred)";
            }
            eventLog1.WriteEntry(eventText, EventLogEntryType.Information, eventId++);
        }

        private bool InfoCheck()
        {
            bool success = true;
            _fileManager.Fetch(out string url, out string dir);
            while(url != null)
            {
                _database.Update(url);
                if(!_website.Open(url))
                {
                    Notification.Push("网络", "无法连接网页: " + url);
                    success = false;
                    continue;
                }
                _parser.Read(_website, dir, _database);
                _fileManager.Fetch(out url, out dir);
            }
            _database.Update();
            return success;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        private int eventId = 1;
        private readonly string DatabasePath;
        private readonly string WebsiteDataPath;

        private readonly Website _website;
        private readonly FileManager _fileManager;
        private readonly Parser _parser;
        private readonly Database _database;
    }
}
