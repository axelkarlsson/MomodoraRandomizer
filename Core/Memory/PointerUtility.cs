using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;
using System.Linq;

namespace MomodoraRandomizer.Core.Memory {
    internal static class PointerUtility {
        /// <summary>
        ///     Create a DeepPointer for the process with the offsets (includes the base)<br/>
        /// </summary>
        /// <param name="offsets">Offsets, including the base, of the pointer</param>
        /// <returns>
        ///     Reference to a Deep pointer<br/>
        ///     <seealso cref="IntPtr.Zero"/> otherwise
        /// </returns>
        internal static IntPtr CreateIntPtr(Process process, params int[] offsets) {
            if (offsets.Length == 1) {
                return IntPtr.Add(process.MainModule.BaseAddress, offsets.First());
            } else if (offsets.Length > 1) {
                return IntPtr.Add((IntPtr)new DeepPointer(offsets.First(), offsets.Skip(1).Take(offsets.Length - 2).ToArray()).Deref<int>(process), offsets.Last());
            }

            return IntPtr.Zero;
        }

        internal static DeepPointer CreateDeepPointer(Process process, params int[] offsets) {
            if (offsets.Length == 0) {
                return null;
            }

            IntPtr basePtr = IntPtr.Add(process.MainModule.BaseAddress, offsets.First());

            if (offsets.Length == 1) {
                return new DeepPointer(basePtr);
            }

            return new DeepPointer(basePtr, offsets.Skip(1).ToArray());
        }

        /// <summary>
        ///     Writes a value of a specified type to the memory of a target process at the given memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to write. Must be a value type (struct).</typeparam>
        /// <param name="process">The target process to write the value to.</param>
        /// <param name="pointer">Deep pointer to memory where the value will be written.</param>
        /// <param name="value">The value to be written to the process's memory.</param>
        /// <returns>
        ///     <c>true</c> if the value was successfully written to the address<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        internal static bool WriteValue<T>(Process process, DeepPointer pointer, T value) where T : struct {
            if (!pointer.Deref(process, out IntPtr address)) {
                return false;
            }

            return WriteValue<T>(process, address, value);
        }

        /// <summary>
        ///     Reads a value of a specified type from the memory of a target process at the given memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to read. Must be a value type (struct).</typeparam>
        /// <param name="process">The target process from which to read the value.</param>
        /// <param name="pointer">Deep Pointer to memory from which the value will be read.</param>
        /// <returns>
        ///     <c>Value</c> at that address if its valid<br/>
        ///     <c>default</c> value of type <typeparamref name="T"/> otherwise.
        /// </returns>
        internal static T ReadValue<T>(Process process, DeepPointer pointer) where T : struct {
            if (!pointer.Deref(process, out IntPtr address)) {
                return default;
            }

            return ReadValue<T>(process, address);
        }

        /// <summary>
        ///     Writes a value of a specified type to the memory of a target process at the given memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to write. Must be a value type (struct).</typeparam>
        /// <param name="process">The target process to write the value to.</param>
        /// <param name="pointer">The memory address where the value will be written.</param>
        /// <param name="value">The value to be written to the process's memory.</param>
        /// <returns>
        ///     <c>true</c> if the value was successfully written to the address<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        internal static bool WriteValue<T>(Process process, IntPtr pointer, T value) where T : struct {
            if (process == null && process.HasExited) {
                return false;
            }

            return process.WriteValue<T>(pointer, value);
        }

        /// <summary>
        ///     Reads a value of a specified type from the memory of a target process at the given memory address.
        /// </summary>
        /// <typeparam name="T">The type of the value to read. Must be a value type (struct).</typeparam>
        /// <param name="process">The target process from which to read the value.</param>
        /// <param name="pointer">The memory address from which the value will be read.</param>
        /// <returns>
        ///     <c>Value</c> at that address if its valid<br/>
        ///     <c>default</c> value of type <typeparamref name="T"/> otherwise.
        /// </returns>
        internal static T ReadValue<T>(Process process, IntPtr pointer) where T : struct
            => process?.ReadValue<T>(pointer) ?? default;
    }
}