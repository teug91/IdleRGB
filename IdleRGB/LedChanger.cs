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
        void InitializeCueSDK()
        {
            try
            {
                CueSDK.Initialize();
                corsairKeyboard = CueSDK.KeyboardSDK;
                corsairMouse = CueSDK.MouseSDK;

                if (corsairKeyboard == null)
                {
                    throw new WrapperException("No keyboard found");

                }

                if (corsairMouse == null)
                {
                    throw new WrapperException("No mouse found");

                }
            }

            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// Changes all LED colors except media.
        /// </summary>
        /// <param name="backgroundColor">The <see cref="System.Drawing.Color"/> for the background.</param>
        public void ChangeLeds(Color backgroundColor)
        {
            corsairKeyboard.Brush = new SolidColorBrush(backgroundColor);

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

            corsairKeyboard.Update();

            corsairMouse[CorsairMouseLedId.B1].Color = backgroundColor;
            corsairMouse[CorsairMouseLedId.B2].Color = backgroundColor;
            corsairMouse[CorsairMouseLedId.B3].Color = backgroundColor;
            corsairMouse[CorsairMouseLedId.B4].Color = backgroundColor;

            corsairMouse.Update();
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
