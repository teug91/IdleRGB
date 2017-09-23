using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using IdleRGB.Properties;
using System.Windows.Media;
using System.Security.Permissions;
using CUE.NET;
using CUE.NET.Devices.Generic;
using System.Windows.Input;

namespace IdleRGB
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ToolWindow
    {
        bool autoStart;
        Color newInactiveColor;
        Color newCapsColor;


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

            Color color = Color.FromRgb(Settings.Default.idleColor.R, Settings.Default.idleColor.G, Settings.Default.idleColor.B);
            inactiveRectangle.Fill = new SolidColorBrush(color);

            color = Color.FromRgb(Settings.Default.capsColor.R, Settings.Default.capsColor.G, Settings.Default.capsColor.B);
            capsLockRectangle.Fill = new SolidColorBrush(color);

            InitializeAutoStartCheckbox();
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
        ///     Saves settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan newTime = GetNewTime();

            if (Settings.Default.idleTime != newTime)
                Settings.Default.idleTime = newTime;
                Settings.Default.Save();

            if (autostartCheckbox.IsChecked != autoStart)
            {
                try
                {
                    autoStart = (bool)autostartCheckbox.IsChecked;

                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                    if (autoStart)
                        key.SetValue(Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().MainModule.FileName);
                    else
                        key.DeleteValue(Process.GetCurrentProcess().ProcessName, false);
                }

                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            Color color = Color.FromRgb(Settings.Default.idleColor.R, Settings.Default.idleColor.R, Settings.Default.idleColor.B);

            if(color != newInactiveColor)
                Settings.Default.idleColor = System.Drawing.Color.FromArgb(newInactiveColor.R, newInactiveColor.G, newInactiveColor.G);

            if(color != newCapsColor)
                Settings.Default.capsColor = System.Drawing.Color.FromArgb(newCapsColor.R, newCapsColor.G, newCapsColor.G);

            Settings.Default.Save();
            Close();
        }


        /// <summary>
        ///     Closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Removes autoStartCheckbox if registry is unavailable. Otherwise sets appropriate check value.
        /// </summary>
        private void InitializeAutoStartCheckbox()
        {
            try
            {
                RegistryPermission perm1 = new RegistryPermission(RegistryPermissionAccess.Write, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                perm1.Demand();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (key.GetValue(Process.GetCurrentProcess().ProcessName) != null)
                    autostartCheckbox.IsChecked = autoStart = true;
                else
                    autostartCheckbox.IsChecked = autoStart = false;
            }

            // No registry access.
            catch (System.Security.SecurityException)
            {
                autostartCheckbox.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        ///     Gets new time from checkboxes.
        /// </summary>
        /// <returns>Time from checkboxes.</returns>
        private TimeSpan GetNewTime()
        {
            var h = (int)hoursComboBox.SelectedItem;
            var m = (int)minutesComboBox.SelectedItem;
            var s = (int)secondsComboBox.SelectedItem;

            return new TimeSpan(h, m, s);
        }

        /// <summary>
        ///     Color picker for inactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inactiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!colorCanvas.IsVisible)
                colorCanvas.Visibility = Visibility.Visible;

            else
                setCapsButton.Visibility = Visibility.Hidden;

            Color color = Color.FromRgb(Settings.Default.idleColor.R, Settings.Default.idleColor.G, Settings.Default.idleColor.B);
            colorCanvas.SelectedColor = color;
            setInactiveButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Color picker for caps lock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CapsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!colorCanvas.IsVisible)
                colorCanvas.Visibility = Visibility.Visible;

            else
                setInactiveButton.Visibility = Visibility.Hidden;

            Color color = Color.FromRgb(Settings.Default.capsColor.R, Settings.Default.capsColor.G, Settings.Default.capsColor.B);
            colorCanvas.SelectedColor = color;
            setCapsButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Sets LEDs to selected color for preview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(CueSDK.IsInitialized)
            {
                CorsairColor color = new CorsairColor(colorCanvas.R, colorCanvas.G, colorCanvas.B);
                LedChanger.ChangeLeds(color);
            }
        }

        /// <summary>
        ///     Sets LEDs back from preview color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetInactiveButton_Click(object sender, RoutedEventArgs e)
        {
            setInactiveButton.Visibility = Visibility.Hidden;
            colorCanvas.Visibility = Visibility.Hidden;

            newInactiveColor = Color.FromRgb(colorCanvas.R, colorCanvas.G, colorCanvas.B);
            inactiveRectangle.Fill = new SolidColorBrush(newInactiveColor);

            var capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);
            if (capsToggled)
                LedChanger.ChangeLeds(Settings.Default.capsColor);
            else
                LedChanger.ResetLeds();
        }

        /// <summary>
        ///     Sets LEDs back from preview color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCapsButton_Click(object sender, RoutedEventArgs e)
        {
            setCapsButton.Visibility = Visibility.Hidden;
            colorCanvas.Visibility = Visibility.Hidden;

            newCapsColor = Color.FromRgb(colorCanvas.R, colorCanvas.G, colorCanvas.B);
            capsLockRectangle.Fill = new SolidColorBrush(newCapsColor);

            var capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);
            if (capsToggled)
                LedChanger.ChangeLeds(Settings.Default.capsColor);
            else
                LedChanger.ResetLeds();
        }
    }
}