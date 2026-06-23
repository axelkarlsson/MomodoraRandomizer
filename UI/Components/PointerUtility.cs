using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;

internal static class PointerUtility
{    
    /// <summary>
        /// Create a DeepPointer for the process with the offsets (includes the base)<br/>
        /// </summary>
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

    /// <summary>
    /// Writes a value of a specified type to the memory of a target process at the given memory address.
    /// </summary>
    /// <typeparam name="T">The type of the value to write. Must be a value type (struct).</typeparam>
    /// <param name="process">The target process to write the value to.</param>
    /// <param name="pointer">The memory address where the value will be written.</param>
    /// <param name="value">The value to be written to the process's memory.</param>
    /// <returns>
    ///     <c>true</c> if the value was successfully written to the process's memory;<br/>
    ///     <c>false</c> if the process is null or if the value type is not a valid struct.
    /// </returns>
    public static bool WriteValue<T>(Process process, IntPtr pointer, T value) where T : struct
    {
        if (process != null && !process.HasExited)
        {
            process.WriteValue<T>(pointer, value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Reads a value of a specified type from the memory of a target process at the given memory address.
    /// </summary>
    /// <typeparam name="T">The type of the value to read. Must be a value type (struct).</typeparam>
    /// <param name="process">The target process from which to read the value.</param>
    /// <param name="pointer">The memory address from which the value will be read.</param>
    /// <returns>
    ///     The value read from the process's memory, or the default value of type T if the process is null or if the value type is not a valid struct.
    /// </returns>
    public static T ReadValue<T>(Process process, IntPtr pointer) where T : struct => process?.ReadValue<T>(pointer) ?? default;
}
