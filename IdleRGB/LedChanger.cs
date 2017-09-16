using System;
using System.Diagnostics;
using CUE.NET;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Exceptions;
using System.Drawing;
using CUE.NET.Devices.Mouse.Enums;
using CUE.NET.Brushes;
using CUE.NET.Devices.Keyboard.Enums;
using CUE.NET.Groups;
using CUE.NET.Devices.Generic.Enums;
using System.Windows;
using CUE.NET.Devices.Generic;
using System.Collections;
using System.Collections.Generic;

namespace IdleRGB
{
    class LedChanger
    {
        CorsairKeyboard corsairKeyboard;
        CorsairMouse corsairMouse;

        // Color settings.
        Color stopColor;
        Color prevColor;
        Color playPauseColor;
        Color nextColor;
        Color muteColor;
        bool enableKeyboard;
        bool enableMouse;
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

            enableKeyboard = Properties.Settings.Default.enableKeyBoard;
            enableMouse = Properties.Settings.Default.enableMouse;
            enableMedia = Properties.Settings.Default.enableMedia;

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
            }

            catch (CUEException e)
            {
                Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), e.Error));
            }
        }

        private void InitializeKeyboard()
        {
            try
            {
                corsairKeyboard = CueSDK.KeyboardSDK;

                if (corsairKeyboard == null)
                {
                    enableKeyboard = false;
                    throw new WrapperException("No keyboard found");
                }

                else
                    enableKeyboard = true;

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

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }

        private void InitializeMouse()
        {
            try
            {
                corsairMouse = CueSDK.MouseSDK;

                if (corsairMouse == null)
                {
                    enableMouse = false;
                    throw new WrapperException("No Mouse found");

                }

                else
                    enableMouse = true;
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }
        }

        /// <summary>
        /// Changes all LED colors except media.
        /// </summary>
        /// <param name="backgroundColor">The <see cref="System.Drawing.Color"/> for the background.</param>
        public void ChangeLeds(Color backgroundColor)
        {
            try
            {
                if (enableKeyboard)
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

                if (enableMouse)
                {
                    IEnumerator<CorsairLed> mouseLeds = corsairMouse.GetEnumerator();

                    while (mouseLeds.MoveNext())
                        mouseLeds.Current.Color = backgroundColor;

                    corsairMouse.Update();
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        /// <summary>
        /// Resets all LEDs.
        /// </summary>
        public void ResetLeds()
        {
            CueSDK.Reinitialize();
        }
    }
}
