using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class ContainerWatcher<T> where T : struct
{
    public static List<ContainerWatcher<T>> List = new List<ContainerWatcher<T>>();

    public string Name { get; }
    public MemoryWatcher<T> Watcher { get; }

    #region Constructors
    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="pointer">Pointer to memory address</param>
    /// <param name="timeSpan">Update interval (10ms if not provided)</param>
    /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, IntPtr pointer, TimeSpan? timeSpan, bool? enabled, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null)
    {
        this.Name = name;
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
    public ContainerWatcher(string name, Process process, int[] offsets, MemoryWatcher<T>.DataChangedEventHandler OnChanged) : this(name, PointerUtility.CreatePointer(process, offsets), null, null, OnChanged) { }

    /// <summary>
    /// Construct WatcherContainer, creating a new MemoryWatcher
    /// </summary>
    /// <param name="name">MemoryWatcher Name</param>
    /// <param name="process">Process to get pointer of</param>
    /// <param name="offsets">Offsets of pointer</param>
    /// <param name="enabled"><c>Enable</c> or <c>Disable</c> Watcher</param>
    /// <param name="OnChanged">Method reference to attach to OnChanged event</param>
    public ContainerWatcher(string name, Process process, int[] offsets, bool enabled = true, MemoryWatcher<T>.DataChangedEventHandler OnChanged = null) : this(name, PointerUtility.CreatePointer(process, offsets), null, enabled, OnChanged) { }
    #endregion

    #region Class methods
    /// <summary>
    /// Update Watchers from the WatcherContainer Class List
    /// </summary>
    /// <param name="process">Process to use in the Update</param>
    public static void UpdateWatchers(Process process)
    {
        foreach (var container in List)
        {
            container.Watcher.Update(process);
        }
    }

    /// <summary>
    /// Clear list of ContainerWatchers
    /// </summary>
    public static void Clear() => List.Clear();

    /// <summary>
    /// Obtain WatcherContainer form Class List matching name and Type
    /// </summary>
    /// <param name="name">Name of the WatcherContainer</param>
    /// <returns>
    ///     <c>reference</c> to WatcherContainer if it exists<br/>
    ///     <c>null</c> otherwise
    /// </returns>
    public static ContainerWatcher<T> GetContainerWatcher(string name)
    {
        foreach(var container in List)
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
        ContainerWatcher<T> container = GetContainerWatcher(name);

        if (container != null)
        {
            return container.Watcher;
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
        MemoryWatcher<T> watcher = GetMemoryWatcher(name);

        if (watcher != null)
        {
            return watcher.Current;
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
        MemoryWatcher<T> watcher = GetMemoryWatcher(name);

        if (watcher != null)
        {
            return watcher.Old;
        }

        return default;
    }
    #endregion
}
