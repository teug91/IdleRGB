using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace IdleRGB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ToolWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            TimeSpan it = Properties.Settings.Default.idleTime;

            InitializeComboBox(hoursComboBox, 24);
            InitializeComboBox(minutesComboBox, 60);
            InitializeComboBox(secondsComboBox, 60);

            LoadSettings();
        }

        public void LoadSettings()
        {
            TimeSpan it = Properties.Settings.Default.idleTime;

            hoursComboBox.SelectedItem = it.Hours;
            minutesComboBox.SelectedItem = it.Minutes;
            secondsComboBox.SelectedItem = it.Seconds;
        }

        private void InitializeComboBox(ComboBox comboBox, int upperLimit)
        {
            for (int i = 0; i <= upperLimit; i++)
                comboBox.Items.Add(i);
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            int h = (int)hoursComboBox.SelectedItem;
            int m = (int)minutesComboBox.SelectedItem;
            int s = (int)secondsComboBox.SelectedItem;

            TimeSpan it = new TimeSpan(h, m, s);

            if (!it.Equals(new TimeSpan(0, 0, 0)))
            {
                Properties.Settings.Default.idleTime = it;
                Properties.Settings.Default.Save();

                Debug.WriteLine("idleTime = " + Properties.Settings.Default.idleTime.ToString());

                Close();
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
