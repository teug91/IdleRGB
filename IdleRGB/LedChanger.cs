using System;
using System.Diagnostics;
using System.Drawing;
using CUE.NET;
using CUE.NET.Brushes;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Enums;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Mousemat;
using CUE.NET.Exceptions;
using CUE.NET.Groups;
using IdleRGB.Properties;
using System.Timers;

namespace IdleRGB
{
    internal class LedChanger
    {
        // Color settings.
        private readonly Color muteColor;
        private readonly Color nextColor;
        private readonly Color playPauseColor;
        private readonly Color prevColor;
        private readonly Color stopColor;

        private CorsairHeadset corsairHeadset;
        private CorsairKeyboard corsairKeyboard;
        private CorsairMouse corsairMouse;
        private CorsairMousemat corsairMousemat;

        private bool enableMedia = false;
        private bool headsetConnected;
        private bool keyboardConnected;
        private bool mouseConnected;
        private bool mousematConnected;

        Timer timer1;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LedChanger" /> class.
        /// </summary>
        public LedChanger()
        {
            stopColor = Settings.Default.stopColor;
            prevColor = Settings.Default.nextColor;
            playPauseColor = Settings.Default.playPauseColor;
            nextColor = Settings.Default.nextColor;
            muteColor = Settings.Default.muteColor;

            if(!InitializeSDK())
            {
                timer1 = new Timer(5000);
                timer1.Elapsed += Timer1_Tick;
                timer1.Enabled = true;
                GC.KeepAlive(timer1);
            }
        }


        /// <summary>
        ///     Attempts to initialize CUE SDK and connected periferal.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        private bool InitializeSDK()
        {
            if (CueSDK.IsSDKAvailable())
            {
                CueSDK.Initialize();
                InitializeKeyboard();
                InitializeMouse();
                InitializeHeadset();
                InitializeMousemat();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes keyboard, if connected.
        /// </summary>
        private void InitializeKeyboard()
        {
            try
            {
                if (CueSDK.IsSDKAvailable(CorsairDeviceType.Keyboard))
                {
                    corsairKeyboard = CueSDK.KeyboardSDK;

                    if (corsairKeyboard == null)
                    {
                        keyboardConnected = false;
                        throw new WrapperException("No keyboard found");
                    }

                    keyboardConnected = true;

                    // Checking if keyboard has media buttons.
                    if (corsairKeyboard[CorsairKeyboardLedId.ScanPreviousTrack] != null)
                        enableMedia = true;
                }

                else
                {
                    keyboardConnected = false;
                }
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }

        /// <summary>
        /// Initializes mouse, if connected.
        /// </summary>
        private void InitializeMouse()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mouse))
                try
                {
                    corsairMouse = CueSDK.MouseSDK;

                    if (corsairMouse == null)
                    {
                        mouseConnected = false;
                        throw new WrapperException("No Mouse found");
                    }

                    mouseConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }

            else
                mouseConnected = false;
        }

        /// <summary>
        /// Initializes headset, if connected.
        /// </summary>
        private void InitializeHeadset()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Headset))
                try
                {
                    corsairHeadset = CueSDK.HeadsetSDK;

                    if (corsairHeadset == null)
                    {
                        headsetConnected = false;
                        throw new WrapperException("No Headset found");
                    }

                    headsetConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }

            else
                headsetConnected = false;
        }

        /// <summary>
        /// Initializes mousemat, if connected.
        /// </summary>
        private void InitializeMousemat()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mousemat))
                try
                {
                    corsairMousemat = CueSDK.MousematSDK;

                    if (corsairMousemat == null)
                    {
                        mousematConnected = false;
                        throw new WrapperException("No Mousemat found");
                    }

                    mousematConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }

            else
                mousematConnected = false;
        }

        /// <summary>
        ///     Changes all LED colors with the exception of possible media LEDs.
        /// </summary>
        /// <param name="backgroundColor">The <see cref="System.Drawing.Color" /> for the background.</param>
        public void ChangeLeds(Color backgroundColor)
        {
            try
            {
                if (CueSDK.IsInitialized)
                {
                    if (keyboardConnected)
                    {
                        corsairKeyboard.Brush = new SolidColorBrush(backgroundColor);

                        if (enableMedia)
                        {
                            var stop = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.Stop);
                            stop.Brush = new SolidColorBrush(stopColor);

                            var scanPreviousTrack = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.ScanPreviousTrack);
                            scanPreviousTrack.Brush = new SolidColorBrush(prevColor);

                            var playPause = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.PlayPause);
                            playPause.Brush = new SolidColorBrush(playPauseColor);

                            var scanNextTrack = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.ScanNextTrack);
                            scanNextTrack.Brush = new SolidColorBrush(nextColor);

                            var mute = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.Mute);
                            mute.Brush = new SolidColorBrush(muteColor);
                        }

                        corsairKeyboard.Update();
                    }

                    if (mouseConnected)
                    {
                        var mouseLeds = corsairMouse.GetEnumerator();

                        while (mouseLeds.MoveNext())
                            mouseLeds.Current.Color = backgroundColor;

                        corsairMouse.Update();
                    }

                    if (headsetConnected)
                    {
                        var headsetLeds = corsairHeadset.GetEnumerator();

                        while (headsetLeds.MoveNext())
                            headsetLeds.Current.Color = backgroundColor;

                        corsairMouse.Update();
                    }

                    if (mousematConnected)
                    {
                        var mousematLeds = corsairMousemat.GetEnumerator();

                        while (mousematLeds.MoveNext())
                            mousematLeds.Current.Color = backgroundColor;

                        corsairMouse.Update();
                    }
                }
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }

        /// <summary>
        ///     Releases control back to Corsair CUE.
        /// </summary>
        public void ResetLeds()
        {
            try
            {
                if(CueSDK.IsInitialized)
                    CueSDK.Reinitialize();
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }

        /// <summary>
        ///     Tries to initialize CUE SDKc# while 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void Timer1_Tick(object sender, ElapsedEventArgs e)
        {
            if (InitializeSDK())
            {
                timer1.Stop();
            }
        }
    }
}