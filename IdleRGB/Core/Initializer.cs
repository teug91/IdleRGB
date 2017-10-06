using CUE.NET;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Exceptions;
using System;
using System.Diagnostics;
using System.Timers;

namespace IdleRGB.Core
{
    internal class Initializer
    {
        private Timer initializationTimer;
        public event EventHandler<EventArgs> NewCorsairDeviceConnected;

        /// <summary>
        /// Number of Corsair USB devices, CUE supported or not.
        /// </summary>
        private int numDevices = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Initializer" /> class.
        /// </summary>
        internal Initializer()
        {
            CueSDK.PossibleX86NativePaths.Add(AppDomain.CurrentDomain.BaseDirectory + @"x86\CUESDK_2015.dll");
            CueSDK.PossibleX64NativePaths.Add(AppDomain.CurrentDomain.BaseDirectory + @"x64\CUESDK_2015.dll");

            initializationTimer = new Timer(1);
            initializationTimer.Elapsed += CheckForCue;
            initializationTimer.Enabled = true;
            GC.KeepAlive(initializationTimer);
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
                NewCorsairDeviceConnected?.Invoke(this, new EventArgs());

                if (CueSDK.IsSDKAvailable(CorsairDeviceType.Keyboard))
                    numDevices++;
                if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mouse))
                    numDevices++;
                if (CueSDK.IsSDKAvailable(CorsairDeviceType.Headset))
                    numDevices++;
                if (CueSDK.IsSDKAvailable(CorsairDeviceType.Mousemat))
                    numDevices++;

                initializationTimer.Enabled = false;
                initializationTimer.Elapsed -= InitializeSDK;
                initializationTimer.Elapsed += CheckForDevice;
                initializationTimer.Enabled = true;
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


        /// <summary>
        /// Checks for new Corsair USB devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForDevice(object sender, ElapsedEventArgs e)
        {
            int newNumDevices = CueSDK.CorsairGetDeviceCount();

            if(numDevices < newNumDevices)
            {
                CueSDK.Initialize();
                NewCorsairDeviceConnected?.Invoke(this, new EventArgs());
            }

            numDevices = newNumDevices;
        }
    }
}
