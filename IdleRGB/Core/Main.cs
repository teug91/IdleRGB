using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows.Input;
using IdleRGB.Properties;
using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Mousemat;
using CUE.NET.Exceptions;

namespace IdleRGB
{
    /// <summary>
    ///     Checks for input.
    /// </summary>
    internal class Main
    {
        private Color capsColor;
        private Color idleColor;
        private KeyboardInput keyboard;
        private MouseInput mouse;

        private TimeSpan idleTime;

        private bool inCaps;
        private bool inIdle;

        private DateTime lastInput;
        private Timer initializationTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Main" /> class.
        /// </summary>
        public Main()
        {
            lastInput = DateTime.Now;
            idleTime = Settings.Default.idleTime;
            Settings.Default.SettingsSaving += SettingSaving;

            keyboard = new KeyboardInput();
            keyboard.KeyBoardKeyPressed += InputAction;

            mouse = new MouseInput();
            mouse.MouseMoved += InputAction;

            CueSDK.PossibleX86NativePaths.Add(AppDomain.CurrentDomain.BaseDirectory + @"x86\CUESDK_2015.dll");
            CueSDK.PossibleX64NativePaths.Add(AppDomain.CurrentDomain.BaseDirectory + @"x64\CUESDK_2015.dll");

            initializationTimer = new Timer(1);
            initializationTimer.Elapsed += CheckForCue;
            initializationTimer.Enabled = true;
            GC.KeepAlive(initializationTimer);

            idleColor = Settings.Default.idleColor;
            capsColor = Settings.Default.capsColor;

            if (Keyboard.IsKeyToggled(Key.CapsLock))
            {
                LedChanger.ChangeLeds(capsColor);
                inCaps = true;
            }

            InitTimer();
        }

        /// <summary>
        ///     Updates last input and changes LEDs if needed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void InputAction(object sender, EventArgs e)
        {
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
        private void Timer1_Tick(object sender, ElapsedEventArgs e)
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
            timer.Elapsed += Timer1_Tick;
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
            if (idleTime != Settings.Default.idleTime)
                idleTime = Settings.Default.idleTime;

            if (idleColor != Settings.Default.idleColor)
                idleColor = Settings.Default.idleColor;

            if (capsColor != Settings.Default.capsColor)
                capsColor = Settings.Default.capsColor;

            if (inCaps)
                LedChanger.ChangeLeds(capsColor);
            else
                LedChanger.ResetLeds();
        }

        /// <summary>
        ///     If SDK is available, initializes after 5 seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForCue(object sender, ElapsedEventArgs e)
        {
            if (CueSDK.IsSDKAvailable())
            {
                initializationTimer.Enabled = false;
                initializationTimer.Elapsed += InitializeSDK;
                initializationTimer.Elapsed -= CheckForCue;
                initializationTimer.Interval = 5000;
                initializationTimer.Enabled = true;
            }
        }

        /// <summary>
        ///     Attempts to initialize SDK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializeSDK(object sender, ElapsedEventArgs e)
        {
            try
            {
                CueSDK.Initialize();
                initializationTimer.Stop();
                initializationTimer.Dispose();
            }

            catch (WrapperException ex)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + ex.Message);
            }

            catch (CUEException ex)
            {
                Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));

            }
        }
    }
}