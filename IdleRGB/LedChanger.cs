using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using CUE.NET;
using CUE.NET.Groups;
using CUE.NET.Brushes;
using CUE.NET.Exceptions;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.Mousemat;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Enums;
using CUE.NET.Devices.Generic;
using CUE.NET.Devices.Generic.Enums;

namespace IdleRGB
{
    class LedChanger
    {
        CorsairKeyboard corsairKeyboard;
        CorsairMouse corsairMouse;
        CorsairHeadset corsairHeadset;
        CorsairMousemat corsairMousemat;

        // Color settings.
        Color stopColor;
        Color prevColor;
        Color playPauseColor;
        Color nextColor;
        Color muteColor;
        bool keyboardConnected;
        bool mouseConnected;
        bool headsetConnected;
        bool mousematConnected;
        bool enableMedia;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedChanger"/> class.
        /// </summary>
        public LedChanger()
        {
            stopColor = Properties.Settings.Default.stopColor;
            prevColor = Properties.Settings.Default.nextColor;
            playPauseColor = Properties.Settings.Default.playPauseColor;
            nextColor = Properties.Settings.Default.nextColor;
            muteColor = Properties.Settings.Default.muteColor;

            InitializeCueSDK();
        }

        /// <summary>
        /// Initializes CUE SDK.
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

                        else
                            keyboardConnected = true;

                        switch (corsairKeyboard.DeviceInfo.Model.ToString())
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
                    keyboardConnected = false;
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }


        private void InitializeMouse()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mouse))
            {
                try
                {
                    corsairMouse = CueSDK.MouseSDK;

                    if (corsairMouse == null)
                    {
                        mouseConnected = false;
                        throw new WrapperException("No Mouse found");

                    }

                    else
                        mouseConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }
            }

            else
                mouseConnected = false;
        }

        private void InitializeHeadset()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Headset))
            {
                try
                {
                    corsairHeadset = CueSDK.HeadsetSDK;

                    if (corsairHeadset == null)
                    {
                        headsetConnected = false;
                        throw new WrapperException("No Headset found");

                    }

                    else
                        headsetConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }
            }

            else
                headsetConnected = false;
        }

        private void InitializeMousemat()
        {
            if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mousemat))
            {
                try
                {
                    corsairMousemat = CueSDK.MousematSDK;

                    if (corsairMousemat == null)
                    {
                        mousematConnected = false;
                        throw new WrapperException("No Mousemat found");

                    }

                    else
                        mousematConnected = true;
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }
            }

            else
                mousematConnected = false;
        }

        /// <summary>
        /// Changes all LED colors except media.
        /// </summary>
        /// <param name="backgroundColor">The <see cref="System.Drawing.Color"/> for the background.</param>
        public void ChangeLeds(Color backgroundColor)
        {
            try
            {
                if (keyboardConnected)
                {
                    corsairKeyboard.Brush = new SolidColorBrush(backgroundColor);

                    if (enableMedia)
                    {
                        ListLedGroup stop = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.Stop);
                        stop.Brush = new SolidColorBrush(stopColor);

                        ListLedGroup scanPreviousTrack = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.ScanPreviousTrack);
                        scanPreviousTrack.Brush = new SolidColorBrush(prevColor);

                        ListLedGroup playPause = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.PlayPause);
                        playPause.Brush = new SolidColorBrush(playPauseColor);

                        ListLedGroup scanNextTrack = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.ScanNextTrack);
                        scanNextTrack.Brush = new SolidColorBrush(nextColor);

                        ListLedGroup mute = new ListLedGroup(corsairKeyboard, CorsairKeyboardLedId.Mute);
                        mute.Brush = new SolidColorBrush(muteColor);
                    }

                    corsairKeyboard.Update();
                }

                if (mouseConnected)
                {
                    IEnumerator<CorsairLed> mouseLeds = corsairMouse.GetEnumerator();

                    while (mouseLeds.MoveNext())
                        mouseLeds.Current.Color = backgroundColor;

                    corsairMouse.Update();
                }

                if(headsetConnected)
                {
                    IEnumerator<CorsairLed> headsetLeds = corsairHeadset.GetEnumerator();

                    while (headsetLeds.MoveNext())
                        headsetLeds.Current.Color = backgroundColor;

                    corsairMouse.Update();
                }

                if (mousematConnected)
                {
                    IEnumerator<CorsairLed> mousematLeds = corsairMousemat.GetEnumerator();

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
        /// Resets all LEDs.
        /// </summary>
        public void ResetLeds()
        {
            try
            {
                CueSDK.Reinitialize();
            }

            catch(WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }
    }
}
