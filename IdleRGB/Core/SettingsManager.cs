using IdleRGB.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;

namespace IdleRGB.Core
{
    internal static class SettingsManager
    {
        /// <summary>
        /// Gets settings.
        /// </summary>
        /// <returns>Tuple of idle time, idle color, and caps color.</returns>
        internal static Tuple<TimeSpan, Color, Color> GetSettings()
        {
            return new Tuple<TimeSpan, Color, Color>(Settings.Default.idleTime, Settings.Default.idleColor, Settings.Default.capsColor); ;
        }

        /// <summary>
        /// Gets caps Color.
        /// </summary>
        /// <returns></returns>
        internal static Color GetCapsColor()
        {
            return Settings.Default.capsColor;
        }

        /// <summary>
        /// Saves new settings.
        /// </summary>
        /// <param name="idleTime"></param>
        /// <param name="idleColor"></param>
        /// <param name="capsColor"></param>
        /// <param name="autoStart"></param>
        internal static void SaveSettings(TimeSpan idleTime, Color idleColor, Color capsColor, bool? autoStart)
        {
            if (idleTime != Settings.Default.idleTime)
                Settings.Default.idleTime = idleTime;

            if (idleColor != Settings.Default.idleColor)
                Settings.Default.idleColor = idleColor;

            if (capsColor != Settings.Default.capsColor)
                Settings.Default.capsColor = capsColor;

            if (autoStart != null)
                SetAutoStart(autoStart);

            Settings.Default.Save();
        }

        /// <summary>
        /// Gets autostart setting.
        /// </summary>
        /// <returns>True if activated, null if no access to registry.</returns>
        internal static bool? GetAutoStart()
        {
            try
            {
                RegistryPermission perm1 = new RegistryPermission(RegistryPermissionAccess.Write, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                perm1.Demand();
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (key.GetValue(Process.GetCurrentProcess().ProcessName) != null)
                {
                    if ((string)key.GetValue(Process.GetCurrentProcess().ProcessName) != Process.GetCurrentProcess().MainModule.FileName)
                        SetAutoStart(true);
                    return true;
                }
                else
                    return false;
            }

            // No registry access.
            catch (System.Security.SecurityException)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets autostart setting.
        /// </summary>
        /// <param name="autoStart">Activate or deactivate.</param>
        private static void SetAutoStart(bool? autoStart)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (autoStart == true)
                    key.SetValue(Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().MainModule.FileName);
                else
                    key.DeleteValue(Process.GetCurrentProcess().ProcessName, false);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
