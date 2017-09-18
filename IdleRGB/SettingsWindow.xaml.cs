using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using IdleRGB.Properties;
using Application = System.Windows.Forms.Application;

namespace IdleRGB
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ToolWindow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsWindow" /> class.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();

            InitializeComboBox(hoursComboBox, 23);
            InitializeComboBox(minutesComboBox, 59);
            InitializeComboBox(secondsComboBox, 59);

            LoadSettings();
        }

        /// <summary>
        ///     Changes checkbox values to current idleTime.
        /// </summary>
        private void LoadSettings()
        {
            var it = Settings.Default.idleTime;

            hoursComboBox.SelectedItem = it.Hours;
            minutesComboBox.SelectedItem = it.Minutes;
            secondsComboBox.SelectedItem = it.Seconds;
        }

        /// <summary>
        ///     Initializes ComboBox.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="upperLimit"></param>
        private void InitializeComboBox(ComboBox comboBox, int upperLimit)
        {
            for (var i = 0; i <= upperLimit; i++)
                comboBox.Items.Add(i);
        }

        /// <summary>
        ///     Restarts application with new idleTime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var h = (int) hoursComboBox.SelectedItem;
            var m = (int) minutesComboBox.SelectedItem;
            var s = (int) secondsComboBox.SelectedItem;

            var it = new TimeSpan(h, m, s);

            if (!it.Equals(new TimeSpan(0, 0, 0)))
            {
                Settings.Default.idleTime = it;
                Settings.Default.Save();

                Debug.WriteLine("idleTime = " + Settings.Default.idleTime);

                Close();
                Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }
        }


        /// <summary>
        /// Closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}