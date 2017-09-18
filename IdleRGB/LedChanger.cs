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

        private bool enableMedia;
        private bool headsetConnected;
        private bool keyboardConnected;
        private bool mouseConnected;
        private bool mousematConnected;

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

            InitializeCueSDK();
        }

        /// <summary>
        ///     Initializes CUE SDK.
        /// </summary>
        private void InitializeCueSDK()
        {
            try
            {
                CueSDK.Initialize();
                InitializeKeyboard();
                InitializeMouse();
                InitializeHeadset();
                InitializeMousemat();
            }

            catch (CUEException e)
            {
                Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), e.Error));
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
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

                    // Checking if keyboard has media LEDs.
                    switch (corsairKeyboard.DeviceInfo.Model)
                    {
                        case "K70 RGB":
                            enableMedia = true;
                            break;
                        case "K95 RGB":
                            enableMedia = true;
                            break;
                        default:
                            enableMedia = false;
                            break;
                    }
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
                CueSDK.Reinitialize();
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }
    }
}