using System;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Input;

namespace IdleRGB.Core
{
    /// <summary>
    ///     Checks for input.
    /// </summary>
    internal class Main
    {
        private KeyboardInput keyboard;
        private MouseInput mouse;

        private Color capsColor;
        private Color idleColor;

        private TimeSpan idleTime;
        private DateTime lastInput;

        private bool inCaps;
        private bool inIdle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Main" /> class.
        /// </summary>
        internal Main()
        {
            var settings = SettingsManager.GetSettings();
            SettingsManager.GetAutoStart();

            lastInput = DateTime.Now;
            idleTime = settings.Item1;
            idleColor = settings.Item2;
            capsColor = settings.Item3;

            Properties.Settings.Default.SettingsSaving += SettingSaving;

            keyboard = new KeyboardInput();
            keyboard.KeyBoardKeyPressed += InputAction;

            mouse = new MouseInput();
            mouse.MouseMoved += InputAction;

            new Initializer().NewCorsairDeviceConnected += UpdateNewDevice;

            SettingsWindow.ReleasedControl += RetakeControl;

            if (Keyboard.IsKeyToggled(Key.CapsLock))
            {
                LedChanger.ChangeLeds(capsColor);
                inCaps = true;
            }

            InitTimer();
        }

        /// <summary>
        /// Changes LEDs to proper color after color picker has changed it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetakeControl(object sender, EventArgs e)
        {
            var capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);

            if (capsToggled)
            {
                LedChanger.ChangeLeds(capsColor);
                inCaps = true;
            }

            else
            {
                LedChanger.ResetLeds();
                inCaps= inIdle = false;
            }
        }

        /// <summary>
        ///     Updates last input and changes LEDs if needed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void InputAction(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("INPUT");
            var capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);

            if (inIdle)
            {
                if (capsToggled)
                {
                    LedChanger.ChangeLeds(capsColor);
                    inCaps = true;
                }

                else
                {
                    LedChanger.ResetLeds();
                }

                inIdle = false;
            }

            else if (!capsToggled && inCaps)
            {
                LedChanger.ResetLeds();
                inCaps = false;
            }

            else if (capsToggled && !inCaps)
            {
                LedChanger.ChangeLeds(capsColor);
                //System.Diagnostics.Debug.WriteLine("GOING CAPS!");
                inCaps = true;
            }

            // Updates last input time
            lastInput = DateTime.Now;
        }

        /// <summary>
        ///     Checks if enough time without input has passed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void IdleCheck(object sender, ElapsedEventArgs e)
        {
            if (!inIdle)
            {
                if (DateTime.Now.Subtract(lastInput) > idleTime)
                {
                    LedChanger.ChangeLeds(idleColor);
                    inIdle = true;
                }
            }
        }

        /// <summary>
        ///     Initiates timer.
        /// </summary>
        private void InitTimer()
        {
            Timer timer = new Timer();
            timer.Elapsed += IdleCheck;
            timer.Interval = 1000;
            timer.Enabled = true;
            GC.KeepAlive(timer);
        }

        /// <summary>
        ///     Loading new settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingSaving(object sender, CancelEventArgs e)
        {
            var settings = SettingsManager.GetSettings();

            if (idleTime != settings.Item1)
                idleTime = settings.Item1;

            if (idleColor != settings.Item2)
                idleColor = settings.Item2;

            if (capsColor != settings.Item3)
                capsColor = settings.Item3;

            if (inCaps)
                LedChanger.ChangeLeds(capsColor);
            else
                LedChanger.ResetLeds();
        }


        /// <summary>
        /// Changes LEDs of newly connected device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateNewDevice(object sender, EventArgs e)
        {
            if(inIdle)
                LedChanger.ChangeLeds(idleColor);

            else if(inCaps)
                LedChanger.ChangeLeds(capsColor);
        }
    }
}