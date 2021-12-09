namespace Orc.Skia
{
    using System;
    using System.Runtime.InteropServices;

    internal class Kernel32
    {
        #region Constants
        private const string kernel32 = "kernel32.dll";
        #endregion

        #region Constructors
        static Kernel32()
        {
            CurrentModuleHandle = Kernel32.GetModuleHandle(null);
            if (CurrentModuleHandle == IntPtr.Zero)
            {
                throw new Exception("Could not get module handle.");
            }
        }
        #endregion

        #region Properties
        public static IntPtr CurrentModuleHandle { get; }
        #endregion

        #region Methods
        [DllImport(kernel32, CallingConvention = CallingConvention.Winapi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPTStr)] string lpModuleName);
        #endregion
    }
}
