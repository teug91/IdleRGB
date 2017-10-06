using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CUE.NET;
using CUE.NET.Devices.Generic;
using System.Windows.Input;
using IdleRGB.Core;

namespace IdleRGB
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ToolWindow
    {
        TimeSpan idleTime;
        Color idleColor;
        Color capsColor;

        public static EventHandler ReleasedControl;

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
            var settings = SettingsManager.GetSettings();

            idleTime = settings.Item1;

            hoursComboBox.SelectedItem = idleTime.Hours;
            minutesComboBox.SelectedItem = idleTime.Minutes;
            secondsComboBox.SelectedItem = idleTime.Seconds;
            
            Color color = Color.FromRgb(settings.Item2.R, settings.Item2.G, settings.Item2.B);
            
            idleRectangle.Fill = new SolidColorBrush(color);
            idleColor = color;

            color = Color.FromRgb(settings.Item3.R, settings.Item3.G, settings.Item3.B);
            capsLockRectangle.Fill = new SolidColorBrush(color);
            capsColor = color;

            bool? autoStart = SettingsManager.GetAutoStart();

            if (autoStart == null)
                autostartCheckbox.Visibility = Visibility.Hidden;
            else
                autostartCheckbox.IsChecked = autoStart;
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

            bool? autoStart = null;
            if(autostartCheckbox.Visibility == Visibility.Visible)
                autoStart = autostartCheckbox.IsChecked;

            var newIdleColor = System.Drawing.Color.FromArgb(idleColor.R, idleColor.G, idleColor.B);
            var newCapsColor = System.Drawing.Color.FromArgb(capsColor.R, capsColor.G, capsColor.B);

            SettingsManager.SaveSettings(idleTime, newIdleColor, newCapsColor, autoStart);
            Close();
        }


        /// <summary>
        ///     Closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ReleasedControl.Invoke(null, null);
            Close();
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
        ///     Color picker for idle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void idleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!colorCanvas.IsVisible)
                colorCanvas.Visibility = Visibility.Visible;

            else
                setCapsButton.Visibility = Visibility.Hidden;

            colorCanvas.SelectedColor = Color.FromRgb(idleColor.R, idleColor.G, idleColor.B);
            setIdleButton.Visibility = Visibility.Visible;
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
                setIdleButton.Visibility = Visibility.Hidden;

            colorCanvas.SelectedColor = Color.FromRgb(capsColor.R, capsColor.G, capsColor.B);
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
        private void SetIdleButton_Click(object sender, RoutedEventArgs e)
        {
            setIdleButton.Visibility = Visibility.Hidden;
            colorCanvas.Visibility = Visibility.Hidden;

            idleColor = Color.FromRgb(colorCanvas.R, colorCanvas.G, colorCanvas.B);

            idleRectangle.Fill = new SolidColorBrush(idleColor);

            ReleasedControl.Invoke(null, null);
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

            capsColor = Color.FromRgb(colorCanvas.R, colorCanvas.G, colorCanvas.B);
            capsLockRectangle.Fill = new SolidColorBrush(capsColor);

            ReleasedControl.Invoke(null, null);
        }
    }
}