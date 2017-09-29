using System;

namespace IdleRGB
{
    public class MouseInput : IDisposable
    {
        private const int WH_MOUSE_LL = 14;

        private readonly WindowsHookHelper.HookDelegate mouseDelegate;
        private readonly IntPtr mouseHandle;

        private bool disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MouseInput" /> class.
        /// </summary>
        public MouseInput()
        {
            mouseDelegate = MouseHookDelegate;
            mouseHandle = WindowsHookHelper.SetWindowsHookEx(WH_MOUSE_LL, mouseDelegate, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public event EventHandler<EventArgs> MouseMoved;

        private IntPtr MouseHookDelegate(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return WindowsHookHelper.CallNextHookEx(mouseHandle, code, wParam, lParam);

            MouseMoved?.Invoke(this, new EventArgs());

            return WindowsHookHelper.CallNextHookEx(mouseHandle, code, wParam, lParam);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (mouseHandle != IntPtr.Zero)
                    WindowsHookHelper.UnhookWindowsHookEx(mouseHandle);

                disposed = true;
            }
        }

        ~MouseInput()
        {
            Dispose(false);
        }
    }
}