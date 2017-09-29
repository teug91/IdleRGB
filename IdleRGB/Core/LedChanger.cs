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
    internal static class LedChanger
    {
        /// <summary>
        ///     Changes all LED colors with the exception of possible Media, Brightness, WinLock LEDs.
        /// </summary>
        /// <param name="newColor">The new <see cref="System.Drawing.Color" />.</param>
        public static void ChangeLeds(Color newColor)
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
        public static void ResetLeds()
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
    }
}