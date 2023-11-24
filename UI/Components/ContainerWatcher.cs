using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class ContainerWatcher<T> where T : struct
{
    static List<ContainerWatcher<T>> WatcherContainers { get; } = new List<ContainerWatcher<T>>();

    public string Name { get; }
    public MemoryWatcher<T> Watcher { get; }

    #region Constructors
    private ContainerWatcher(string name)
    {
        this.Name = name;
        if (!Exists())
        {
            WatcherContainers.Add(this);
        }
    }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="pointer">Pointer to memory address</param>
    /// <param name="timeSpan">Update interval (10ms if not provided)</param>
    /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, IntPtr pointer, TimeSpan? timeSpan, bool? enabled, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) : this(name)
    {
        this.Watcher = new MemoryWatcher<T>(pointer)
        {
            UpdateInterval = timeSpan ?? new TimeSpan(0, 0, 0, 0, 10),
            Enabled = enabled ?? true
        };
        Watcher.OnChanged += OnChanged;
    }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="pointer">Pointer to memory address</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, IntPtr pointer, MemoryWatcher<T>.DataChangedEventHandler OnChanged) : this(name, pointer, null, null, OnChanged) { }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="pointer">Pointer to memory address</param>
    /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, IntPtr pointer, bool enabled = false, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) : this(name, pointer, null, enabled, OnChanged) { }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="process">Process to get pointer of</param>
    /// <param name="offsets">Offsets of pointer</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, Process process, int[] offsets, MemoryWatcher<T>.DataChangedEventHandler OnChanged) : this(name, CreatePointer(process, offsets), null, null, OnChanged) { }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="process">Process to get pointer of</param>
    /// <param name="offsets">Offsets of pointer</param>
    /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, Process process, int[] offsets, bool enabled = true, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) : this(name, CreatePointer(process, offsets), null, enabled, OnChanged) { }
    #endregion

    #region Class methods
    /// <summary>
    /// Update Watchers from the WatcherContainer Class List
    /// </summary>
    /// <param name="process">Process to use in the Update</param>
    public static void UpdateWatchers(Process process)
    {
        foreach (var container in WatcherContainers)
        {
            container.Watcher.Update(process);
        }
    }

    /// <summary>
    /// Clear list of ContainerWatchers
    /// </summary>
    public static void ClearWatchers() => WatcherContainers.Clear();

    /// <summary>
    /// Obtain WatcherContainer form Class List matching name and Type
    /// </summary>
    /// <param name="name">Name of the WatcherContainer</param>
    /// <returns>
    ///     <c>reference</c> to WatcherContainer if it exists<br/>
    ///     <c>null</c> otherwise
    /// </returns>
    public static ContainerWatcher<T> GetWatcherContainer(string name)
    {
        foreach(var container in WatcherContainers)
        {
            if(container.Name == name && container.Watcher.Current.GetType() == typeof(T))
            {
                return container;
            }
        }
        return null;
    }

    /// <summary>
    /// Obtain MemoryWatcher from Class List matching name and Type
    /// </summary>
    /// <param name="name">Name of the WatcherContainer containing the MemoryWatcher</param>
    /// <returns>
    ///     <c>reference</c> to MemoryWatcher if it exists<br/>
    ///     <c>null</c> otherwise
    /// </returns>
    public static MemoryWatcher<T> GetMemoryWatcher(string name)
    {
        foreach(var container in WatcherContainers)
        {
            if (container.Name == name && container.Watcher.Current.GetType() == typeof(T))
            {
                return container.Watcher;
            }
        }
        return null;
    }

    /// <summary>
    /// Obtain Current attribute from a MemoryWatcher
    /// </summary>
    /// <param name="name">Name of the WatcherContainer containing the value</param>
    /// <returns>
    ///     <c>value</c> of Current for the MemoryWatcher if it exists<br/>
    ///     <c>default</c> <typeparamref name="T"/> value otherwise
    /// </returns>
    public static T GetCurrent(string name)
    {
        foreach (var container in WatcherContainers)
        {
            if (container.Name == name && container.Watcher.Current.GetType() == typeof(T))
            {
                return container.Watcher.Current;
            }
        }
        return default;
    }

    /// <summary>
    /// Obtain Old attribute from a MemoryWatcher
    /// </summary>
    /// <param name="name">Name of the WatcherContainer containing the value</param>
    /// <returns>
    ///     <c>value</c> of Old for the MemoryWatcher if it exists<br/>
    ///     <c>default</c> <typeparamref name="T"/> value otherwise
    /// </returns>
    public static T GetOld(string name)
    {
        foreach (var container in WatcherContainers)
        {
            if (container.Name == name && container.Watcher.Current.GetType() == typeof(T))
            {
                return container.Watcher.Old;
            }
        }
        return default;
    }

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
    private static IntPtr CreatePointer(Process process, params int[] offsets)
    {
        var typeDeref = new Dictionary<Type, Func<DeepPointer, IntPtr>>
            {
                { typeof(short), (dp) => (IntPtr)dp.Deref<short>(process) },
                { typeof(int), (dp) => (IntPtr)dp.Deref<int>(process) },
                { typeof(long), (dp) => (IntPtr)dp.Deref<long>(process) },
                { typeof(double), (dp) => (IntPtr)dp.Deref<double>(process) },
                { typeof(float), (dp) => (IntPtr)dp.Deref<float>(process) }
            };

        if (typeDeref.TryGetValue(typeof(T), out var deref))
        {
            if (offsets.Length == 1)
            {
                return IntPtr.Add(process.MainModule.BaseAddress, offsets.First());
            }
            else
            {
                return IntPtr.Add(deref(new DeepPointer(offsets.First(), offsets.Skip(1).Take(offsets.Length - 2).ToArray())), offsets.Last());
            }

        }

        return IntPtr.Zero;
    }
    #endregion

    #region Other methods
    /// <summary>
    /// Check if WatcherContainer with the same name and type exists
    /// </summary>
    /// <returns>
    ///     <c>true</c> if it exists<br/>
    ///     <c>false</c> otherwise
    /// </returns>
    private bool Exists()
    {
        foreach(var container in WatcherContainers)
        {
            if (this.Name == container.Name && this.Watcher.Current.GetType() == container.Watcher.Current.GetType())
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}
