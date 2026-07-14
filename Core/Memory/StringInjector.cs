using System;
using System.Runtime.InteropServices;

namespace MomodoraRandomizer.Core.Memory {
    internal class StringInjector {
        public static IntPtr AllocateAnsiString(string text) {
            // Allocates unmanaged memory and copies ANSI bytes + null terminator
            return Marshal.StringToHGlobalAnsi(text);
        }

        public static void FreeAnsiString(IntPtr ptr) {
            if (ptr != IntPtr.Zero) {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
