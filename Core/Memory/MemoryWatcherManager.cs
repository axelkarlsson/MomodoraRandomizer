using LiveSplit.ComponentUtil;
using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MomodoraRandomizer.Core.Memory;

internal class MemoryWatcherManager {
    private Dictionary<PointerNames, MemoryWatcher> Watchers = [];

    /// <summary>
    ///     Add a MemoryWatcher to the dictionary
    /// </summary>
    /// <param name="name">Name that identifies the MemoryWatcher</param>
    public void Add(PointerNames name, MemoryWatcher watcher)
    => Watchers[name] = watcher;

    /// <summary>
    ///     Obtain MemoryWatcher form the Dictionary matching name
    /// </summary>
    /// <param name="name">Name of the MemoryWatcher</param>
    /// <returns>
    ///     <c>reference</c> to MemoryWatcher if it exists<br/>
    ///     <c>null</c> otherwise
    /// </returns>
    public MemoryWatcher<T>? GetMemoryWatcher<T>(PointerNames name) where T : struct
        => Watchers.TryGetValue(name, out var watcher) ? watcher as MemoryWatcher<T> : null;

    /// <summary>
    ///     Obtain Current attribute from a MemoryWatcher
    /// </summary>
    /// <param name="name">Name of the MemoryWatcher</param>
    /// <returns>
    ///     <c>value</c> of Current for the MemoryWatcher if it exists<br/>
    ///     <c>default</c> <typeparamref name="T"/> value otherwise
    /// </returns>
    public T GetCurrent<T>(PointerNames name) where T : struct
        => GetMemoryWatcher<T>(name)?.Current ?? default;

    /// <summary>
    ///     Obtain Old attribute from a MemoryWatcher
    /// </summary>
    /// <param name="name">Name of the MemoryWatcher</param>
    /// <returns>
    ///     <c>value</c> of Old for the MemoryWatcher if it exists<br/>
    ///     <c>default</c> <typeparamref name="T"/> value otherwise
    /// </returns>
    public T GetOld<T>(PointerNames name) where T : struct
        => GetMemoryWatcher<T>(name)?.Old ?? default;

    /// <summary>
    ///     Update all Watchers in the dictionary
    /// </summary>
    /// <param name="process">Process to use in the Update</param>
    public void UpdateAll(Process process) {
        if (process == null) {
            return;
        }

        foreach (MemoryWatcher watcher in Watchers.Values.Cast<MemoryWatcher>()) {
            watcher.Update(process);
        }
    }

    /// <summary>
    ///     Clear dictionary of MemoryWatcher
    /// </summary>
    public void Clear() => Watchers.Clear();
}