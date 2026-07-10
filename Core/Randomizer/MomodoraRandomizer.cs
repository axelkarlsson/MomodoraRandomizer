using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using MomodoraRandomizer.Core.Memory;
using MomodoraRandomizer.Data.Enums;
using MomodoraRandomizer.Data.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace MomodoraRandomizer.Core.Randomizer;

public class MomodoraRandomizer : IComponent {
    #region Component variables
    private SimpleLabel RandomizerLabel;
    private LiveSplitState CurrentState { get; set; }
    private MomodoraRandomizerSettings Settings { get; set; }

    public string ComponentName => "Momodora Randomizer";

    public float HorizontalWidth { get; set; }
    public float MinimumHeight => 10;
    public float VerticalHeight { get; set; }
    public float MinimumWidth => 200;

    public float PaddingTop => 1;
    public float PaddingBottom => 1;
    public float PaddingLeft => 1;
    public float PaddingRight => 1;
    #endregion

    #region Randomizer variables
    private const string PROCESS_NAME = "MomodoraRUtM";
    private Process? GameProcess = null;
    private GameVersion GameVersion = GameVersion.UNSUPPORTED;
    private List<Item> ItemFullList = [];
    private List<Item> ItemRandomizedList = [];
    private List<ItemId> ItemExcludeList = [ // Items to exclude from Randomization
        ItemId.DIRTY_SHROOM,
        ItemId.SMALL_COIN,
        ItemId.BIRTHSTONE
    ];
    private Random? Rnd;
    private int seed = 0;
    private Dictionary<PointerNames, int[]> PointerOffsets = [];
    private MemoryWatcherManager WatcherManager = new();
    #endregion

    #region Component logic
    public IDictionary<string, Action> ContextMenuControls => null;

    public MomodoraRandomizer(LiveSplitState state) {
        CurrentState = state;

        RandomizerLabel = new SimpleLabel();
        Settings = new MomodoraRandomizerSettings();

        state.OnStart += OnStart;
        state.OnReset += OnReset;
    }

    public void prepareDraw(LiveSplitState state) {
        RandomizerLabel.Font = Settings.OverrideTextFont ? Settings.TextFont : state.LayoutSettings.TextFont;
        RandomizerLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
        RandomizerLabel.OutlineColor = Settings.OverrideTextColor ? Settings.OutlineColor : state.LayoutSettings.TextOutlineColor;
        RandomizerLabel.ShadowColor = Settings.OverrideTextColor ? Settings.ShadowColor : state.LayoutSettings.ShadowsColor;

        RandomizerLabel.VerticalAlignment = StringAlignment.Center;
        RandomizerLabel.HorizontalAlignment = StringAlignment.Center;
    }

    public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) => throw new NotImplementedException();

    public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) {
        var textHeight = g.MeasureString("A", state.LayoutSettings.TextFont).Height;
        VerticalHeight = textHeight * 1.5f;

        prepareDraw(state);
        RandomizerLabel.SetActualWidth(g);
        RandomizerLabel.Width = RandomizerLabel.ActualWidth;
        RandomizerLabel.Height = VerticalHeight;
        RandomizerLabel.X = width - PaddingRight - RandomizerLabel.Width;
        RandomizerLabel.Y = 3f;

        DrawBackground(g, width, VerticalHeight);

        RandomizerLabel.Draw(g);
    }

    private void DrawBackground(Graphics g, float width, float height) {
        if (Settings.BackgroundColor.A > 0
            || Settings.BackgroundGradient != GradientType.Plain
            && Settings.BackgroundColor2.A > 0) {
            var gradientBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.BackgroundGradient == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.BackgroundColor,
                        Settings.BackgroundGradient == GradientType.Plain
                        ? Settings.BackgroundColor
                        : Settings.BackgroundColor2);
            g.FillRectangle(gradientBrush, 0, 0, width, height);
        }
    }

    public XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

    public Control GetSettingsControl(LayoutMode mode) => Settings;

    public void SetSettings(XmlNode settings) => this.Settings.SetSettings(settings);

    /// <summary>
    ///     Changes the text of the RandomizerLabel and invalidates the state if it's different.
    /// </summary>
    /// <param name="newString">The text to set RandomizerLabel to.</param>
    /// <param name="invalidator">Component Invalidator.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    private void SetSimpleLabelText(string newString, IInvalidator? invalidator = null, float width = 0, float height = 0) {
        if (RandomizerLabel.Text != newString) {
            RandomizerLabel.Text = newString;
            invalidator?.Invalidate(0, 0, width, height);
        }
    }
    #endregion

    #region Component Events
    private void OnStart(object sender, EventArgs e) {
        if (IsProcessRunning(GameProcess)) {
            GameVersion = GetGameVersion();
            if (IsValidGameVersion(GameVersion)) {
                PointerNames WatcherName;

                // Acquire seed from settings
                if (!Settings.RandomSeed) {
                    bool success = int.TryParse(Settings.seed_get(), out seed);
                    if (!success) {
                        Debug.WriteLine("Failed to parse randomizer seed"); // TODO: Find way to invalidate Label from OnStart
                        CurrentState.CurrentPhase = TimerPhase.NotRunning; // Forcefully stop timer since we cant randomize the game
                        return;
                    }

                    Rnd = new Random(seed);
                }
                
                if (Rnd == null) { // If it failed to acquire seed (should it even continue? Prob not) or it was set to false generate our own
                    Rnd = new Random();
                    seed = Rnd.Next();
                    Settings.seed_set(seed);
                    Rnd = new Random(seed);
                }

                // Fill item list if its empty
                if (ItemFullList.Count == 0) {
                    foreach (var (itemId, dropTypes) in ItemMetadata.DropTypes) {
                        if (dropTypes.Length == 1) {
                            ItemFullList.Add(new Item(itemId, GameProcess, GameVersion));
                        } else {
                            for (int i = 0; i < dropTypes.Length; i++) {
                                ItemFullList.Add(new Item(itemId, GameProcess, GameVersion, i));
                            }
                        }
                    }
                } else { // Prepare offsets based on current version
                    foreach (Item item in ItemFullList) {
                        item.SetStringPtrs(GameProcess, GameVersion);
                    }
                }

                // Copy items in a random order based on the seed, excluding items that should not be randomized
                List<Item> ItemsCopyList = (List<Item>)ItemFullList
                                                    .Where(item => !ShouldExcludeItem(item))
                                                    .OrderBy(_ => Rnd.Next());

                // Assign which item to give instead of the original one
                foreach (Item item in ItemFullList) {
                    Item NewReference = FindValidReference(item, ItemsCopyList);

                    NewReference ??= AssignReferenceChain(item, ItemsCopyList);

                    item.TransformsInto = NewReference;
                    NewReference.TransformsFrom = item;
                    NewReference.Requirements.AddRange(item.Requirements);

                    ItemsCopyList.Remove(NewReference);
                    Debug.WriteLine($"{item.Id} transforms into {item.TransformsInto.Id}");
                }

                // TODO: Make sure that the first item has a TransformFrom as well.

                // Prepare the pointers offsets
                PointerOffsets = PointerMetadata.GetOffsets(GameVersion);

                // Create memory watchers
                WatcherName = PointerNames.LEVEL_ID;
                WatcherManager.Add(WatcherName, MemoryWatcherFactory.Create<double>(GameProcess, PointerOffsets[WatcherName], (old, current) => {
                    Debug.WriteLine($"Level_ID: {old} -> {current}");
                    if (current == 1) {
                        // ItemFullList.ResetValues();
                    } else {
                        // Item.SetValues();
                        // TODO: Add logic to iniciate tracking so we can replace the item when its picked up. But only if we are in a room which contains an item that has not been picked up
                    }
                }));

                WatcherName = PointerNames.MAP_X;
                WatcherManager.Add(WatcherName, MemoryWatcherFactory.Create<double>(GameProcess, PointerOffsets[WatcherName], (old, current) => {
                    Debug.WriteLine($"Map_X: {old} -> {current}");
                }));

                WatcherName = PointerNames.MAP_Y;
                WatcherManager.Add(WatcherName, MemoryWatcherFactory.Create<double>(GameProcess, PointerOffsets[WatcherName], (old, current) => {
                    Debug.WriteLine($"Map_Y: {old} -> {current}");
                }));

                // TODO Add the rest of the watchers and their logic
            }
        }
    }

    public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
        if (IsProcessRunning(GameProcess, invalidator, width, height)) {
            GameVersion = GetGameVersion(invalidator, width, height);
            if (IsValidGameVersion(GameVersion) && CurrentState.CurrentPhase == TimerPhase.Running) {
                WatcherManager.UpdateAll(GameProcess);
            }
        } else {
            GameProcess = GetProcess(PROCESS_NAME);
        }
    }

    private void OnReset(object sender, TimerPhase value) {
        //if ((pGameProcess = GetProcess(PROCESS_NAME)) != null)
        //{
            WatcherManager.Clear();
            PointerOffsets.Clear();
            ItemRandomizedList.Clear();
            ResetItemsData();
            Rnd = null;
        //}
    }

    public void Dispose() {
        CurrentState.OnStart -= OnStart;
        CurrentState.OnReset -= OnReset;
    }
    #endregion

    #region Randomizer logic
    #region Item
    /// <summary>
    ///     Resets values and ItemReferences in the 'ItemList' to default.
    /// </summary>
    private void ResetItemsData() {
        foreach (Item item in ItemFullList) {
            item.Reset();
        }
    }

    /// <summary>
    ///     Check if the item should be excluded from randomization
    /// </summary>
    /// <param name="item">Reference to the item</param>
    /// <returns><c>true</c> it the item should be excluded<br/>
    /// <c>false</c> otherwise</returns>
    private bool ShouldExcludeItem(Item item) {
        if (item.Id == ItemId.VITALITY_FRAGMENT && !Settings.VitalityFragmentsEnabled) {
            return true;
        }

        if (item.Id == ItemId.IVORY_BUG && !Settings.IvoryBugsEnabled) {
            return true;
        }

        if (item.DropType == ItemDropType.Boss && !Settings.HardModeEnabled) {
            return true;
        }

        if (item.Id == ItemId.BELLFLOWER) { // Exclude Bellflower based on the GameVersion and Spawn room since it was changed on 1.07. Temp Fix
            if (GameVersion == GameVersion.VERSION_1_05b && item.SpawnRooms.Contains(64)) {
                return true;
            } else if (GameVersion == GameVersion.VERSION_1_07 && item.SpawnRooms.Contains(70)) {
                return true;
            }
        }

        if (ItemExcludeList.Contains(item.Id)) {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Finds a valid reference item for the specified current item.
    /// </summary>
    /// <param name="currentItem">The current item for which to find a valid reference.</param>
    /// <param name="itemList">The list of items to search for a valid reference.</param>
    /// <returns>The first valid <see cref="Item"/> reference found<br/>
    /// <c>null</c>, otherwise.</returns>
    private Item FindValidReference(Item currentItem, List<Item> itemList) {
        return itemList.FirstOrDefault(potentialReference
            => IsReferenceValid(currentItem, potentialReference));
    }

    // Needs revision to make sure this is working as intended
    /// <summary>
    ///     Reassign references in a chain from the specified item.
    /// </summary>
    /// <param name="currentItem">The item from which the chain starts.</param>
    /// <param name="itemsList">The list of items to use for findig a new reference.</param>
    /// <returns>The last valid item found in the list.</returns>
    private Item AssignReferenceChain(Item currentItem, List<Item> itemsList) {
        Item NewReference = null;
        //int loopCount = 0;
        //do {
        //    NewReference = FindValidReference(currentItem.TransformsFrom, itemsList.Except(currentItem));

        //    loopCount++;
        //} while (NewReference == null && currentItem.TransformsFrom != null);
        //return NewReference;
        do {
            Item referencesCurrent = currentItem.TransformsFrom;
            currentItem.TransformsInto = currentItem;
            currentItem.TransformsFrom = currentItem;
            foreach (ItemId itemName in referencesCurrent.Requirements)
                currentItem.Requirements.Remove(itemName);
            currentItem = referencesCurrent;
            NewReference = FindValidReference(currentItem, itemsList);
        } while (NewReference == null);
        NewReference.Requirements.AddRange(currentItem.Requirements.Except(NewReference.Requirements));
        return currentItem.TransformsInto = NewReference;
    }

    /// <summary>
    ///     Check that <c>potentialReference</c> is a valid item for <c>item</c> to transform into
    /// </summary>
    /// <param name="item">Reference to the item</param>
    /// <param name="potentialReference">Reference to the item that <c>item</c> will tranform into</param>
    /// <returns></returns>
    private bool IsReferenceValid(Item item, Item potentialReference) {
        return !item.Requirements.Contains(potentialReference.Id)
            && !potentialReference.Requirements.Contains(item.Id);
    }
    #endregion

    #region Process
    /// <summary>
    ///     Checks if a process with the specified name is currently running and returns a reference.
    /// </summary>
    /// <param name="processName">The name of the process to check.</param>
    /// <returns>
    ///     A <see cref="Process"/> reference if the process is running; otherwise, <c>null</c>.
    /// </returns>
    private Process? GetProcess(string processName) {
        Process[] game = Process.GetProcessesByName(processName);
        Process? process = null;

        if (game.Length > 0) {
            process = game[0];
        }

        return process;
    }

    /// <summary>
    ///     Checks if the specified process is currently running.
    /// </summary>
    /// <param name="process">The reference to the process to check.</param>
    /// <param name="invalidator">Component Invalidator.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <returns>
    ///     <c>true</c> if the process is running; otherwise, <c>false</c>.
    /// </returns>
    private bool IsProcessRunning(Process? process, IInvalidator? invalidator = null, float width = 0, float height = 0) {
        bool Running = process != null && !process.HasExited;

        if (invalidator != null && !Running) {
            SetSimpleLabelText("Game not running", invalidator, width, height);
        }

        return Running;
    }
    #endregion

    #region Game version
    /// <summary>
    ///     Gets the version of the Game based on the module memory size.
    /// </summary>
    /// <param name="invalidator">Component Invalidator</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <returns>
    ///     <see cref="MomodoraRandomizer.Data.Enums.GameVersion"/> if it's a supported version; otherwise, <see cref="GameVersion.NONE"/>.
    /// </returns>
    private GameVersion GetGameVersion(IInvalidator? invalidator = null, float width = 0, float height = 0) {
        string text;
        GameVersion version;

        if (GameProcess == null) {
            return GameVersion.NONE;
        }

        switch (GameProcess?.MainModule?.ModuleMemorySize) {
            case 39690240:
                text = "Supported version detected: 1.05b";
                version = GameVersion.VERSION_1_05b;
                break;
            case 40222720:
                text = "Supported version detected: 1.07";
                version = GameVersion.VERSION_1_07;
                break;
            default:
                text = "Version not supported";
                version = GameVersion.UNSUPPORTED;
                break;
        }

        if (invalidator != null) {
            SetSimpleLabelText(text, invalidator, width, height);
        }

        return version;
    }

    ///<summary>
    ///     Checks that the version sent is supported
    ///</summary>
    ///<param name="gameVersion">Version of the game to check</param>
    ///<returns>
    ///     <c>true</c> if the version is valid; otherwise, <c>false</c>.
    ///</returns>
    private static bool IsValidGameVersion(GameVersion gameVersion)
        => gameVersion is not GameVersion.NONE and not GameVersion.UNSUPPORTED;
    #endregion
    #endregion
}