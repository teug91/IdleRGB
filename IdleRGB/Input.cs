using System;
using System.Drawing;
using System.Timers;
using System.Windows.Input;
using IdleRGB.Properties;

namespace IdleRGB
{
    /// <summary>
    ///     Checks for input.
    /// </summary>
    internal class Input
    {
        private readonly LedChanger idle;
        private readonly Color capsColor;
        private readonly Color idleColor;

        /// <summary>
        ///     Amount of time before changing to idleColor.
        /// </summary>
        private readonly TimeSpan idleTime;

        private bool inCaps;
        private bool inIdle;

        private DateTime lastInput;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        public Input()
        {
            lastInput = DateTime.Now;
            idleTime = Settings.Default.idleTime;

            var keyboard = new KeyboardInput();
            keyboard.KeyBoardKeyPressed += InputAction;

            var mouse = new MouseInput();
            mouse.MouseMoved += InputAction;

            idle = new LedChanger();

            idleColor = Settings.Default.idleColor;
            capsColor = Settings.Default.capsColor;

            InitTimer();
        }

        /// <summary>
        ///     Updates last input and deactivates idle.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void InputAction(object sender, EventArgs e)
        {
            var capsToggled = Keyboard.IsKeyToggled(Key.CapsLock);

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

            else if (!capsToggled && inCaps)
            {
                idle.ResetLeds();
                inCaps = false;
            }

            else if (capsToggled && !inCaps)
            {
                idle.ChangeLeds(capsColor);
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
                if (DateTime.Now.Subtract(lastInput) > idleTime)
                {
                    idle.ChangeLeds(idleColor);
                    inIdle = true;
                }
        }

        /// <summary>
        ///     Initiates timer.
        /// </summary>
        private void InitTimer()
        {
            Timer timer1 = new Timer();
            timer1.Elapsed += Timer1_Tick;
            timer1.Interval = 1000;
            timer1.Enabled = true;
            GC.KeepAlive(timer1);
        }
    }
}