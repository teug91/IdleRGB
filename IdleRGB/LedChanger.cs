using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Headset;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Mouse;
using CUE.NET.Devices.Mousemat;
using CUE.NET.Exceptions;
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

        Timer initializationTimer;
        bool isCueReady = false;

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

            InitializeSDK();

            // Continues to look for CUE and adds devices.
            initializationTimer = new Timer(10000);
            initializationTimer.Elapsed += Timer1_Tick;
            initializationTimer.Enabled = true;
            GC.KeepAlive(initializationTimer);
        }


        /// <summary>
        ///     Attempts to initialize CUE SDK or looks for new devices.
        /// </summary>
        private void InitializeSDK()
        {
            if (CueSDK.IsSDKAvailable() && !isCueReady)
            {
                isCueReady = true;
            }

            else if (CueSDK.IsSDKAvailable() && isCueReady)
            {
                try
                {
                    CueSDK.Initialize();
                    initializationTimer.Stop();
                    initializationTimer.Dispose();
                }

                catch (WrapperException e)
                {
                    Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                }

                catch (CUEException e)
                {
                    Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), e.Error));

                }
            }
        }

        /// <summary>
        ///     Changes all LED colors with the exception of possible Media, Brightness, WinLock LEDs.
        /// </summary>
        /// <param name="newColor">The new <see cref="System.Drawing.Color" />.</param>
        public void ChangeLeds(Color newColor)
        {
            if (CueSDK.IsInitialized)
            {
                // No changes are done to these LEDs.
                List<CorsairLedId> skipLeds = new List<CorsairLedId>();
                skipLeds.Add(CorsairLedId.Stop);
                skipLeds.Add(CorsairLedId.ScanPreviousTrack);
                skipLeds.Add(CorsairLedId.PlayPause);
                skipLeds.Add(CorsairLedId.ScanNextTrack);
                skipLeds.Add(CorsairLedId.Mute);
                skipLeds.Add(CorsairLedId.Brightness);
                skipLeds.Add(CorsairLedId.WinLock);

                var initializedDevices = CueSDK.InitializedDevices.GetEnumerator();

                while (initializedDevices.MoveNext())
                {
                    try
                    {
                        var leds = initializedDevices.Current.GetEnumerator();

                        while (leds.MoveNext())
                        {
                            if (!skipLeds.Contains(leds.Current.Id))
                            {
                                leds.Current.Color = newColor;
                            }
                        }

                        initializedDevices.Current.Update();
                    }

                    catch (WrapperException e)
                    {
                        Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
                    }

                    catch (CUEException e)
                    {
                        Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), e.Error));
                    }
                }
            }
        }

        /// <summary>
        ///     Releases control back to Corsair CUE.
        /// </summary>
        public void ResetLeds()
        {
            try
            {
                if (CueSDK.IsInitialized)
                    CueSDK.Reinitialize();
            }

            catch (WrapperException e)
            {
                Debug.WriteLine("Wrapper Exception! Message:" + e.Message);
            }

            catch (CUEException e)
            {
                Debug.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), e.Error));
                
            }
        }

        /// <summary>
        ///     Try to reinitialize SDK or look for new devices.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.</param>
        private void Timer1_Tick(object sender, ElapsedEventArgs e)
        {
            InitializeSDK();
        }
    }
}