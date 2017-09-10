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
            hoursComboBox.SelectedItem = it.Hours;
        }

        private void InitializeComboBox(ComboBox comboBox, int upperLimit)
        {
            for (int i = 0; i <= upperLimit; i++)
                comboBox.Items.Add(i);
        }
    }
}
