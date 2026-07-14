using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;

namespace MomodoraRandomizer.Core.Memory {
    public static class MemoryWatcherFactory {
        #region Constructors
        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">Pointer to memory address</param>
        /// <param name="timeSpan">Update interval (10ms if not provided)</param>
        /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(IntPtr pointer, TimeSpan? timeSpan, bool? enabled, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) where T : struct {
            MemoryWatcher<T> Watcher = new MemoryWatcher<T>(pointer) {
                UpdateInterval = timeSpan ?? new TimeSpan(0, 0, 0, 0, 10), // default to 10 ms refresh rate
                Enabled = enabled ?? true
            };

            Watcher.OnChanged += OnChanged;

            return Watcher;
        }

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">Deep Pointer to memory</param>
        /// <param name="timeSpan">Update interval (10ms if not provided)</param>
        /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(DeepPointer pointer, TimeSpan? timeSpan, bool? enabled, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) where T : struct {
            MemoryWatcher<T> Watcher = new MemoryWatcher<T>(pointer) {
                UpdateInterval = timeSpan ?? new TimeSpan(0, 0, 0, 0, 10), // default to 10 ms refresh rate
                Enabled = enabled ?? true
            };

            Watcher.OnChanged += OnChanged;

            return Watcher;
        }

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">Pointer to memory address</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(IntPtr pointer, MemoryWatcher<T>.DataChangedEventHandler OnChanged) where T : struct
            => Create(pointer, null, null, OnChanged);

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">Pointer to memory address</param>
        /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(IntPtr pointer, bool enabled = false, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) where T : struct
            => Create(pointer, null, enabled, OnChanged);

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">DeepPointer to memory</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(DeepPointer pointer, MemoryWatcher<T>.DataChangedEventHandler OnChanged) where T : struct
            => Create(pointer, null, null, OnChanged);

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="pointer">DeepPointer to memory</param>
        /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(DeepPointer pointer, bool enabled = false, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) where T : struct
            => Create(pointer, null, enabled, OnChanged);

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="process">Process to get pointer of</param>
        /// <param name="offsets">Offsets of pointer</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(Process process, int[] offsets, MemoryWatcher<T>.DataChangedEventHandler OnChanged) where T : struct
            => Create(PointerUtility.CreateDeepPointer(process, offsets), null, null, OnChanged);

        /// <summary>
        ///     Construct a new MemoryWatcher
        /// </summary>
        /// <param name="process">Process to get pointer of</param>
        /// <param name="offsets">Offsets of pointer</param>
        /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
        /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
        /// <returns>New instance of MemoryWatcher</returns>
        public static MemoryWatcher<T> Create<T>(Process process, int[] offsets, bool enabled = true, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) where T : struct
            => Create(PointerUtility.CreateDeepPointer(process, offsets), null, enabled, OnChanged);
        #endregion
    }
}