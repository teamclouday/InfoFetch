﻿using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

// Code inspired from https://github.com/psantosl/ConsoleToast/blob/master/Program.cs

namespace InfoFetch
{
    public static class MyNotification
    {
        /// <summary>
        /// Push fetched website content
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="date"></param>
        public static void Push(string url, string content, string date)
        {
            url = url.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            content = content.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            string toastContent = string.Format(MyToastWebTemplate, content, date, url, url);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastEvents toastEvents = new ToastEvents();
            toast.Activated += toastEvents.ToastActivated;
            ToastNotificationManager.CreateToastNotifier(InfoFetchConsole.Program.AppID).Show(toast);
        }

        /// <summary>
        /// Push normal notifications
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        public static void Push(string header, string message)
        {
            message = message.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("&", "&amp;").Replace("\'", "&apos;");
            string toastContent = string.Format(MyToastSimpleTemplate, header, message);
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent);
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier(InfoFetchConsole.Program.AppID).Show(toast);
        }

        private static string MyToastWebTemplate =
            "<toast launch=\"{3}\">" +
            "<visual>" +
            "<binding template=\"ToastText04\">" +
            "<text id = \"1\">" + InfoFetchConsole.Program.localeM.GetString("InfoFetchContentUpdate") + "</text>+" +
            "<text id = \"2\">" + InfoFetchConsole.Program.localeM.GetString("Content") + "</text>+" +
            "<text id = \"3\">" + InfoFetchConsole.Program.localeM.GetString("Date") + "\n" +
            InfoFetchConsole.Program.localeM.GetString("URL") + "</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";

        private static string MyToastSimpleTemplate =
            "<toast>" +
            "<visual>" +
            "<binding template=\"ToastText02\">" +
            "<text id = \"1\">" + InfoFetchConsole.Program.localeM.GetString("InfoFetchMessage") + "</text>+" +
            "<text id = \"2\">{1}</text>+" +
            "</binding>" +
            "</visual>" +
            "</toast>";
    }

    public class ToastEvents
    {
        /// <summary>
        /// For launching the web page on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ToastActivated(ToastNotification sender, object e)
        {
            var args = e as ToastActivatedEventArgs;
            string url = args.Arguments;
            System.Diagnostics.Process.Start(url);
        }
    }
}
