using System;
using System.Diagnostics;
using System.Timers;
using System.IO;
using System.Drawing;
using System.Windows.Input;

namespace IdleRGB
{
    class Input
    {
        KeyboardInput keyboard;
        MouseInput mouse;
        LedChanger idle;

        DateTime lastInput;

        // Settings
        TimeSpan idleTime;
        Color idleColor;
        Color capsColor;

        bool inIdle = false;
        bool inCaps = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
        /// </summary>
        public Input()
        {
            lastInput = DateTime.Now;
            idleTime = Properties.Settings.Default.idleTime;

            keyboard = new KeyboardInput();
            keyboard.KeyBoardKeyPressed += InputAction;

            mouse = new MouseInput();
            mouse.MouseMoved += InputAction;

            idle = new LedChanger();

            idleColor = Properties.Settings.Default.idleColor;
            capsColor = Properties.Settings.Default.capsColor;

            InitTimer();
        }

        /// <summary>
        /// Updates last input and deactivates idle.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InputAction(object sender, EventArgs e)
        {
            bool capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);

            // Checking if in idle
            if (inIdle)
            {
                if (capsToggled)
                {
                    idle.ChangeLeds(capsColor);
                    inCaps = true;
                }

                else
                {
                    idle.ResetLeds();
                }

                inIdle = false;
            }

            else if(!capsToggled && inCaps)
            {
                idle.ResetLeds();
                inCaps = false;
            }

            else if(capsToggled && !inCaps)
            {
                idle.ChangeLeds(capsColor);
                inCaps = true;
            }

            // Updates last input time
            lastInput = DateTime.Now;
        }

        /// <summary>
        /// Checks if enough time without input has passed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        void Timer1_Tick(object sender, ElapsedEventArgs e)
        {
            if (!inIdle)
            {
                // Checks if enough time has passed to put in idle
                if (DateTime.Now.Subtract(lastInput) > idleTime)
                {
                    idle.ChangeLeds(idleColor);
                    inIdle = true;
                }
            }
        }

        /// <summary>
        /// Initiates timer.
        /// </summary>
        void InitTimer()
        {
            System.Timers.Timer timer1;
            timer1 = new System.Timers.Timer();
            timer1.Elapsed += Timer1_Tick;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            GC.KeepAlive(timer1);
        }
    }
}
