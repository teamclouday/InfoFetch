using System.Windows.Forms;
using System.Drawing;

namespace InfoFetch
{
    /// <summary>
    /// Enum for interval time format
    /// </summary>
    public enum IntervalFormat
    {
        IntervalDay,
        IntervalHour,
        IntervalMinute,
        IntervalSecond
    }

    static class MyUI
    {
        /// <summary>
        /// Show an input box to change interval
        /// Idea comes from https://www.csharp-examples.net/inputbox/
        /// </summary>
        /// <returns></returns>
        public static DialogResult IntervalInput(long currentInterval, out long newInterval)
        {
            Form form = new Form();
            form.Text = InfoFetchConsole.Program.localeM.GetString("ChangeInterval");

            Button buttonOK = new Button();
            Button buttonCancel = new Button();

            buttonOK.Text = InfoFetchConsole.Program.localeM.GetString("OK");
            buttonCancel.Text = InfoFetchConsole.Program.localeM.GetString("Cancel");
            buttonOK.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            buttonOK.SetBounds(20, 70, 70, 20);
            buttonCancel.SetBounds(110, 70, 70, 20);

            IntervalFormat format = ConvertTime(currentInterval, out int txtBoxValue);

            TextBox txtBox = new TextBox();
            txtBox.Text = txtBoxValue.ToString();
            txtBox.Focus();
            txtBox.AcceptsReturn = false;
            txtBox.AcceptsTab = false;
            txtBox.SetBounds(10, 10, 80, 50);
            txtBox.Font = new Font(FontFamily.GenericSansSerif, 20);
            txtBox.KeyPress += Txtbox_KeyPress;
            txtBox.MaxLength = 4;

            ComboBox comboBox = new ComboBox();
            string[] formats = new string[] {
                InfoFetchConsole.Program.localeM.GetString("Day"),
                InfoFetchConsole.Program.localeM.GetString("Hour"),
                InfoFetchConsole.Program.localeM.GetString("Minute"),
                InfoFetchConsole.Program.localeM.GetString("Second")
            };
            comboBox.Items.AddRange(formats);
            comboBox.SetBounds(100, 10, 90, 50);
            switch(format)
            {
                case IntervalFormat.IntervalDay:
                    comboBox.SelectedIndex = 0;
                    break;
                case IntervalFormat.IntervalHour:
                    comboBox.SelectedIndex = 1;
                    break;
                case IntervalFormat.IntervalMinute:
                    comboBox.SelectedIndex = 2;
                    break;
                case IntervalFormat.IntervalSecond:
                    comboBox.SelectedIndex = 3;
                    break;
            }
            comboBox.Font = new Font(FontFamily.GenericSansSerif, 18);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            form.ClientSize = new Size(200, 100);
            form.Controls.AddRange(new Control[] { buttonOK, buttonCancel, txtBox, comboBox });
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowInTaskbar = true;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOK;
            form.CancelButton = buttonCancel;

            DialogResult result = form.ShowDialog();
            if(result == DialogResult.OK)
            {
                string notificationFormat = "";
                switch(comboBox.SelectedIndex)
                {
                    case 0:
                        format = IntervalFormat.IntervalDay;
                        notificationFormat = InfoFetchConsole.Program.localeM.GetString("Day");
                        break;
                    case 1:
                        format = IntervalFormat.IntervalHour;
                        notificationFormat = InfoFetchConsole.Program.localeM.GetString("Hour");
                        break;
                    case 2:
                        format = IntervalFormat.IntervalMinute;
                        notificationFormat = InfoFetchConsole.Program.localeM.GetString("Minute");
                        break;
                    case 3:
                        format = IntervalFormat.IntervalSecond;
                        notificationFormat = InfoFetchConsole.Program.localeM.GetString("Second");
                        break;
                    default:
                        format = IntervalFormat.IntervalSecond;
                        break;
                }
                int num = System.Convert.ToInt32(txtBox.Text);
                newInterval = ConvertBackTime(num, format);

                MyNotification.Push(InfoFetchConsole.Program.localeM.GetString("IntervalUpdate"),
                    InfoFetchConsole.Program.localeM.GetString("IntervalNew") + " " + num.ToString() + " " + notificationFormat);
            }
            else
            {
                newInterval = 0;
            }
            return result;
        }

        /// <summary>
        /// Helper function for Interval Input Box
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="newInterval"></param>
        /// <returns></returns>
        private static IntervalFormat ConvertTime(long interval, out int newInterval)
        {
            IntervalFormat format = IntervalFormat.IntervalHour;
            if(interval >= 86400000)
            {
                newInterval = (int)(interval / 86400000);
                format = IntervalFormat.IntervalDay;
            }
            else if(interval >= 3600000)
            {
                newInterval = (int)(interval / 3600000);
                format = IntervalFormat.IntervalHour;
            }
            else if(interval >= 60000)
            {
                newInterval = (int)(interval / 60000);
                format = IntervalFormat.IntervalMinute;
            }
            else
            {
                newInterval = (int)(interval / 1000);
                format = IntervalFormat.IntervalSecond;
            }
            return format;
        }

        /// <summary>
        /// Helper function for Interval Input Box
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static long ConvertBackTime(int interval, IntervalFormat format)
        {
            long result = (long)interval;
            switch(format)
            {
                case IntervalFormat.IntervalDay:
                    result *= 86400000;
                    break;
                case IntervalFormat.IntervalHour:
                    result *= 3600000;
                    break;
                case IntervalFormat.IntervalMinute:
                    result *= 60000;
                    break;
                case IntervalFormat.IntervalSecond:
                    result *= 1000;
                    break;
            }
            return result;
        }

        private static void Txtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}