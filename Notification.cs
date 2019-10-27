using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace InfoFetch
{
    public static class Notification
    {
        public static void Push(string url, string content, string date)
        {
            string toastContent = string.Format(MyToastWebTemplate, content, url, date);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("InfoFetch").Show(toast);
        }

        public static void Push(string message)
        {
            string toastContent = string.Format(MyToastSimpleTemplate, message);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("InfoFetch").Show(toast);
        }

        private const string MyToastWebTemplate =
            // "<?xml version = \"1.0\" encoding = \"UTF-8\">"+
            "<toast>" +
            "<visual>" +
            "<binding template=\"ToastText04\">" +
            "<text id = \"1\">InfoFetch内容更新</text>+" +
            "<text id = \"2\">内容： {0}</text>+" +
            "<text id = \"3\">网址： {1}\n日期： {2}</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";

        private const string MyToastSimpleTemplate =
            "<toast>" +
            "<visual>" +
            "<binding template=\"ToastText02\">" +
            "<text id = \"1\">InfoFetch提示</text>+" +
            "<text id = \"2\">{0}</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";
    }
}
