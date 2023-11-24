using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Windows.Forms.VisualStyles;

namespace LiveSplit.UI.Components
{
    public class MomodoraRandomizer : IComponent
    {
        #region Randomizer variables
        private readonly string PROCESS_NAME = "MomodoraRUtM";
        private Process pGameProcess = null;
        private string sGameVersion = "";
        private Random Rnd = new Random();
        #region Pointers
        private Dictionary<string, int[]> Offsets = new Dictionary<string, int[]>();
        #endregion
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
            if (IsProcessRunning(pGameProcess))
            {
                if ((sGameVersion = GetGameVersion()) != "")
                {
                    if (Items.ItemsList.Count() == 0)
                    {
                        PopulateItemList(Items.ItemsList);
                    }

                    //TODO Rendomize items taking into account placement restrictions
                    List<Items> ItemsCopy = new List<Items>(Items.ItemsList);
                    foreach (Items item in Items.ItemsList)
                    {
                        int Index = Rnd.Next(ItemsCopy.Count);
                        int ItemId = ItemsCopy[Index].Id;

                        item.ItemReference = Items.ItemsList[ItemId];
                        ItemsCopy.RemoveAt(Index);
                    }

                    PrepareOffsets();

                    new ContainerWatcher<int>("LevelId", pGameProcess, Offsets["LevelId"], true, (old, current) =>
                    {
                        Debug.WriteLine("Current: " + current + ", Old: " + old);
                        if(current == 1)
                        {
                            //MainMenu, restore values
                        }
                        else
                        {
                            //InGame, Do Stuff
                        }
                    });

                    new ContainerWatcher<double>("Map_X", pGameProcess, Offsets["Map_X"], true, (old, current) =>
                    {
                        Debug.WriteLine("Current: " + current + ", Old: " + old);
                    });

                    new ContainerWatcher<double>("Map_Y", pGameProcess, Offsets["Map_Y"], true, (old, current) =>
                    {
                        Debug.WriteLine("Current: " + current + ", Old: " + old);
                    });
                }
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (IsProcessRunning(pGameProcess, invalidator, width, height))
            {
                sGameVersion = GetGameVersion(invalidator, width, height);
                if(sGameVersion != "" && CurrentState.CurrentPhase == TimerPhase.Running)
                {
                    ContainerWatcher<int>.UpdateWatchers(pGameProcess);

                    //TODO Check language and update strings if necessary
                }
            }
            else
            {
                pGameProcess = GetProcess(PROCESS_NAME);
            }
        }

        private void OnReset(object sender, TimerPhase value)
        {
            if ((pGameProcess = GetProcess(PROCESS_NAME)) != null)
            {
                ContainerWatcher<int>.ClearWatchers();
                //TODO Reset Values and strings
            }
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
        /// Populate a list with the items of the game
        /// </summary>
        /// <param name="List">List where the items will be stored</param>
        private void PopulateItemList(List<Items> List)
        {
            foreach(ItemValue item in Enum.GetValues(typeof(ItemValue)))
            {
                List.Add(new Items(item, sGameVersion));
            }
        }

        /// <summary>
        /// Create pointers for the different variables to track
        /// </summary>
        /// <param name="version">Version of the game</param>
        /// <param name="process">Proces to get pointer from</param>
        private void PrepareOffsets()
        {
            switch(sGameVersion)
            {
                case "1.05b":
                    Offsets.Add("RoomId", new int[] { 0x230F1A0 });
                    //ADD X, Y Map coord
                    //ADD Doors
                    //ADD Green Leaf
                    //ADD Dirty Shroom
                    //ADD Shops
                    //ADD Language
                    //ADD Strings
                    break;
                case "1.07":
                    Offsets.Add("RoomId", new int[] { 0x237C360 });
                    Offsets.Add("Map_X",  new int[] { 0x2371EA8, 0x4, 0x7B0 });
                    Offsets.Add("Map_Y",  new int[] { 0x2371EA8, 0x4, 0x7C0 });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check if the process with name is currently running and return a reference
        /// </summary>
        /// <param name="procesName">Name of the process to check</param>
        /// <param name="invalidator">Component Invalidator</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>
        ///     <c>Process</c> reference if the process is running.<br/>
        ///     <c>null</c> otherwise
        /// </returns>
        private Process GetProcess(string procesName)
        {
            Process[] game = Process.GetProcessesByName(procesName);
            Process process = null;

            if (game.Length > 0)
            {
                process = game[0];
            }

            return process;
        }

        /// <summary>
        /// Check if the process is currently running
        /// </summary>
        /// <param name="process">Reference to the process</param>
        /// <param name="invalidator">Component Invalidator</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>
        ///     <c>True</c> if the process is running<br/>
        ///     <c>False</c> otherwise
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
        ///     Get version of the process based on module memory size
        /// </summary>
        /// <param name="invalidator">Component Invalidator</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>
        ///     <c>string</c> with the version value if it's supported<br/>
        ///     <c>empty string</c> otherwise
        /// </returns>
        private string GetGameVersion(IInvalidator invalidator = null, float width = 0, float height = 0)
        {
            string text, version;

            switch (pGameProcess.MainModule.ModuleMemorySize)
            {
                case 39690240:
                    text = "Supported version detected: v1.05b";
                    version = "1.05b";
                    break;
                case 40222720:
                    text = "Supported version detected: v1.07";
                    version = "1.07";
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
        /// Change text of RandomizerLabel and invalidate state if it's different
        /// </summary>
        /// <param name="newString">text to set RandomizerLabel to</param>
        /// <param name="invalidator">Component Invalidator</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
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