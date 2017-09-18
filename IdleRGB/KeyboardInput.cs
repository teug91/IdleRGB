using System;

namespace IdleRGB
{
    public class KeyboardInput : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private bool disposed;

        private readonly WindowsHookHelper.HookDelegate keyBoardDelegate;
        private readonly IntPtr keyBoardHandle;

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyboardInput" /> class.
        /// </summary>
        public KeyboardInput()
        {
            keyBoardDelegate = KeyboardHookDelegate;
            keyBoardHandle = WindowsHookHelper.SetWindowsHookEx(
                WH_KEYBOARD_LL, keyBoardDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public event EventHandler<EventArgs> KeyBoardKeyPressed;

        private IntPtr KeyboardHookDelegate(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return WindowsHookHelper.CallNextHookEx(
                    keyBoardHandle, code, wParam, lParam);

            KeyBoardKeyPressed?.Invoke(this, new EventArgs());

            return WindowsHookHelper.CallNextHookEx(
                keyBoardHandle, code, wParam, lParam);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (keyBoardHandle != IntPtr.Zero)
                    WindowsHookHelper.UnhookWindowsHookEx(
                        keyBoardHandle);

                disposed = true;
            }
        }

        ~KeyboardInput()
        {
            Dispose(false);
        }
    }
}