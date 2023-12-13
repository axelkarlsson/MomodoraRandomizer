using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;

internal static class PointerUtility
{    
    /// <summary>
        /// Create a DeepPointer for the process with the offsets (includes the base)<br/>
        /// Supports short, int, long, double and float
        /// </summary>
        /// <typeparam name="T">Pointer type</typeparam>
        /// <param name="offsets">Offsets, including the base, of the pointer</param>
        /// <returns>
        ///     Reference to a Deep pointer<br/>
        ///     <c>zero</c> pointer otherwise
        /// </returns>
    public static IntPtr CreatePointer(Process process, params int[] offsets)
    {
        if (offsets.Length == 1)
        {
            return IntPtr.Add(process.MainModule.BaseAddress, offsets.First());
        }
        else if (offsets.Length > 1)
        {
            return IntPtr.Add((IntPtr)new DeepPointer(offsets.First(), offsets.Skip(1).Take(offsets.Length - 2).ToArray()).Deref<int>(process), offsets.Last());
        }

        return IntPtr.Zero;
    }

    public static bool WriteValue(Process process, IntPtr pointer, int value)
    {
        if (process != null)
        {
            process.WriteValue<int>(pointer, value);
            return true;
        }

        return false;
    }

    public static bool WriteValue(Process process, IntPtr pointer, double value)
    {
        if (process != null)
        {
            process.WriteValue<double>(pointer, value);
            return true;
        }

        return false;
    }

    public static bool WriteValue(Process process, IntPtr pointer, float value)
    {
        if(process != null)
        {
            process.WriteValue<float>(pointer, value);
            return true;
        }

        return false;
    }

    public static T ReadValue<T>(Process process, IntPtr pointer) where T : struct => process?.ReadValue<T>(pointer) ?? default;
}
