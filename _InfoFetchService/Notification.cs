using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace InfoFetch
{
    public static class Notification
    {
        public static void Push(string url, string content, string date)
        {
            url = url.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            content = content.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            string toastContent = string.Format(MyToastWebTemplate, content, date, url);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("InfoFetch").Show(toast);
        }

        public static void Push(string header, string message)
        {
            message = message.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            string toastContent = string.Format(MyToastSimpleTemplate, header, message);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("InfoFetch").Show(toast);
        }

        private const string MyToastWebTemplate =
            "<toast>" +
            "<visual>" +
            "<binding template=\"ToastText04\">" +
            "<text id = \"1\">InfoFetch内容更新</text>+" +
            "<text id = \"2\">内容： {0}</text>+" +
            "<text id = \"3\">日期： {1}\n网址： {2}</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";

        private const string MyToastSimpleTemplate =
            "<toast>" +
            "<visual>" +
            "<binding template=\"ToastText02\">" +
            "<text id = \"1\">InfoFetch提示：{0}</text>+" +
            "<text id = \"2\">{1}</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";
    }
}
