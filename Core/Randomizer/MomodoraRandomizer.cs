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

namespace MomodoraRandomizer.Core.Randomizer {
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
        private Process OldGameProcess = null;
        private Process GameProcess = null;
        private GameVersion OldGameVersion = GameVersion.NONE;
        private GameVersion GameVersion = GameVersion.NONE;
        private List<Item> ItemFullList = new List<Item>();
        private List<Item> ItemRandomizedList = new List<Item>();
        private List<ItemId> ItemExcludeList = new List<ItemId>() { // Items to exclude from Randomization
            ItemId.SMALL_COIN, // Can't randomize NG+ items
            ItemId.BIRTHSTONE, // Can't randomize NG+ items
            ItemId.DIRTY_SHROOM // Can't randomize item dependent on inventory presence
        };
        private Random Rnd;
        private int seed = 0;
        private Dictionary<PointerNames, int[]> PointerOffsets = new Dictionary<PointerNames, int[]>();
        private MemoryWatcherManager WatcherManager = new MemoryWatcherManager();
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
        private void SetSimpleLabelText(string newString, IInvalidator invalidator = null, float width = 0, float height = 0) {
            if (RandomizerLabel.Text != newString) {
                RandomizerLabel.Text = newString;
                invalidator?.Invalidate(0, 0, width, height);
            }
        }
        #endregion

        #region Component Events
        private void OnStart(object sender, EventArgs e) {
            if (!IsProcessRunning(GameProcess)) {
                Debug.WriteLine("Unable to start randomizer. Game process not running."); // TODO: Find way to invalidate Label from OnStart
                return;
            }

            if (!GameVersionMetadata.IsValidGameVersion(GameVersion)) {
                Debug.WriteLine("Unable to start randomizer. No valid version found."); // TODO: Find way to invalidate Label from OnStart
                return;
            }

            bool Success;
            #region RNG seed
            if (!Settings.RandomSeed) { // Acquire seed from settings
                Success = int.TryParse(Settings.seed_get(), out seed);
                if (!Success) {
                    Debug.WriteLine("Unable to start randomizer. Failed to parse seed."); // TODO: Find way to invalidate Label from OnStart
                    // CurrentState.CurrentPhase = TimerPhase.NotRunning; // Forcefully stop timer since we cant randomize the game
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

            Debug.WriteLine($"Seed: {seed}"); // TODO: Find way to invalidate Label from OnStart
            #endregion

            #region Item Randomization
            // Copy items in a random order based on the seed, excluding items that should not be randomized
            ItemRandomizedList = ItemFullList
                                .Where(item => !ShouldExcludeItem(item))
                                .OrderBy(_ => Rnd.Next()).ToList();

            SetNonRandomizedItems(); // Set the non randomized items to be "given as" and "given instead of" to themselves

            RandomizeItems(new ItemId[] { ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP });
            RandomizeItems(new ItemId[] { ItemId.FRESH_SPRING_LEAF });
            RandomizeItems(new ItemId[] { ItemId.CAT_SPHERE });
            RandomizeItems(new ItemId[] { ItemId.GARDEN_KEY, ItemId.MONASTERY_KEY, ItemId.CINDER_KEY });
        
            if (Settings.HardModeEnabled) { // Just to secure Garden access
                RandomizeItems(new ItemId[] { ItemId.BAKMAN_PATCH });
            }

            if (Settings.IvoryBugsEnabled) {
                RandomizeItems(new ItemId[] { ItemId.IVORY_BUG });
            }

            if (Settings.VitalityFragmentsEnabled) {
                RandomizeItems(new ItemId[] { ItemId.VITALITY_FRAGMENT });
            }

            RandomizeItems(ItemRandomizedList
                .Where(item => item.GivenInsteadOf == null)
                .Select(item => item.Id).ToArray());

            ItemRandomizedList
                .OrderBy(item => item.Id.ToString())
                .Select((item, index) => $"{index + 1}.\t {item.Id} {item.InstanceId} transforms into {item.GivenAs.Id} {item.GivenAs.InstanceId}")
                .ToList()
                .ForEach(entry => Debug.WriteLine(entry));

            Debug.WriteLine($"Randomization completed."); // TODO: Find way to invalidate Label from OnStart
            #endregion
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {
            if (!IsProcessRunning(GameProcess)) { // Only update things if the game has been runnning for at least one frame
                SetSimpleLabelText("Game not running.", invalidator, width, height);

                OldGameProcess = GameProcess;
                GameProcess = GetProcess(PROCESS_NAME);

                return;
            }

            if (GameProcess != OldGameProcess) {
                SetSimpleLabelText("Game running.", invalidator, width, height);

                if (CurrentState.CurrentPhase == TimerPhase.Running) {
                    SetSimpleLabelText("New game instance detected\nAttempting to continue...", invalidator, width, height);
                }

                OldGameProcess = GameProcess; // Update old value so this section is not triggered again on the next update
            }

            OldGameVersion = GameVersion;
            GameVersion = GameVersionMetadata.GetGameVersion(GameProcess);

            if (!GameVersionMetadata.IsValidGameVersion(GameVersion)) {
                SetSimpleLabelText("Unsupported game version.", invalidator, width, height);
                return;
            }
        
            if (GameVersion != OldGameVersion) {
                SetSimpleLabelText("Game version changed.", invalidator, width, height);

                if (CurrentState.CurrentPhase == TimerPhase.NotRunning) { // Prepare randomizer information and wait for run to start
                    SetSimpleLabelText($"Supported version detected:\n{GameVersion}", invalidator, width, height);

                    if (ItemFullList.Count == 0) { // Fill item list if its empty, this only needs to be done once
                        foreach (ItemId Id in Enum.GetValues(typeof(ItemId))) {
                            for (int i = 0; i < ItemMetadata.GetItemCount(Id); i++) {
                                ItemFullList.Add(new Item(Id, GameProcess, GameVersion, i));
                            }
                        }
                    }

                    ResetItemsData(); // Make sure the items are on the default state
                    ItemRandomizedList.Clear(); // Clear randomized list since user can change item randomization toggles
                }

                PointerOffsets = PointerMetadata.GetOffsets(GameVersion); // Prepare the pointer offsets

                ItemFullList.ForEach(item => item.SetStringDeepPointers(GameProcess, GameVersion)); // Update Item string pointers

                WatcherManager.Clear(); // Remove old Memory Watchers since base offset is not valid anymore
                PrepareMemoryWatchers(); // Prepare Memory Watchers

                return;
            }

            if (CurrentState.CurrentPhase == TimerPhase.NotRunning) { // Only update watchers if the run is ongoing
                return;
            }

            WatcherManager.UpdateAll(GameProcess); // Only update if everything is correct
        }

        private void OnReset(object sender, TimerPhase value) {
            ResetItemsData();
            Rnd = null; // Reset in case user decides to use a random seed.
        }

        public void Dispose() {
            FreeItemsFakeStrings();
            CurrentState.OnStart -= OnStart;
            CurrentState.OnReset -= OnReset;
        }
        #endregion

        #region Randomizer logic
        #region Memory Watchers
        /// <summary>
        ///     Initializes and registers the memory watchers used to track in‑game values.
        /// </summary>
        private void PrepareMemoryWatchers() {
            PointerNames WatcherName = PointerNames.LEVEL_ID;
            WatcherManager.Add(WatcherName, MemoryWatcherFactory.Create<int>(GameProcess, PointerOffsets[WatcherName], (old, current) => {
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
        #endregion

        #region Item Randomization
        /// <summary>
        ///     Attempts to randomize the specified set of item Ids by assigning each matching item instance in <see cref="ItemRandomizedList"/> a valid source.
        /// </summary>
        /// <param name="ids">Collection of item Ids to randomize.</param>
        /// <returns>
        ///     <c>true</c> if every item receives a valid source<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool RandomizeItems(ItemId[] ids) {
            foreach (ItemId id in ids) {
                foreach (Item item in ItemRandomizedList.Where(i => i.Id == id)) {
                    if (!AssignValidSource(item)) {
                        Debug.WriteLine($"Unable to start randomizer. Failed to find source for {item.Id}, {item.InstanceId}.");
                        return false;
                    }
                    // Debug.WriteLine($"Item {item.GivenInsteadOf.Id}, {item.GivenInsteadOf.InstanceId} transforms into {item.Id}, {item.InstanceId}.");
                }
            }

            return true;
        }

        /// <summary>
        ///     Sets all items that should not be randomized to transform into themselves.
        /// </summary>
        private void SetNonRandomizedItems() {
            foreach (Item item in ItemFullList) {
                if (!ShouldExcludeItem(item)) {
                    continue;
                }

                item.GivenAs = item;
                item.GivenInsteadOf = item;
            }
        }

        /// <summary>
        ///     Check if the item should be excluded from randomization
        /// </summary>
        /// <param name="item">Reference to the item</param>
        /// <returns>
        ///     <c>true</c> it the item should be excluded<br/>
        ///     <c>false</c> otherwise
        /// </returns>
        private bool ShouldExcludeItem(Item item) {            
            if (item.Id == ItemId.VITALITY_FRAGMENT && !Settings.VitalityFragmentsEnabled) {
                return true;
            }

            if (item.Id == ItemId.IVORY_BUG && !Settings.IvoryBugsEnabled) {
                return true;
            }

            if (item.DropType == ItemDropType.BOSS && !Settings.HardModeEnabled) {
                return true;
            }

            if (item.DropType == ItemDropType.NONE || item.DropType == ItemDropType.STARTER) {
                return true;
            }

            if (ItemExcludeList.Contains(item.Id)) {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempts to assign a valid source item for the specified item based on the dependency and reachability rules.
        /// </summary>
        /// <param name="item">The item that requires a valid source assignment.</param>
        /// <returns>
        ///     <c>true</c> if a valid source is found or the item already has a source assigned<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool AssignValidSource(Item item) {
            if (item.GivenInsteadOf != null) { // This item was already randomized
                return true;
            }
            // Debug.WriteLine($"Assigning {item.Id}, {item.InstanceId}.");

            foreach (Item source in ItemRandomizedList) {
                if (source.GivenAs != null) { // This item already has a replacement item
                    continue;
                }

                // Debug.WriteLine($"Attempting source {source.Id}, {source.InstanceId}.");
                if (!IsValidSource(item, source)) {
                    // Debug.WriteLine($"Failed to source at {source.Id}, {source.InstanceId}.");
                    continue;
                }

                item.GivenInsteadOf = source;
                source.GivenAs = item;

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Check that <c>item</c> is a valid replacement for <c>source</c>.
        /// </summary>
        /// <param name="item">The item that will be given</param>
        /// <param name="source">The item that is picked up</param>
        /// <returns>
        ///     <c>true</c> if the item is acquirable when picking it up at the source candidate<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool IsValidSource(Item item, Item source) {
            item.GivenInsteadOf = source; // Temporary assignment
            source.GivenAs = item;

            bool Acquirable = IsItemAcquirable(item, new HashSet<Item>());

            item.GivenInsteadOf = null; // Undo temporary assignment
            source.GivenAs = null;

            return Acquirable;
        }

        /// <summary>
        ///     Check if the specified item is acquirable based on its assigned source and its requirements.
        /// </summary>
        /// <param name="item">The item whose acquirability is being evaluated.</param>
        /// <param name="visited">Set of items already evaluated in the current recursion chain. Used to prevent dependency loops.</param>
        /// <returns>
        ///     <c>true</c> if the item is acquirable through its assigned source and all associated requirements<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool IsItemAcquirable(Item item, HashSet<Item> visited) {
            if (visited.Contains(item)) {
                // Debug.WriteLine($"Item {item.Id}, {item.InstanceId} already checked, executing loop prevention mechanism.");
                return false;
            }

            visited.Add(item);

            // Debug.WriteLine($"Checking if item {item.Id} {item.InstanceId} acquirable.");
            Item ItemSource = item.GivenInsteadOf; // What Item is given as item

            if (ItemSource == null) { // If the item does not have a source attempt to assign one
                // Debug.WriteLine($"No source found for {item.Id} {item.InstanceId}, attempting to find new source.");
                return AssignValidSource(item); // We know that the item is reachable if it has found an item source, if no item source is found then the item is not reachable as of now
            }

            // Debug.WriteLine($"Checking if source {ItemSource.Id} {ItemSource.InstanceId} acquirable.");
            if (!AreHardRequirementsCompletable(ItemSource, visited)) {
                // Debug.WriteLine($"Hard requirements not met for {ItemSource.Id} {ItemSource.InstanceId}.");
                visited.Remove(item);
                return false;
            }

            if (!AreSoftRequirementsCompletable(ItemSource, visited)) {
                // Debug.WriteLine($"Soft requirements not met for {ItemSource.Id} {ItemSource.InstanceId}.");
                visited.Remove(item);
                return false;
            }

            if (!AreIvoryBugRequirementsCompletable(ItemSource, visited)) {
                // Debug.WriteLine($"Ivory Bug requirements not met for {ItemSource.Id} {ItemSource.InstanceId}.");
                visited.Remove(item);
                return false;
            }

            if (!AreCrestFragmentRequirementsCompletable(ItemSource, visited)) {
                // Debug.WriteLine($"Crest Fragment requirements not met for {ItemSource.Id} {ItemSource.InstanceId}.");
                visited.Remove(item);
                return false;
            }

            // Debug.WriteLine($"Requirements met for {ItemSource.Id} {ItemSource.InstanceId}, item is acquirable.");
            visited.Remove(item);
            return true;
        }

        /// <summary>
        ///     Checks if all hard requirements for the specified item are completable.
        /// </summary>
        /// <param name="item">The item whose requirements are being evaluated.</param>
        /// <param name="visited">Set of items already evaluated in the current recursion chain. Used to prevent dependency loops.</param>
        /// <returns>
        ///     <c>true</c> if all hard requirements are acquirable<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool AreHardRequirementsCompletable(Item item, HashSet<Item> visited) {
            List<ItemId> HardRequirementList = new List<ItemId>(ItemMetadata.GetHardRquirements(item.Id, item.InstanceId));

            // Debug.WriteLine($"Hard requirements for {item.Id} {item.InstanceId}:  {HardRequirementList.Count}.");
            foreach (ItemId itemId in HardRequirementList) {
                Item HardItem = ItemFullList.First(i => i.Id == itemId);

                if (!IsItemAcquirable(HardItem, visited)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Checks if at least one soft requirement for the specified item is completable.
        /// </summary>
        /// <param name="item">The item whose requirements are being evaluated.</param>
        /// <param name="visited">Set of items already evaluated in the current recursion chain. Used to prevent dependency loops.</param>
        /// <returns>
        ///     <c>true</c> if at least one soft requirement is acquirable<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool AreSoftRequirementsCompletable(Item item, HashSet<Item> visited) {
            List<ItemId> SoftRequirementList = new List<ItemId> (ItemMetadata.GetSoftRquirements(item.Id, item.InstanceId));

            // Debug.WriteLine($"Soft requirements for {item.Id} {item.InstanceId}:  {SoftRequirementList.Count}.");
            if (SoftRequirementList.Count == 0) {
                return true;
            }

            foreach (ItemId itemId in SoftRequirementList) {
                Item SoftItem = ItemFullList.First(i => i.Id == itemId);

                if (SoftItem.DropType == ItemDropType.BOSS && !Settings.HardModeEnabled) {
                    // Debug.WriteLine($"Hard Mode disabled, boss item {item.Id} {item.InstanceId} considred as not reachable.");
                    continue;
                }

                if (IsItemAcquirable(SoftItem, visited)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks if the Ivory Bug requirements for the specified item are completable.
        /// </summary>
        /// <param name="item">The item whose requirements are being evaluated.</param>
        /// <param name="visited">Set of items already evaluated in the current recursion chain. Used to prevent dependency loops.</param>
        /// <returns>
        ///     <c>true</c> if the required number of Ivory Bugs are acquirable<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool AreIvoryBugRequirementsCompletable(Item item, HashSet<Item> visited) {
            int MaxIvoryBugCount = ItemMetadata.GetItemCount(ItemId.IVORY_BUG);
            int IvoryBugRequirementCount = ItemMetadata.GetIvoryBugCountRequirements(item.Id, item.InstanceId);

            // Debug.WriteLine($"Ivory Bug requirement count for {item.Id} {item.InstanceId}:  {IvoryBugRequirementCount}.");
            if (IvoryBugRequirementCount == 0) {
                return true;
            }

            List<Item> IBList = new List<Item>(ItemFullList.Where(i => i.Id == ItemId.IVORY_BUG && !IsItemAcquirable(i, visited)));

            // Debug.WriteLine($"Non acquirable Ivory Bugs:  {string.Join(", ", IBList.Select(i => $"{i.Id} {i.InstanceId}"))}.");
            if (MaxIvoryBugCount - IBList.Count >= IvoryBugRequirementCount) {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if the Crest Fragment requirements for the specified item are completable.
        /// </summary>
        /// <param name="item">The item whose requirements are being evaluated.</param>
        /// <param name="visited">Set of items already evaluated in the current recursion chain. Used to prevent dependency loops.</param>
        /// <returns>
        ///     <c>true</c> if the required number of Crest Fragments are acquirable<br/>
        ///     <c>false</c> otherwise.
        /// </returns>
        private bool AreCrestFragmentRequirementsCompletable(Item item, HashSet<Item> visited) {
            ItemId[] CrestFragmentIds = new ItemId[] { ItemId.CF_BOW_LVL, ItemId.CF_BOW_SPEED, ItemId.CF_DASH, ItemId.CF_WARP };
            int MaxCrestFragmentCount = CrestFragmentIds.Sum(itemId => ItemMetadata.GetItemCount(itemId)); // I know its 4, you know its 4, my grandma knows its 4. But I still need to do it like this
            int CrestFragmentRequirementCount = ItemMetadata.GetCrestFragmentCountRequirenents(item.Id, item.InstanceId);

            // Debug.WriteLine($"Crest Fragment requirement count for {item.Id}, {item.InstanceId}:  {CrestFragmentRequirementCount}.");
            if (CrestFragmentRequirementCount == 0) {
                return true;
            }

            List<Item> CFList = new List<Item>(ItemFullList.Where(i => CrestFragmentIds.Contains(i.Id) && !IsItemAcquirable(i, visited)));

            // Debug.WriteLine($"Non acquirable Crest Fragments:  {string.Join(", ", CFList.Select(i => $"{i.Id} {i.InstanceId}"))}.");
            if (MaxCrestFragmentCount - CFList.Count >= CrestFragmentRequirementCount) {
                return true;
            }

            return false;
        }
        #endregion

        #region Item manipulation
        /// <summary>
        ///     Frees the memory for fake strings.
        /// </summary>
        private void FreeItemsFakeStrings()
            => ItemFullList.ForEach(item => item.FreeFakeStringPointers());

        /// <summary>
        ///     Resets very item in <see cref="ItemFullList"/> to the default values.
        /// </summary>
        private void ResetItemsData()
            => ItemFullList.ForEach(item => item.Reset());
        #endregion

        #region Process
        /// <summary>
        ///     Checks if a process with the specified name is currently running and returns a reference.
        /// </summary>
        /// <param name="processName">The name of the process to check.</param>
        /// <returns>
        ///     A <see cref="Process"/> reference if the process is running; otherwise, <c>null</c>.
        /// </returns>
        private Process GetProcess(string processName) {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0 ? processes[0] : null;
        }

        /// <summary>
        ///     Checks if the specified process is currently running.
        /// </summary>
        /// <param name="process">The reference to the process to check.</param>
        /// <returns>
        ///     <c>true</c> if the process is running; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProcessRunning(Process process)
            => process != null && !process.HasExited;
        #endregion
        #endregion
    } 
}