using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using LiveSplit.Model;
using System.Drawing.Drawing2D;

namespace LiveSplit.UI.Components
{
    public class MomodoraRandomizer : IComponent
    {
        #region Randomizer variables
        private const string VERSION_1_05b = "1.05b", VERSION_1_07 = "1.07";
        private const string PROCESS_NAME = "MomodoraRUtM";
        private Process GameProcess = null;
        private string GameVersion = "";
        private Random Rnd;
        private int seed = 0;
        private Dictionary<string, int[]> Offsets = new Dictionary<string, int[]>();
        #endregion

        #region Basic component variables
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

        #region Basic component logic
        public IDictionary<string, Action> ContextMenuControls => null;

        public MomodoraRandomizer(LiveSplitState state)
        {
            RandomizerLabel = new SimpleLabel();
            Settings = new MomodoraRandomizerSettings();

            state.OnStart += OnStart;
            state.OnReset += OnReset;

            CurrentState = state;
        }

        public void prepareDraw(LiveSplitState state)
        {
            RandomizerLabel.Font = Settings.OverrideTextFont ? Settings.TextFont : state.LayoutSettings.TextFont;
            RandomizerLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            RandomizerLabel.OutlineColor = Settings.OverrideTextColor ? Settings.OutlineColor : state.LayoutSettings.TextOutlineColor;
            RandomizerLabel.ShadowColor = Settings.OverrideTextColor ? Settings.ShadowColor : state.LayoutSettings.ShadowsColor;

            RandomizerLabel.VerticalAlignment = StringAlignment.Center;
            RandomizerLabel.HorizontalAlignment = StringAlignment.Center;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) => throw new NotImplementedException();

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
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

        private void DrawBackground(Graphics g, float width, float height)
        {
            if (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.A > 0)
            {
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
        #endregion

        #region Component Events
        private void OnStart(object sender, EventArgs e)
        {
            if (IsProcessRunning(GameProcess))
            {
                if ((GameVersion = GetGameVersion()) != "")
                {
                    string WhatcherName = "";

                    Items.Version = GameVersion;
                    Items.Process = GameProcess;

                    // Generate seed
                    if (!Settings.RandomSeed)
                    {
                        int.TryParse(Settings.seed_get(), out seed);
                        Rnd = new Random(seed);
                    }
                    else
                    {
                        Rnd = new Random();
                        seed = Rnd.Next();
                        Settings.seed_set(seed);
                        Rnd = new Random(seed);
                    }

                    // Fill item list if its empty
                    if (!Items.List.Any())
                    {
                        Items.List = Enum.GetValues(typeof(ItemName))
                                            .Cast<ItemName>()
                                            .Except(Items.ExcludeList)
                                            .Select(itemName => new Items(itemName))
                                            .ToList();
                    }

                    // Copy items in a random order based on the seed
                    List<Items> ItemsCopy = new List<Items>(Items.List.OrderBy(_ => Rnd.Next()).ToList());

                    // Randomize items and assign them to the item list (so we know which item to give the player when a specific item is collected)
                    foreach (Items item in Items.List)
                    {
                        Items NewReference = FindValidReference(item, ItemsCopy);
                        if(NewReference != null)
                        {
                            item.ItemReference = NewReference;
                            NewReference.ReferencedBy = item;
                            NewReference.DependsOn.AddRange(item.DependsOn.Except(NewReference.DependsOn));
                        }
                        else
                        {
                            NewReference = AssignReferenceChain(item, ItemsCopy);
                        }
                        ItemsCopy.Remove(NewReference);
                        Debug.WriteLine("{0} transforms into {1}", item.ItemName, item.ItemReference.ItemName);
                    }

                    // Prepare the offsets of the pointers
                    PrepareOffsets();

                    WhatcherName = "LevelId";
                    ContainerWatcher<int>.List.Add(new ContainerWatcher<int>(WhatcherName, GameProcess, Offsets[WhatcherName], (old, current) =>
                    {
                        Debug.WriteLine("LevelId_Current: " + current + ", LevelId_Old: " + old);
                        if(current == 1)
                        {
                            Items.ResetValues();
                        }
                        else
                        {
                            Items.SetValues();
                        }
                    }));

                    WhatcherName = "Map_X";
                    ContainerWatcher<double>.List.Add(new ContainerWatcher<double>(WhatcherName, GameProcess, Offsets[WhatcherName], (old, current) =>
                    {
                        Debug.WriteLine("Map_X_Current: " + current + ", Map_X_Old: " + old);
                    }));

                    WhatcherName = "Map_Y";
                    ContainerWatcher<double>.List.Add(new ContainerWatcher<double>(WhatcherName, GameProcess, Offsets[WhatcherName], (old, current) => 
                    {
                        Debug.WriteLine("Map_Y_Current: " + current + ", Map_Y_Old: " + old);
                    }));

                    // TODO Add relevant watchers logic
                }
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (IsProcessRunning(GameProcess, invalidator, width, height))
            {
                GameVersion = GetGameVersion(invalidator, width, height);
                if(GameVersion != "" && CurrentState.CurrentPhase == TimerPhase.Running)
                {
                    ContainerWatcher<int>.UpdateWatchers(GameProcess);
                    ContainerWatcher<double>.UpdateWatchers(GameProcess);
                }
            }
            else
            {
                GameProcess = GetProcess(PROCESS_NAME);
            }
        }

        private void OnReset(object sender, TimerPhase value)
        {
            //if ((pGameProcess = GetProcess(PROCESS_NAME)) != null)
            //{
                ContainerWatcher<int>.Clear();
                ContainerWatcher<double>.Clear();
                Offsets.Clear();
                Items.Reset();
            //}
            //TODO Log events if necessary
        }

        public void Dispose()
        {
            CurrentState.OnStart -= OnStart;
            CurrentState.OnReset -= OnReset;
        }
        #endregion

        #region Randomizer logic
        /// <summary>
        /// Finds a valid reference item for the specified current item.
        /// </summary>
        /// <param name="currentItem">The current item for which to find a valid reference.</param>
        /// <param name="itemList">The list of items to search for a valid reference.</param>
        /// <returns>The first valid reference item found; otherwise, null if no valid reference is found.</returns>
        private Items FindValidReference(Items currentItem, List<Items> itemList)
        {
            return itemList.FirstOrDefault(potentialReference =>
                !currentItem.DependsOn.Contains(potentialReference.ItemName));
        }

        // Needs revision to make recursive so items can try to find a new valid reference besides themselves
        /// <summary>
        /// Reassign references in a chain from the specified item.
        /// </summary>
        /// <param name="currentItem">The item from which the chain starts.</param>
        /// <param name="itemsCopy">The list of items to use for findig references.</param>
        /// <returns>The last valid item found in the list.</returns>
        private Items AssignReferenceChain(Items currentItem, List<Items> itemsCopy)
        {
            Items NewReference;
            do
            {
                Items referencesCurrent = currentItem.ReferencedBy;
                currentItem.ItemReference = currentItem;
                currentItem.ReferencedBy = currentItem;
                foreach (ItemName itemName in referencesCurrent.DependsOn)
                    currentItem.DependsOn.Remove(itemName);
                currentItem = referencesCurrent;
                NewReference = FindValidReference(currentItem, itemsCopy);
            } while ( NewReference == null);
            NewReference.DependsOn.AddRange(currentItem.DependsOn.Except(NewReference.DependsOn));
            return currentItem.ItemReference = NewReference;
        }

        /// <summary>
        /// Adds a collection of offsets to a dictionary.
        /// </summary>
        private void PrepareOffsets()
        {
            switch(GameVersion)
            {
                case VERSION_1_05b:
                    Offsets.Add("LevelId",  new int[] { 0x230F1A0 });
                    //ADD X, Y Map coord
                    //ADD Doors
                    //ADD Green Leaf
                    //ADD Dirty Shroom
                    //ADD Shops
                    //ADD Language
                    //ADD Strings
                    break;
                case VERSION_1_07:
                    Offsets.Add("LevelId",  new int[] { 0x237C360 });
                    Offsets.Add("Map_X",    new int[] { 0x2371EA8, 0x4, 0x7B0 });
                    Offsets.Add("Map_Y",    new int[] { 0x2371EA8, 0x4, 0x7C0 });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks if a process with the specified name is currently running and returns a reference.
        /// </summary>
        /// <param name="processName">The name of the process to check.</param>
        /// <returns>
        ///     A <see cref="Process"/> reference if the process is running; otherwise, <c>null</c>.
        /// </returns>
        private Process GetProcess(string processName)
        {
            Process[] game = Process.GetProcessesByName(processName);
            Process process = null;

            if (game.Length > 0)
            {
                process = game[0];
            }

            return process;
        }

        /// <summary>
        /// Checks if the specified process is currently running.
        /// </summary>
        /// <param name="process">The reference to the process to check.</param>
        /// <param name="invalidator">Component Invalidator.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <returns>
        ///     <c>true</c> if the process is running; otherwise, <c>false</c>.
        /// </returns>
        private bool IsProcessRunning(Process process, IInvalidator invalidator = null, float width = 0, float height = 0)
        {
            bool Running = process != null && !process.HasExited;

            if (invalidator != null && !Running)
            {
                SetSimpleLabelText("Game not running", invalidator, width, height);
            }

            return Running;
        }

        /// <summary>
        ///     Gets the version of the Game based on the module memory size.
        /// </summary>
        /// <param name="invalidator">Component Invalidator</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>
        ///     A <c>string</c> with the version value if it's supported; otherwise, an empty string.
        /// </returns>
        private string GetGameVersion(IInvalidator invalidator = null, float width = 0, float height = 0)
        {
            string text, version;

            switch (GameProcess.MainModule.ModuleMemorySize)
            {
                case 39690240:
                    text = "Supported version detected: " + VERSION_1_05b;
                    version = VERSION_1_05b;
                    break;
                case 40222720:
                    text = "Supported version detected: " + VERSION_1_07;
                    version = VERSION_1_07;
                    break;
                default:
                    text = "Version not supported";
                    version = "";
                    break;
            }

            if(invalidator != null)
            {
                SetSimpleLabelText(text, invalidator, width, height);
            }

            return version;
        }

        /// <summary>
        /// Changes the text of the RandomizerLabel and invalidates the state if it's different.
        /// </summary>
        /// <param name="newString">The text to set RandomizerLabel to.</param>
        /// <param name="invalidator">Component Invalidator.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        private void SetSimpleLabelText(string newString, IInvalidator invalidator = null, float width = 0, float height = 0)
        {
            if(RandomizerLabel.Text != newString)
            {
                RandomizerLabel.Text = newString;
                invalidator?.Invalidate(0, 0, width, height);
            }
        }
        #endregion
    }
}