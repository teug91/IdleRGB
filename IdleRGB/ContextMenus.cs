using System;
using System.Windows.Forms;
using IdleRGB.Properties;

namespace IdleRGB
{
    /// <summary>
    /// 
    /// </summary>
    class ContextMenus
    {
        SettingsWindow settingsWindow;
        public event EventHandler<EventArgs> ExitPressed;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create()
        {
            // Add the menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            // Settings.
            item = new ToolStripMenuItem();
            item.Text = "Settings";
            item.Click += new EventHandler(Settings_Click);
            item.Image = Resources.time;
            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            item.Image = Resources.exit;
            menu.Items.Add(item);

            settingsWindow = new SettingsWindow();

            //menu.ForeColor = System.Drawing.Color.Red;

            return menu;
        }

        /// <summary>
        /// Handles the Click event of the Settings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Settings_Click(object sender, EventArgs e)
        {
            if (settingsWindow.Visibility == System.Windows.Visibility.Hidden)
            {
                settingsWindow.LoadSettings();
                settingsWindow.Show();
            }
        }

        /// <summary>
        /// Handles the Click event of the Exit control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            // Quit application.
            ExitPressed(this, new EventArgs());
            System.Windows.Application.Current.Shutdown();
        }
    }
}